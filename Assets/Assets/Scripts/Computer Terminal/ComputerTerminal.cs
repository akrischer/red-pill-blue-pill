using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;

/**
    A component that does everything a computer terminal needs to do:
        - Accept and display text
        - Blinking chat that represents current place
*/
public class ComputerTerminal : AbstractTerminal {

    public event System.EventHandler FinishedDisplaying;

    public void Start()
    {
        setCharDrawDelay = charDrawDelay;
        text = SceneObjectUtils.GetUIText(gameObject);
        isDrawing = false;

        // add all test messages
        if (testTextRendering)
        {
            TestAcceptNodePackage("...", 0.5f, true);
            TestAcceptNodePackage("Hello?", 1f, true);
            TestAcceptNodePackage("I just wanted to say", 1f, true);
            TestAcceptNodePackage("This should not come immediately after the last message", 3f, true);
            TestAcceptNodePackage("And this is a multiline message that will span many line breaks if all is going according to plan.", 3.5f, true);
            TestAcceptNodePackage("That is all.", 1.5f, true);
            for (int i = 0; i < 11; i++) { TestAcceptNodePackage(i + "...", 0.5f, true); }
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SkipCurrentSequence();
        }
    }

    public void TestAcceptNodePackage(string str, float delay, bool atOnce)
    {
        AcceptNodePackage(new Node.NodePackage(new SubscriberNode(), new List<StringToDraw> { new StringToDraw(str, delay, atOnce) }));
    }

    private void OnFinishedDisplaying(EventArgs e)
    {
        if (FinishedDisplaying != null)
        {
            FinishedDisplaying(this, e);
        }
    }


    /// <summary>
    ///     Skip the current drawing sequence.
    /// </summary>
    public override void SkipCurrentSequence()
    {
        if (currentDrawingAnimation == null)
        {
            Debug.Log("Cannot skip current sequence-- none currently going");
            return;
        }
        // Stop coroutine
        //StopCoroutine(currentDrawingAnimation);
        //currentDrawingAnimation = null;
        charDrawDelay = .01f;

        // Finish the drawing sequence
        //text.text = textAfterDrawing;
    }

    #region Displaying characters

    protected override IEnumerator AnimateText(StringToDraw stringToDrawStruct)
    {
        string stringToDraw = stringToDrawStruct.str;
        float delay = stringToDrawStruct.delay;
        bool drawAtOnce = stringToDrawStruct.atOnce;
        // wait the specified delay
        yield return new WaitForSeconds(delay);

        // We store the "finished" drawn text in case the user skips the current drawing sequence.
        textAfterDrawing = text.text + stringToDraw;

        if (drawAtOnce)
        {
            text.text = textAfterDrawing + '\n';
        }
        else
        {
            // Draw each character in the string. Don't do a draw delay for whitespace characters
            for (int i = 0; i < stringToDraw.Length; i++)
            {
                char c = stringToDraw[i];
                text.text += c;
                if (c != ' ')
                {
                    yield return new WaitForSeconds(charDrawDelay);
                }
                // if we reached the end of the string,
                if (i == stringToDraw.Length - 1)
                {
                    text.text += System.Environment.NewLine;
                    UnlockDrawing();
                    DrawingEnded();
                    charDrawDelay = setCharDrawDelay;
                    yield break;
                }
            }
        }
    }

    // pseudo-Event method
    protected override void DrawingEnded()
    {
        if (toDisplay.Count > 0)
        {
            StartDrawAnimation();
        }
        else
        {
            currentDrawingAnimation = null;
            isDrawing = false;
            OnFinishedDisplaying(EventArgs.Empty);
        }
    }

    private void ArchiveCurrentDisplay()
    {
        Debug.Log("Archiving current display");
        TerminalArchiveManager.StringArchive.Create(text.text);
        ClearTerminal();
    }
    #endregion

    #region String Archive Stuff

    public void ForceArchive()
    {
        SkipCurrentSequence();
    }

    #endregion
}
