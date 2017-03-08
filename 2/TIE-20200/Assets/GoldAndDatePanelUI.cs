using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GoldAndDatePanelUI : MonoBehaviour {
	public Text date;
	public Text gold;

	void Awake(){
		Refresh();
	}
	void OnEnable(){
		GameManager.OnDayChange += Refresh;
		GameManager.OnGoldChange += Refresh;
	}
	void OnDisable(){
		GameManager.OnDayChange -= Refresh;
		GameManager.OnGoldChange -= Refresh;
	}
	private void Refresh(){
		date.text = "MONTH: " + GameManager.instance.CurrentGame.month + "\nWEEK: " + GameManager.instance.CurrentGame.week + "\nDAY: " + GameManager.instance.CurrentGame.day;
		gold.text = "GOLD: " + GameManager.instance.CurrentGame.gold;
	}
}
