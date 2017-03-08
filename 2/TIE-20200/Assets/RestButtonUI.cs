using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RestButtonUI : MonoBehaviour {

	private Button restButton;
	void Awake () {
		restButton = GetComponent<Button> ();
		restButton.onClick.AddListener (delegate {
			GameManager.instance.WaitForNextDay ();
		}
		);
	}
}
