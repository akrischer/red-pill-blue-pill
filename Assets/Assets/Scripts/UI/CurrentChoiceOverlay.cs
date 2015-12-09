using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CurrentChoiceOverlay : MonoBehaviour {

    public float transitionTime = 1f;

    public Sprite normalSprite;
    public Sprite invertedSprite;
    private Image image;

	// Use this for initialization
	void Start () {
        image = gameObject.GetComponent<Image>();
        StartCoroutine(Blink());
	}

    void Update()
    {
        UserChoicesUnderlay.UserChoiceNode currentSelectedNode = UserChoicesUnderlay.UserChoiceNode.CurrentUserChoiceNode;
        if (currentSelectedNode != null)
        {
            transform.position = currentSelectedNode.uiText.transform.position;
        }
    }
	
	public IEnumerator Blink()
    {
        yield return new WaitForSeconds(transitionTime);
        if (image.sprite == normalSprite)
        {
            image.sprite = invertedSprite;
        }
        else
        {
            image.sprite = normalSprite;
        }
        StartCoroutine(Blink());
    }
}
