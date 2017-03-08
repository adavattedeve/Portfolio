using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatTexts : MonoBehaviour {

	Text[] texts;
	CharacterStats stats;
	private string damage;
	private string impact;
	private string armor;
	private string stability;
	private string betweenMark;

	void Awake () {
		texts = GetComponentsInChildren<Text> ();
		damage = "Damage: ";
		impact= "Impact: ";
		armor="Armor: ";
		stability="Stability: ";
		betweenMark = " - ";
	}
	void OnEnable(){
		if (GlobalEvents.instance != null) {
			GlobalEvents.instance.OnStatChange += RefreshStats;
		}
		if (stats == null && PlayerManager.instance!=null) {
			stats = PlayerManager.instance.Player.GetComponent<CharacterStats> ();
		}
		if (stats != null) {
			RefreshStats ();
		}

	}
	void OnDisable(){
		GlobalEvents.instance.OnStatChange -= RefreshStats;
	}
	public void RefreshStats(){
		texts [0].text = damage + stats.GetStat (StatType.DAMAGEMIN) + betweenMark + stats.GetStat (StatType.DAMAGEMAX);
		texts [1].text = impact + stats.GetStat (StatType.IMPACT);
		texts [2].text =  armor+ stats.GetStat (StatType.ARMOR);
		texts [3].text = stability + stats.GetStat (StatType.STABILITY);
	}
}
