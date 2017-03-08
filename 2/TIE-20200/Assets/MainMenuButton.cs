using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuButton : MonoBehaviour {

	void Awake(){
		GetComponent<Button> ().onClick.AddListener (delegate {
			GameManager.instance.ToMainMenu();
	});
	}
}
