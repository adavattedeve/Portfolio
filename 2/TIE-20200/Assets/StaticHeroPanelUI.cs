using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StaticHeroPanelUI : MonoBehaviour {
	StatsPanelUI statsPanel;
	public Text nameText;
	public Image heroIcon;
	public Text experienceText;
	void Awake () {
		statsPanel = GetComponentInChildren<StatsPanelUI> ();
	}
	void Start(){
		Hero hero = GameManager.instance.CurrentGame.playerTroop.hero;
		statsPanel.CreateStats (GameManager.instance.CurrentGame.playerTroop.hero.stats);
		nameText.text =hero.name + ", Lvl: " + hero.level;
		heroIcon.sprite = hero.Icon;
		experienceText.text = "Expierence: " + hero.Experience + "/" + DataBase.instance.gameData.levelExpierenceRequirements [hero.level - 1];
	}
}
