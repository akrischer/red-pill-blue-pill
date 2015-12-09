using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class ComputerTerminalInput : AbstractTerminal {

    public char blinkCharacter = '_';
    public char inputChar = '>';
    // a runtime bool that if true, will trigger to clear terminal directly after drawing
    // Then it's reset to false.
    private Queue<DrawSettings> interruptDrawingQueue = new Queue<DrawSettings>();
    private float m_TimeStamp;
    private int blinkingCharIdx;

    private ComputerTerminal computerTerminal;

    struct DrawSettings
    {
        public float charDrawDelay;
        public bool clearAfterDrawing;
        public bool invisible;

        DrawSettings(float delay, bool clearAfterDrawing, bool invis)
        {
            this.charDrawDelay = delay;
            this.clearAfterDrawing = clearAfterDrawing;
            this.invisible = invis;
        }
        public static DrawSettings SKIPPED
        {
            get
            {
                return new DrawSettings(.0001f, true, true);
            }
        }
    }

    public class TestCycleText
    {
        string text;
        private static List<TestCycleText> allTexts = new List<TestCycleText>();
        private static int currentIdx = 0;

        public TestCycleText(string text)
        {
            this.text = text;
            allTexts.Add(this);
        }

        public static string Next()
        {
            currentIdx += 1;
            currentIdx %= allTexts.Count;
            return allTexts[currentIdx].text;
        }
    }

    void Start()
    {
        setCharDrawDelay = charDrawDelay;
        computerTerminal = gameObject.GetComponent<ComputerTerminal>();
        text = GameObject.Find(Constants.COMPUTER_OBJ_NAME + "_input_text").GetComponent<Text>();
        text.text = "> ";
        blinkingCharIdx = text.text.Length - 1;

        if (testTextRendering)
        {
            new TestCycleText("1111111111111111111111111111111111?");
            new TestCycleText("2222222222222222222222222222222222");
            new TestCycleText("3333333333333333333333333333333333");
            new TestCycleText("4444444444444444444444444444444444");
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (text.text == "")
        {
            text.text += " ";
        }
        int lastIndex = text.text.Length - 1;
        if (blinkingCharIdx >= 0 && text.text[lastIndex] == ' ' || text.text[lastIndex] == blinkCharacter)
        {
            blinkingCharIdx = lastIndex;
        }
        // Every so often, toggle blinkinkChar from ' ' and the specified blink character.
        if (Time.time - m_TimeStamp >= 0.5 && !isDrawing && blinkingCharIdx >= 0)
        {
            m_TimeStamp = Time.time;
            if (text.text.Length > blinkingCharIdx)
            {
                if (text.text[blinkingCharIdx] == blinkCharacter)
                {
                    char[] newText = text.text.ToCharArray();
                    newText[blinkingCharIdx] = ' ';
                    text.text = new string(newText);

                }
                else
                {
                    char[] newText = text.text.ToCharArray();
                    newText[blinkingCharIdx] = blinkCharacter;
                    text.text = new string(newText);
                }
            }

        }

        if (Input.GetKeyDown(KeyCode.RightArrow) && testTextRendering)
        {
            SkipCurrentSequence();
            AcceptNodePackage(new Node.NodePackage(new Node(), new List<StringToDraw> { new StringToDraw(TestCycleText.Next(), 0.4f, false) }));
        }
    }

    /// <summary>
    ///     Skip the current drawing sequence.
    /// </summary>
    public override void SkipCurrentSequence()
    {
        if (currentDrawingAnimation == null)
        {
            Debug.Log("can't skip current sequence-- its null");
            return;
        }
        interruptDrawingQueue.Enqueue(DrawSettings.SKIPPED);

    }

    /// <summary>
    /// This is overwriting parent's definition because only 1 message should be queued at a time
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator DequeueAndAnimateText()
    {
        Node.NodePackage nodePackage = toDisplay.Dequeue();
        if (nodePackage.messages == null || nodePackage.messages.Count != 1)
        {
            Debug.LogWarning("Processed input node package; node package either null or message count != 1");
            yield break;
        }
        currentDrawingAnimation = StartCoroutine(AnimateText(nodePackage.messages[0]));
        yield return currentDrawingAnimation;
        DrawingEnded();
    }

    protected override IEnumerator AnimateText(StringToDraw stringToDrawStruct)
    {
        LockDrawing();
        ClearTerminal();

        string stringToDraw = stringToDrawStruct.str;
        float delay = stringToDrawStruct.delay;
        bool drawAtOnce = stringToDrawStruct.atOnce;
        bool clearAfterDraw = false;
        bool receivedInterrupt = false;

        // wait the specified delay
        yield return new WaitForSeconds(delay);

        // We store the "finished" drawn text in case the user skips the current drawing sequence.
        textAfterDrawing = text.text + stringToDraw;

        if (drawAtOnce)
        {
            text.text = textAfterDrawing + '\n';
            UnlockDrawing();
            DrawingEnded();
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
                    if (interruptDrawingQueue.Count > 0 && !receivedInterrupt)
                    {
                        DrawSettings settings = interruptDrawingQueue.Dequeue();
                        charDrawDelay = settings.charDrawDelay;
                        if (settings.invisible)
                        {
                            Color col = text.color;
                            col.a = 0;
                            text.color = col;
                        }
                        if (settings.clearAfterDrawing)
                        {
                            clearAfterDraw = true;
                        }
                        receivedInterrupt = true;
                    }
                    yield return new WaitForSeconds(charDrawDelay);
                }
                // if we reached the end of the string,
                if (i == stringToDraw.Length - 1)
                {
                    if (clearAfterDraw) { ClearTerminal(); }
                    if (receivedInterrupt) { ResetDrawSettings(); }
                    RepositionBlinkingChar();
                    text.text += System.Environment.NewLine;
                    DrawingEnded();
                    UnlockDrawing();
                    yield break;
                }
            }
        }
    }

    public override void ClearTerminal()
    {
        text.text = "" + inputChar;
        textAfterDrawing = text.text;
    }

    internal void RepositionBlinkingChar()
    {
        blinkingCharIdx = text.text.Length;
        text.text += " ";
    }

    private void ResetDrawSettings()
    {
        Color c = text.color;
        c.a = 1;
        text.color = c;
        charDrawDelay = setCharDrawDelay;
    }

}
