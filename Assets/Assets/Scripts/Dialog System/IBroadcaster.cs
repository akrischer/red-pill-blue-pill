using UnityEngine;
using System.Collections;

public interface IBroadcaster {

    event System.EventHandler TimePassed;
    event System.EventHandler FinishedDisplaying;
}
