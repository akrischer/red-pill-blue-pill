using UnityEngine;
using System.Collections;

public class Utils : MonoBehaviour {

    private static Utils _instance;
    private static Utils Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<Utils>();
            }
            return _instance;
        }
    }

    public static Coroutine StaticStartCoroutine(IEnumerator iEnumerator)
    {
        return Instance.StartCoroutine(iEnumerator);
    }
}
