using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthText : MonoBehaviour {
	Text[] texts;
	CharacterStats stats;
	Health playerHealth;
	private string health;
	private string stability;
	private string betweenMark;
	
	void Awake () {
		texts = GetComponentsInChildren<Text> ();
		health = "health: ";
		stability= "stability: ";
		betweenMark = " / ";
	}
	void Start(){
		GlobalEvents.instance.OnStatChange += RefreshHealth;
		stats = PlayerManager.instance.Player.GetComponent<CharacterStats> ();
		stats.GetComponent<CharacterEvents> ().OnHealthChange += RefreshHealth;
		playerHealth = stats.GetComponent<Health> ();
		RefreshHealth ();
	}
	public void RefreshHealth(){
		texts [0].text = health + playerHealth.CurrentHealth + betweenMark + stats.GetStat (StatType.MAXHEALTH);
		texts [1].text = stability+ playerHealth.CurrentStability + betweenMark + stats.GetStat (StatType.STABILITY);
	}
}
