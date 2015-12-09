using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public abstract class AbstractTerminal : MonoBehaviour {

    public bool testTextRendering;
    public static float charDrawDelay = .03f;
    protected float setCharDrawDelay;

    protected Text text;
    // A queue that holds strings to animate. Each will animate as soon as possible.
    protected Queue<Node.NodePackage> toDisplay = new Queue<Node.NodePackage>();
    protected bool isDrawing;
    protected Coroutine currentDrawingAnimation;
    protected string textAfterDrawing;

    public struct StringToDraw
    {
        public string str;
        public float delay;
        public bool atOnce;

        public StringToDraw(string str, float delay, bool atOnce)
        {
            this.str = str;
            this.delay = delay;
            this.atOnce = atOnce;
        }

        public static StringToDraw EMPTY_DELAY(float delay)
        {
            return new StringToDraw("", delay, true);
        }
    }

    public bool IsDrawing()
    {
        return currentDrawingAnimation != null;
    }

    protected void StartDrawAnimation()
    {
        if (CanDraw())
        {
            StartCoroutine(DequeueAndAnimateText()); ;
        }
        
    }

    // pseudo-Event method
    protected virtual void DrawingEnded()
    {
        if (toDisplay.Count > 0)
        {
            StartDrawAnimation();
        }
        else
        {
            currentDrawingAnimation = null;
            isDrawing = false;
        }
    }

    // pseudo-Event method
    protected void PackageAddedToQueue()
    {
        if (CanDraw())
        {
            StartDrawAnimation();
        }
    }

    public void LockDrawing()
    {
        isDrawing = true;
    }

    public void UnlockDrawing()
    {
        isDrawing = false;
    }

    public bool CanDraw()
    {
        return !isDrawing && toDisplay.Count > 0;
    }

    /// <summary>
    ///     Add a string for the system to draw. Strings are drawn in a FIFO basis, so calling this method
    ///     does not guarantee the string will immediately begin drawing.
    /// </summary>
    /// <param name="str">String to draw</param>
    /// <param name="delay">Optional param, to specify delay before the message is written</param>
    public void AcceptNodePackage(Node.NodePackage package, bool skipCurrent = false)
    {
        if (skipCurrent) { SkipCurrentSequence(); }
        toDisplay.Enqueue(package);
        PackageAddedToQueue();
    }

    public virtual void ClearTerminal()
    {
        text.text = "";
        textAfterDrawing = "";
    }

    protected virtual IEnumerator DequeueAndAnimateText()
    {
        Node.NodePackage nodePackage = toDisplay.Dequeue();
        foreach (StringToDraw std in nodePackage.messages)
        {
            LockDrawing();
            currentDrawingAnimation = StartCoroutine(AnimateText(std));
            yield return currentDrawingAnimation;
            UnlockDrawing();
        }
        DrawingEnded();
    }

    protected abstract IEnumerator AnimateText(StringToDraw std);

    public abstract void SkipCurrentSequence();
}
