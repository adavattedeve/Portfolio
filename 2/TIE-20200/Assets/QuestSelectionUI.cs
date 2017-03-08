using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuestSelectionUI : MonoBehaviour {
	public Button[] questButtons;
	private Text[] buttonTexts;
	private int selectedQuest;
	public int SelectQuest{set{
			if (selectedQuest>=0){
				questButtons[selectedQuest].GetComponent<MouseOnButtonAnimation>().Selected=false;
			}
			questButtons[value].GetComponent<MouseOnButtonAnimation>().Selected=true;
			selectedQuest=value;}}
	void Awake () {
		buttonTexts = new Text[questButtons.Length];
		for (int i=0; i< questButtons.Length; ++i){
			buttonTexts[i] = questButtons[i].GetComponentInChildren<Text>();
			int tempInt = i;
			questButtons[i].onClick.AddListener (delegate {
				SelectQuest = tempInt;
				GameManager.instance.SelectedQuest = tempInt;
			});
		}

		Refresh ();
	}
	void OnEnable(){
		GameManager.OnTownChange+=Refresh;
	}
	void OnDisable(){
		GameManager.OnTownChange-=Refresh;
	}
	private void Refresh(){
		for (int i=0; i<questButtons.Length; ++i ){
			if (GameManager.instance.CurrentGame.CurrentTown.currentQuests.Count> i && GameManager.instance.CurrentGame.CurrentTown.currentQuests[i]!=null){
				questButtons[i].gameObject.SetActive(true);
				buttonTexts[i].text = GameManager.instance.CurrentGame.CurrentTown.currentQuests[i].name;
			}else{
				questButtons[i].gameObject.SetActive(false);
			}
		}
	}
}
