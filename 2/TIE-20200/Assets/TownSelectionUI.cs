using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TownSelectionUI : MonoBehaviour {
	public Button[] townButtons;
	private Text[] buttonTexts;
	public Button travelButton;

	public int selected=0;
	void Awake(){
		buttonTexts=new Text[townButtons.Length];
		for (int i=0; i<townButtons.Length; ++i) {
			int tempInt = i;
			townButtons[i].onClick.AddListener(delegate {
				selected = tempInt;
		});
			travelButton.onClick.AddListener(delegate {
				GameManager.instance.TravelToTown(selected);
				gameObject.SetActive(false);
		});
			buttonTexts[i] = townButtons[i].GetComponentInChildren<Text>();
		}
		Refresh ();
	}

	void Refresh(){
		for (int i=0; i<townButtons.Length; ++i ){
			if (GameManager.instance.CurrentGame.towns.Length> i && GameManager.instance.CurrentGame.towns[i]!=null && GameManager.instance.CurrentGame.unlockedTown>=i){
				townButtons[i].gameObject.SetActive(true);
				buttonTexts[i].text = GameManager.instance.CurrentGame.towns[i].name;
			}else{
				townButtons[i].gameObject.SetActive(false);
			}
		}
	}
}
