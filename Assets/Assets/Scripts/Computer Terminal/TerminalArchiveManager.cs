using UnityEngine;
using System.Collections;

public class TerminalArchiveManager : MonoBehaviour {

    ComputerTerminal terminal;

	// Use this for initialization
	void Start () {
        terminal = GameObject.Find("computer_spr").GetComponent<ComputerTerminal>();
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            terminal.SkipCurrentSequence();
        }
	}

    public class StringArchive
    {
        public string str;
        private StringArchive next;
        private StringArchive prev;
        private static StringArchive head;
        private static StringArchive tail;

        public StringArchive Next { get { return this.next; } }
        public StringArchive Prev { get { return this.prev; } }
        public static StringArchive Head { get { return head; } }

        private StringArchive(string str)
        {
            this.str = str;
        }

        public static StringArchive Create(string str)
        {
            StringArchive newArchive = new StringArchive(str);
            newArchive.AddToHead();
            return newArchive;
        }

        public bool HasNext() { return this.next != null; }
        public bool HasPrev() { return this.prev != null; }

        private void AddToHead()
        {
            if (head != null)
            {
                head.next = this;
                this.prev = head;
            }
            else
            {
                tail = this;
            }

            head = this;
        }
    }
}
