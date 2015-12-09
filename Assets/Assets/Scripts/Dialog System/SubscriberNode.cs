using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SubscriberNode : Node, ISubscriber
{
    // This node listens for the subscriber to be finished
    internal BroadcasterNode listenFor;
    internal string text;

    public event EventHandler SubscriberFinished;

    // TODO: Can be part of subcriber/broadcaster interface pattern
    public override void HookupEvents()
    {
        listenFor.FinishedDisplaying += new EventHandler(Activate);
    }

    // TODO: IMPLEMENT!!!!!!!
    public void Activate(object sender, EventArgs e)
    {
        // show options, blah blah...
        Debug.Log("Subscriber node with text '" + text + "' is activated");
        UserChoicesUnderlay.Instance.Accept(this);
    }

    public void OnSubscriberFinished(EventArgs e)
    {
        Debug.Log("Subscriber node with text '" + text + "' has finished!");
        if (SubscriberFinished != null)
        {
            SubscriberFinished(this, e);
        }
    }

    // If this is called, that means THIS subscriber node was the user's entered choice.
    // Call computerTerminal.Accept(<this>) -- 0 delay
    // Call OnSubscriberFinished
    public void OnChoiceEntered(EventArgs e)
    {
        string prependText = System.Environment.NewLine + "  >";
        NodePackage np = new NodePackage(this, new List<AbstractTerminal.StringToDraw> { new AbstractTerminal.StringToDraw(prependText + text, 0, true) });
        ComputerTerminal.AcceptNodePackage(np);
        UserChoicesUnderlay.FlushUserChoices();
        OnSubscriberFinished(EventArgs.Empty);
    }
}
