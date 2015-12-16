using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class BroadcasterNode : TextNode, IBroadcaster
{
    private EventHandler notifySubscribers;

    // Node that will kick off this node
    internal List<SubscriberNode> activatorNodes = new List<SubscriberNode>();
    // Subscribed nodes (i.e.
    // list of strings to display
    private List<AbstractTerminal.StringToDraw> messages = new List<AbstractTerminal.StringToDraw>();
    //{
    //    new AbstractTerminal.StringToDraw("Message 1 to display on terminal.", 0.8f, true),
    //    new AbstractTerminal.StringToDraw("Message 2 to display on terminal.", 1.5f, true),
    //    new AbstractTerminal.StringToDraw("Message 3 to display on terminal.", 1.5f, true),
    //    new AbstractTerminal.StringToDraw("Message 4 to display on terminal.", 0.8f, true)
    //};

    public event EventHandler TimePassed;
    public event EventHandler FinishedDisplaying;

    // lastTimePassedEventFired = Time.time when the last TimePassed event was fired
    // Once Time.time - lastTimePassedEventFired > 1, we fire new TimePassed event and reset lastTimePassedEventFired
    // Send down activatedTime in event to describe how much time has actually passed
    private float lastTimePassedEventFired;
    private float activatedTime;

    public class TimePassedEventArgs : EventArgs
    {
        public float timePassed;
        public TimePassedEventArgs(float time)
        {
            this.timePassed = time;
        }
    }

    public void AddMessage(string str, float delay)
    {
        messages.Add(new AbstractTerminal.StringToDraw(str, delay, true));
    }

    // call this once activatorNodes set
    public override void HookupEvents()
    {
        // keep reference to delegate, as we want to unsubscribe as soon as we notify our subscribers.
        notifySubscribers = new EventHandler(NotifySubscribers);
        // TODO: Hook up nodes from the bottom up. Each broadcaster node has an activator node, which is a SubscriberNode,
        // as a user's actions prompt the next node to activate!
        foreach (SubscriberNode snode in activatorNodes)
        {
            snode.SubscriberFinished += new EventHandler(Activate);
        }
    }

    #region ANode Methods

    public override void Next()
    {
        FinishedDisplaying(this, EventArgs.Empty);
    }

    public override void AcceptDelegate(EventHandler eventHandler)
    {
        FinishedDisplaying += eventHandler;
    }

    public override void RemoveDelegate(EventHandler eventHandler)
    {
        FinishedDisplaying -= eventHandler;
    }

    public override void Activate(object sender, EventArgs e)
    {
        isActive = true;
        activatedTime = Time.time;
        lastTimePassedEventFired = Time.time;
        // Do sequence.
        // add an empty delay so it doesn't start RIGHT after the user enters their choice
        messages.Insert(0, AbstractTerminal.StringToDraw.EMPTY_DELAY(1f));
        ComputerTerminal.FinishedDisplaying += notifySubscribers;
        ComputerTerminal.AcceptNodePackage(new NodePackage(this, messages));
        // listen for finish    
        Utils.StaticStartCoroutine(FireTimePassedEvents());
    }

    #endregion

    private IEnumerator FireTimePassedEvents()
    {
        float startTime = Time.time;
        while (isActive)
        {
            yield return new WaitForSeconds(1f);
            if (isActive)
            {
                OnTimePassed(new TimePassedEventArgs(Time.time - startTime));
            }
        }
    }

    #region On-Event Methods
    private void OnTimePassed(TimePassedEventArgs e)
    {
        // TODO: Pass down (Time.time - activatedTime)
        if (TimePassed != null)
        {
            TimePassed(this, e);
        }
    }

    private void NotifySubscribers(object sender, EventArgs e)
    {
        Debug.Log("Broadcaster node finished-- notifying subscribers");
        ComputerTerminal.FinishedDisplaying -= notifySubscribers;
        isActive = false;
        // notify all subscribed nodes
        if (FinishedDisplaying != null)
        {
            FinishedDisplaying(this, e);
        }
        UserChoicesUnderlay.Instance.Show();
    }
    #endregion
}
