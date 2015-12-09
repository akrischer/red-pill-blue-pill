using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System.Linq;

public class DialogObjectBuilder : MonoBehaviour {

    public TextAsset dialogJson;

    private Dictionary<string, JSON_CSharpPairing<SubscriberNode>> subscriberDict;
    private Dictionary<string, JSON_CSharpPairing<BroadcasterNode>> broadcasterDict;
    private bool inited = false;
    public bool Initiated
    {
        get
        {
            return inited;
        }
    }

    private struct JSON_CSharpPairing<T>
    {
        public JSONNode jsonNode;
        public T csharpObject;
        public JSON_CSharpPairing(JSONNode jsonNode, T csharpObject)
        {
            this.jsonNode = jsonNode;
            this.csharpObject = csharpObject;
        }
    }

	// Use this for initialization
	void Start () {
        BuildDialogTree();
	}
	
	private void BuildDialogTree()
    {
        if (inited)
        {
            Debug.LogWarning("Tried initializing dialog when it's already been inited");
            return;
        }
        inited = true;
        subscriberDict = new Dictionary<string, JSON_CSharpPairing<SubscriberNode>>();
        broadcasterDict = new Dictionary<string, JSON_CSharpPairing<BroadcasterNode>>();
        // Populate dicts from JSON
        PopulateDictionariesFromJson();

        // Hookup subscribers and broadcaster
        CreateAndHookupNodes();

        // Trigger first broadcast node
        JSON_CSharpPairing<BroadcasterNode> firstBroadcaster;
        if (broadcasterDict.TryGetValue("B1", out firstBroadcaster))
        {
            firstBroadcaster.csharpObject.Activate(this, System.EventArgs.Empty);
        }
        else
        {
            Debug.LogError("Cannot kickoff dialog tree. No node with id 'B1'!");
        }
    }

    private void PopulateDictionariesFromJson()
    {
        if (dialogJson == null)
        {
            Debug.LogError("Dialog JSON must be defined in order to start!");
            return;
        }

        var slurpedJson = JSON.Parse(dialogJson.text);
        var sNodes = slurpedJson["subscriberNodes"];
        var bNodes = slurpedJson["broadcasterNodes"];

        for (int i = 0; i < sNodes.AsArray.Count; i++)
        {
            var sNode = sNodes[i];
            string id = sNode["id"].Value;
            SubscriberNode newNode = new SubscriberNode();
            newNode.text = sNode["text"].Value;
            JSON_CSharpPairing<SubscriberNode> pairing = new JSON_CSharpPairing<SubscriberNode>(sNode, newNode);
            subscriberDict.Add(id, pairing);
        }

        for (int i = 0; i < bNodes.AsArray.Count; i++)
        {
            var bNode = bNodes[i];
            string id = bNode["id"].Value;
            BroadcasterNode newNode = new BroadcasterNode();
            JSON_CSharpPairing<BroadcasterNode> pairing = new JSON_CSharpPairing<BroadcasterNode>(bNode, newNode);
            broadcasterDict.Add(id, pairing);
        }
    }

    private void CreateAndHookupNodes()
    {
        /**
            Foreach broadcaster node:
                - find all subscriber nodes whose 'nodeToActivate' == broadcaster's id
                    - Add these subscriber nodes to broadcaster.activatorNodes
                - find all subscriber nodes that should listen to broadcaster
                    - Foreach subscriber node, set subscriber.listenFor to broadcaster

            Then, foreach broadcaster node and foreach subscriber node:
                - node.hookup();
        */
        foreach (KeyValuePair<string, JSON_CSharpPairing<BroadcasterNode>> entry in broadcasterDict)
        {
            string bId = entry.Key;
            var bNode = entry.Value.jsonNode;
            BroadcasterNode broadcaster = entry.Value.csharpObject;

            // Setup broadcaster's messages
            for (int i = 0; i < bNode["messages"].AsArray.Count; i++)
            {
                broadcaster.AddMessage(bNode["messages"][i]["text"].Value, bNode["messages"][i]["delay"].AsFloat);
            }

            // Find all subscriber nodes whose associated 'nodeToActivate' == bId
            List<SubscriberNode> activatorNodes = subscriberDict
                .Select(kv => kv.Value)
                .Where(pairing => bId == pairing.jsonNode["nodeToActivate"].Value)
                .Select(pairing => pairing.csharpObject)
                .ToList<SubscriberNode>();

            // Add these subscriber nodes to broadcaster's activator nodes
            broadcaster.activatorNodes.AddRange(activatorNodes);

            // Find all subscriber nodes in bNode.responses
            List<SubscriberNode> listenerNodes = subscriberDict
                .Select(kv => kv.Value)
                .Where(pairing => JSONArrayContainsString(bNode["responses"].AsArray, pairing.jsonNode["id"].Value))
                .Select(pairing => pairing.csharpObject)
                .ToList<SubscriberNode>();

            // Foreach subscriber node, set listenFor to this broadcaster node
            foreach(SubscriberNode listener in listenerNodes)
            {
                listener.listenFor = broadcaster;
            }
        }

        // hookup each node
        foreach (JSON_CSharpPairing<SubscriberNode> subPairing in subscriberDict.Values)
        {
            subPairing.csharpObject.HookupEvents();
        }
        foreach (JSON_CSharpPairing<BroadcasterNode> broadPairing in broadcasterDict.Values)
        {
            broadPairing.csharpObject.HookupEvents();
        }
    }

    private bool JSONArrayContainsString(JSONArray array, string value)
    {
        for (int i = 0; i < array.Count; i++)
        {
            if (array[i].Value == value)
            {
                return true;
            }
        }
        return false;
    }
}
