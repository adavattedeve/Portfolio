using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuestInfoUI : MonoBehaviour {
	public Text nameText;
	public Text descriptionText;
	public GameObject contentPanel;
	void Awake(){
		Refresh ();
	}
	void OnEnable(){
		GameManager.OnQuestSelected += Refresh;
	}
	void OnDisable(){
		GameManager.OnQuestSelected -= Refresh;
	}
	public void Refresh(){
		if (GameManager.instance.SelectedQuest >= 0 && GameManager.instance.SelectedQuest < GameManager.instance.CurrentGame.CurrentTown.currentQuests.Count) {
			contentPanel.SetActive (true);
			nameText.text = GameManager.instance.CurrentGame.CurrentTown.currentQuests [GameManager.instance.SelectedQuest].name;
			descriptionText.text = GameManager.instance.CurrentGame.CurrentTown.currentQuests [GameManager.instance.SelectedQuest].descriptionText;
		} else {
			contentPanel.SetActive (false);
		}
	}
}
