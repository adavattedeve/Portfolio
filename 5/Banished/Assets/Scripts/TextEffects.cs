using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextEffects : MonoBehaviour {
    private Text text;

	void Start () {
        text = GetComponent<Text>();
        text.color = UIManager.instance.otherTextColor;
        text.font = UIManager.instance.otherTextFont;
    }
	
}
