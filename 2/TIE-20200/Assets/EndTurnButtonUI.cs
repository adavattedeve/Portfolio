using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndTurnButtonUI : MonoBehaviour {
	private Button button;
	private CombatManager.Player currentPlayer;
	void Awake(){
		button = GetComponent<Button> ();
	}
	void Start(){
		button.onClick.AddListener (delegate {
			CombatManager.instance.NextTurn();
	});
	}
	void OnEnable(){
		CombatManager.instance.OnTurnChange += Refresh;
	}
	void OnDisable(){
		CombatManager.instance.OnTurnChange -= Refresh;
	}
	void Update(){
			if (CombatManager.instance.TurnIsAbleToEnd () && currentPlayer.control==Control.INPUT) {
				button.interactable = true;
			} else {
				button.interactable = false;
			}
	}
	void Refresh(CombatManager.Player player){
		currentPlayer = player;
		if (player.control == Control.INPUT) {
			button.interactable = true;
		} else {
			button.interactable = false;
		}
	}
}
