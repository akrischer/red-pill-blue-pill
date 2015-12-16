using UnityEngine;
using System.Collections;
using System;

public abstract class ANode {
    // Called when this node is done and wants to trigger the next
    public abstract void Next();

    // Add this delegate to this node
    public abstract void AcceptDelegate(EventHandler eventHandler);

    // Do this node's action... then call next
    public abstract void Activate(object sender, EventArgs e);

    // Remove this delegate from this node
    public abstract void RemoveDelegate(EventHandler eventHandler);
}
