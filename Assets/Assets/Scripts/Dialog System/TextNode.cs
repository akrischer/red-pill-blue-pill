using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TextNode : ANode {

    public struct NodePackage
    {
        public TextNode node;
        public List<AbstractTerminal.StringToDraw> messages;

        public NodePackage(TextNode node, List<AbstractTerminal.StringToDraw> mes)
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

    public override void Next()
    {
        throw new NotImplementedException();
    }

    public override void AcceptDelegate(EventHandler eventHandler)
    {
        throw new NotImplementedException();
    }

    public override void Activate(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    public override void RemoveDelegate(EventHandler eventHandler)
    {
        throw new NotImplementedException();
    }
}
