using UnityEngine;
using System.Collections;

public interface ISubscriber {

    event System.EventHandler SubscriberFinished;
}
