using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UserOptionsSpriteText : SpriteText {

    public const string FONT_NAME = "userfontsheet_load";
    public const int CHAR_SIZE = 12;
    private static Dictionary<string, Sprite> _sprites = new Dictionary<string, Sprite>();
    private static Dictionary<char, int> _characterIndexes = new Dictionary<char, int>()
    {
        {'A', 0},
        {'B', 1},
        {'C', 2},
        {'D', 3},
        {'E', 4},
        {'F', 5},
        {'G', 6},
        {'H', 7},
        {'I', 8},
        {'J', 9},
        {'K', 10},
        {'L', 11},
        {'M', 12},
        {'N', 13},
        {'O', 14},
        {'P', 15},
        {'Q', 16},
        {'R', 17},
        {'S', 18},
        {'T', 19},
        {'U', 20},
        {'V', 21},
        {'W', 22},
        {'X', 23},
        {'Y', 24},
        {'Z', 25},
        {' ', 26},
        {'0', 27},
        {'1', 28},
    };

    protected static bool spritesInitialized = false;

    private UserOptionsSpriteText() : base(FONT_NAME, _sprites, _characterIndexes, 0, CHAR_SIZE) { }

    public static SpriteText create(string text)
    {
        if (!spritesInitialized) initSprites();
        UserOptionsSpriteText userOptionsSpriteText = new UserOptionsSpriteText();
        userOptionsSpriteText.SetText(text);
        return userOptionsSpriteText;
    }

    private static void initSprites()
    {
        spritesInitialized = true;
        Sprite[] spriteList = Resources.LoadAll<Sprite>(FONT_NAME);

        for (int i = 0; i < spriteList.Length; i++)
        {
            if (!_sprites.ContainsKey(spriteList[i].name))
            {
                _sprites.Add(spriteList[i].name, spriteList[i]);
            }
        }
    }
}
