using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnitRecruimentUI : MonoBehaviour {
	public Button buyButton;
	public Text costsText;
	private int gold;
	private int goldCost;

	void Awake () {
		buyButton.onClick.AddListener (delegate {
			GameManager.instance.RecruitUnits();
	});
		Refresh ();
	}
	public void OnEnable(){
		GameManager.OnSelectedForRecruitChange += Refresh;
		GameManager.OnGoldChange += Refresh;
	}
	public void OnDisable(){
		GameManager.OnSelectedForRecruitChange -= Refresh;
		GameManager.OnGoldChange -= Refresh;
	}
	public void Refresh(){
		gold = GameManager.instance.Gold;
		goldCost = 0;
		for (int i=0; i<GameManager.instance.SelectedForRecruit.Count; ++i) {
			if (GameManager.instance.SelectedForRecruit[i]>0 && GameManager.instance.CurrentGame.recruitableUnits.units[i]!=null ){
				goldCost+= GameManager.instance.SelectedForRecruit[i]*GameManager.instance.CurrentGame.recruitableUnits.units[i].goldValue;
			}
		}
		costsText.text = gold + "\n-" + goldCost + "\n=" + (gold - goldCost);
		if (gold < goldCost && goldCost>=0) {
			buyButton.interactable = false;
		} else {
			buyButton.interactable = true;
		}
	}
}
