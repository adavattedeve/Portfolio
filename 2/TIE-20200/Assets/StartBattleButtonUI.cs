using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StartBattleButtonUI : MonoBehaviour {
	private Button button;

	void Awake(){
		button = GetComponent<Button> ();
		button.onClick.AddListener (delegate {
			GameManager.instance.StartQuest();
	});
		Refresh ();
	}
	void OnEnable(){
		GameManager.OnQuestSelected += Refresh;
	}
	void OnDisable(){
		GameManager.OnQuestSelected -= Refresh;
	}
	public void Refresh(){
		if (GameManager.instance.SelectedQuest >= 0 && GameManager.instance.CurrentGame.CurrentTown.currentQuests.Count > GameManager.instance.SelectedQuest) {
			button.interactable=true;
			return;
		}
		button.interactable = false;
	}
}
