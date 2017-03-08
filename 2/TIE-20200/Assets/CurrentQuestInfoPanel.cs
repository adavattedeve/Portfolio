using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CurrentQuestInfoPanel : MonoBehaviour {
	public Text nameText;
	public Text descriptionText;
	void OnEnable(){
		Refresh ();
	}
	public void Refresh(){
		if (GameManager.instance.currentRandomEvent == null) {
			nameText.text = GameManager.instance.currentQuest.name;
			descriptionText.text = GameManager.instance.currentQuest.descriptionText;
		}
		else {
			nameText.text = GameManager.instance.currentRandomEvent.name;
			descriptionText.text = GameManager.instance.currentRandomEvent.descriptionText;
		}
	}

}
