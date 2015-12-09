using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node {

    public struct NodePackage
    {
        public Node node;
        public List<AbstractTerminal.StringToDraw> messages;

        public NodePackage(Node node, List<AbstractTerminal.StringToDraw> mes)
        {
            this.node = node;
            this.messages = mes;
        }
    }
    #region hidden variables -- do not modify/access
    private ComputerTerminal _computerTerminal; //don't use this!
    #endregion
    protected ComputerTerminal ComputerTerminal
    {
        get
        {
            if (_computerTerminal == null)
            {
                _computerTerminal = GameObject.FindObjectOfType<ComputerTerminal>();
            }
            return _computerTerminal;
        }
    }
    protected bool isActive = false;

    public virtual void HookupEvents()
    {
        throw new System.NotImplementedException();
    }
}
