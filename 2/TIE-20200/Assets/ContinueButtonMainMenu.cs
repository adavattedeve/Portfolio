using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ContinueButtonMainMenu : MonoBehaviour {

	void Start(){
		GetComponent<Button> ().onClick.AddListener (delegate {
			GameManager.instance.ToGameView();
		});

		RefreshInteractable(GameManager.instance.CurrentGame);
	}
	void OnEnable(){
		GameManager.OnCurrentGameChange += RefreshInteractable ;
	}
	void OnDisable(){
		GameManager.OnCurrentGameChange -= RefreshInteractable ;
	}
	public void RefreshInteractable(GameState newGameState){
		if (newGameState != null) {
			GetComponent<Button> ().interactable = true;
		} else {
			GetComponent<Button> ().interactable = false;
		}
	}
}
