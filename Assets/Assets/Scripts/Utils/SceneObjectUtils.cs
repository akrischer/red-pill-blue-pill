using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SceneObjectUtils {

	public static Text GetUIText(GameObject gameObject)
    {
        string uiTextName = gameObject.name + "_text";
        GameObject foundObject = GameObject.Find(uiTextName);
        if (foundObject == null)
        {
            Debug.LogError("No game object found with name '" + uiTextName + "'");
            return null;
        }
        else
        {
            return foundObject.GetComponent<Text>();
        }
    }
}
