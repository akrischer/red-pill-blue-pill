using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class UserChoicesUnderlay : MonoBehaviour {

    #region Singleton Pattern
    private static UserChoicesUnderlay instance;
    public static UserChoicesUnderlay Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<UserChoicesUnderlay>();
            }
            return instance;
        }
    }
    #endregion

    private CanvasGroup _canvasGroup;
    private CanvasGroup canvasGroup
    {
        get
        {
            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.GetComponent<CanvasGroup>();
            }
            return _canvasGroup;
        }
    }

    private List<UserChoiceNode> choicesTexts = new List<UserChoiceNode>();
    private const string USER_CHOICE_TEXT = "user_choice_text_";
    private int currentCount = 0;
    public const int maxCount = 4;
    public bool isActive
    {
        get
        {
            return canvasGroup.alpha == 1 && canvasGroup.interactable;
        }
    }

    public class UserChoiceNode
    {
        // 0,0 = TOP LEFT
        // 0,1 = TOP RIGHT
        // 1,0 = BOTTOM LEFT
        // 1,1 == BOTOM RIGHT
        public static UserChoiceNode[,] choices = new UserChoiceNode[2, 2];
        private static int currentRow = 0;
        private static int currentColumn = 0;
        public static UserChoiceNode CurrentUserChoiceNode
        {
            get
            {
                return choices[currentRow, currentColumn];
            }
        }
        private static ComputerTerminalInput _computerTerminalInput;
        internal static ComputerTerminalInput computerTerminalInput
        {
            get
            {
                if (_computerTerminalInput == null)
                {
                    _computerTerminalInput = GameObject.FindObjectOfType<ComputerTerminalInput>();
                }
                return _computerTerminalInput;
            }
        }

        public SubscriberNode sub;
        public Text uiText;
        public UserChoiceNode(Text uiText)
        {
            this.uiText = uiText;
            this.sub = null;
            for (int r = 0; r < 2; r++)
            {
                for (int c = 0; c < 2; c++)
                {
                    if (choices[r, c] == null)
                    {
                        choices[r, c] = this;
                        return;
                    }
                }
            }
        }
        public bool IsEmpty()
        {
            // TODO: Instead of checking if "_", check against the set blinking char
            return uiText.text == " " || uiText.text == "_" || sub == null;
        }
        public void SetSubscriber(SubscriberNode newSub)
        {
            sub = newSub;
            uiText.text = newSub.text;
        }

        public static void UpdateInputTerminal()
        {
            if (CurrentUserChoiceNode != null)
            {
                AbstractTerminal.StringToDraw std = new AbstractTerminal.StringToDraw(CurrentUserChoiceNode.uiText.text, 0.2f, false);
                computerTerminalInput.AcceptNodePackage(new Node.NodePackage(CurrentUserChoiceNode.sub, new List<AbstractTerminal.StringToDraw> { std }));
            }
        }

        public static void FlushInputTerminal()
        {
            AbstractTerminal.StringToDraw std = new AbstractTerminal.StringToDraw(" ", 0.2f, true);
            computerTerminalInput.AcceptNodePackage(new Node.NodePackage(CurrentUserChoiceNode.sub, new List<AbstractTerminal.StringToDraw> { std }));
        }


        public static void OnLeftRightArrowKey()
        {
            currentColumn = (currentColumn + 1) % 2;
            UpdateInputTerminal();
        }
        public static void OnUpDownArrowKey()
        {
            currentRow = (currentRow + 1) % 2;
            UpdateInputTerminal();
        }
    }

	// Use this for initialization
	void Start () {
        // init refs to user choices texts
	    for (int i = 0; i < 4; i++)
        {
            Text child = GameObject.Find(USER_CHOICE_TEXT + i).GetComponent<Text>();
            child.text = " ";
            choicesTexts.Add(new UserChoiceNode(child));
        }
        Hide();
	}

    void Update()
    {
        if (isActive)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                UserChoiceNode.OnLeftRightArrowKey();
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                UserChoiceNode.OnUpDownArrowKey();
            }
            if (Input.GetKeyDown(KeyCode.Return) && !UserChoiceNode.CurrentUserChoiceNode.IsEmpty())
            {
                SelectChoice(UserChoiceNode.CurrentUserChoiceNode);
            }
        }
    }
	
	public void Show()
    {
        StartCoroutine(ShowAnimation());
    }
    private IEnumerator ShowAnimation()
    {
        yield return new WaitForSeconds(1f);
        float t = 0;
        float a = 0;
        while (t <= 1)
        {
            float alpha = Mathf.Lerp(0, 1, t);
            canvasGroup.alpha = alpha;
            t += .02f + a;
            a += .01f;
            yield return null;
        }
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
    }

    public void Hide()
    {
        StartCoroutine(HideAnimation());
    }
    private IEnumerator HideAnimation()
    {
        canvasGroup.interactable = false;
        float t = 0;
        float a = 0;
        while (t <= 1)
        {
            float alpha = Mathf.Lerp(1, 0, t);
            canvasGroup.alpha = alpha;
            t += .02f + a;
            a += .01f;
            yield return null;
        }
        canvasGroup.alpha = 0;
    }

    public void SelectChoice(UserChoiceNode userChoiceNode)
    {
        if (userChoiceNode == null || userChoiceNode.IsEmpty())
        {
            return;
        }
        UserChoiceNode.FlushInputTerminal();
        userChoiceNode.sub.OnChoiceEntered(EventArgs.Empty);
        currentCount = 0;
        Hide();
    }

    public bool Accept(SubscriberNode sub)
    {
        if (currentCount >= 4)
        {
            return false;
        }

        choicesTexts[currentCount].SetSubscriber(sub);
        currentCount++;
        return true;
    }

    public static void FlushUserChoices()
    {
        foreach(UserChoiceNode ucn in UserChoiceNode.choices)
        {
            ucn.uiText.text = " ";
            ucn.sub = null;
        }
        UserChoiceNode.computerTerminalInput.RepositionBlinkingChar();
    }
}
