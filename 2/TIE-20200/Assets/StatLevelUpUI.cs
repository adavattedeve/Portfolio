using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatLevelUpUI : MonoBehaviour {
	public Text pointsLeftText;

	public StatType[] statTypes;
	public Button[] levelUpButtons;
	public Image[] statIcons;
	public Text[] statTexts;

	void Awake(){
		for (int i=0; i<statIcons.Length; ++i) {
			statIcons[i].sprite = DataBase.instance.GetSprite(statTypes[i].ToString());
			Refresh (GameManager.instance.CurrentGame.playerTroop.hero.stats.GetStat (statTypes [i]));
			int index=i;
			levelUpButtons[i].onClick.AddListener(delegate {
				LevelUpStat(index);
		});
		}

	}
	void OnEnable(){
		for (int i=0; i<statTypes.Length; ++i) {
			Debug.Log ("adding to event");
			GameManager.instance.CurrentGame.playerTroop.hero.stats.GetStat (statTypes [i]).OnStatChange += Refresh;
		}
	}
	void OnDisable(){
		for (int i=0; i<statTypes.Length; ++i) {
			GameManager.instance.CurrentGame.playerTroop.hero.stats.GetStat (statTypes [i]).OnStatChange -= Refresh;
		}
	}
	public void Refresh(Stat stat){
		bool isPointsLeft=false;
		int pointsLeft = GameManager.instance.CurrentGame.playerTroop.hero.attributePoints;
		int value = stat.Value;
		for (int i=0; i<statTypes.Length; ++i) {
			if (stat.type == statTypes[i]){
				statTexts [i].text = value + "=>" + (value + 1);
			}
		}
		if (pointsLeft > 0) {
			isPointsLeft=true;
		}
		for (int i=0; i<statTypes.Length; ++i) {
			levelUpButtons[i].interactable=isPointsLeft;
		}
		pointsLeftText.text = pointsLeft.ToString();
	}
	public void LevelUpStat(int index){
		GameManager.instance.CurrentGame.playerTroop.hero.LevelUpAttribute (statTypes[index]);
	}
}
