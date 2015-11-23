using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class SpriteText
{
    protected string spriteFontName;
    protected Dictionary<string, Sprite> sprites;
    protected List<GameObject> characters = new List<GameObject>();
    protected GameObject gameObject;
    protected Dictionary<char, int> characterIndexes;
    protected int stringLength = 0;
    protected int defaultCharacterIndex;
    protected float charSize;
    private string text;

    public Transform Transform
    {
        get
        {
            return gameObject.transform;
        }
    }

    protected SpriteText(string spriteFontName, Dictionary<string, Sprite> sprites, Dictionary<char, int> characterIndexes, int defaultCharacterIndex, float charSize, string text = null)
    {
        gameObject = new GameObject("sprite text (" + spriteFontName + "): " + text);

        this.spriteFontName = spriteFontName;

        this.sprites = sprites;

        this.characterIndexes = characterIndexes;

        this.defaultCharacterIndex = defaultCharacterIndex;

        this.charSize = charSize;

        if (text != null && text != string.Empty)
        {
            SetText(text);
        }
        else
        {
            stringLength = 0;
        }
    }

    public void SetText(string text)
    {
        // make all text uppercase!
        text = text.ToUpper();
        this.text = text;

        //save old scale, position and rotation
        Vector3 oldScale = gameObject.transform.localScale;
        gameObject.transform.localScale = Vector3.one;

        Vector3 oldPosition = gameObject.transform.position;
        gameObject.transform.position = Vector3.zero;

        Quaternion oldRotation = gameObject.transform.rotation;
        gameObject.transform.rotation = Quaternion.identity;

        //delete old game objects
        foreach (GameObject obby in characters)
        {
            GameObject.Destroy(obby);
        }

        //reset string length
        stringLength = 0;

        string spriteName = null;
        GameObject charObj = null;
        SpriteRenderer spriteRenderer;
        bool charFound;

        float currentX = 0;

        characters = new List<GameObject>();

        if (text != null && text != string.Empty)
        {
            //loop through characters and create game objects
            foreach (char charry in text)
            {
                charFound = false;
                if (characterIndexes.ContainsKey(charry))
                {
                    spriteName = string.Format("{0}_{1}", spriteFontName, characterIndexes[charry]);
                    if (sprites.ContainsKey(spriteName))
                    {

                        charObj = new GameObject("char: " + charry);

                        charFound = true;
                    }
                }

                //use default character if characters not found
                if (!charFound)
                {
                    spriteName = string.Format("{0}_{1}", spriteFontName, defaultCharacterIndex);
                    if (sprites.ContainsKey(spriteName))
                    {
                        charObj = new GameObject("default char: " + charry);

                        charFound = true;
                    }
                }

                //if we have found a character, create all the stuff for the sprite 
                if (charFound && charObj != null && spriteName != null)
                {
                    charObj.transform.parent = gameObject.transform;
                    charObj.transform.position = new Vector3(currentX, 0, 0);
                    spriteRenderer = charObj.AddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = sprites[spriteName];
                    currentX += charSize;
                    characters.Add(charObj);
                    stringLength++;
                }
            }
        }

        //reset scale, position and rotation
        gameObject.transform.localScale = oldScale;
        gameObject.transform.position = oldPosition;
        gameObject.transform.rotation = oldRotation;
    }
}