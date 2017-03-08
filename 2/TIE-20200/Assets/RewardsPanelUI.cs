using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class RewardsPanelUI : MonoBehaviour {
	public Text victoryText;
	public Image goldIcon;
	public Image expIcon;
	public Text goldText;
	public Text expText;

	public Text[] additionalRewardTexts;

	public Button continueButton;

	void Awake(){
		continueButton.onClick.AddListener (delegate {
			GameManager.instance.ToGameView();
	});

	}
	void OnEnable(){
		Refresh ();
	}
	private void Refresh(){
		for (int i=0; i< additionalRewardTexts.Length; ++i) {
			additionalRewardTexts[i].gameObject.SetActive(false);
		}
		//goldIcon.sprite = GuiManager.instance.GoldIcon;
		//expIcon.sprite = GuiManager.instance.ExpIcon;
		int goldEarned = 0;
		float expEarned = 0;
		for (int i=0; i< CombatManager.instance.player2.losses.units.Count; ++i) {
			if (CombatManager.instance.player2.losses.units[i]!=null){
				goldEarned += (int)(CombatManager.instance.player2.losses.units[i].goldValue*CombatManager.instance.player2.losses.units[i].amount*DataBase.instance.gameData.unitOnKillGoldGainMpl);
				expEarned += CombatManager.instance.player2.losses.units[i].experienceValue*CombatManager.instance.player2.losses.units[i].amount;
			}
		}
		goldText.text = goldEarned.ToString ();
		expText.text = expEarned.ToString ();
		List<Reward> additionalRewards;
		if (GameManager.instance.currentRandomEvent==null){
			additionalRewards = GameManager.instance.currentQuest.GetAdditionalRewards(GameManager.instance.CurrentGame.CurrentTown.baseDifficultyValue);
			if (GameManager.instance.currentQuest.rewardType == QuestReward.NEWUNIT){
				if (!GameManager.instance.CurrentGame.UnlockedUnits.Contains(CombatManager.instance.player2.losses.units[0].id)){
					additionalRewardTexts[0].gameObject.SetActive(true);
					additionalRewardTexts[0].text = "New unit available: " + CombatManager.instance.player2.losses.units[0].name;
					GameManager.instance.CurrentGame.UnlockUnit = CombatManager.instance.player2.losses.units[0].id;
				}
			}
			else if (GameManager.instance.currentQuest.rewardType == QuestReward.NEWTOWN){
				GameManager.instance.CurrentGame.unlockedTown +=1;
				additionalRewardTexts[0].gameObject.SetActive(true);
				additionalRewardTexts[0].text = "New town available: " + GameManager.instance.CurrentGame.towns[GameManager.instance.CurrentGame.unlockedTown].name;
			}
		}else{
			additionalRewards = GameManager.instance.currentRandomEvent.GetAdditionalRewards(GameManager.instance.CurrentGame.CurrentTown.baseDifficultyValue);
		}
		for (int i=0; i< additionalRewards.Count; ++i) {
			switch(additionalRewards[i].type){
			case RewardType.EXP:
				expEarned+=additionalRewards[i].rewardValue;
				break;
			case RewardType.GOLD:
				goldEarned+=additionalRewards[i].rewardValue;
				break;
			case RewardType.ITEM:
				GameManager.instance.CurrentGame.playerTroop.hero.inventory.AddItem(DataBase.instance.GetItem(additionalRewards[i].rewardValue));
				break;
			}
			for (int i2=0; i2< additionalRewardTexts.Length; ++i2) {
				if (!additionalRewardTexts[i2].gameObject.activeSelf){
					additionalRewardTexts[i2].gameObject.SetActive(true);
					additionalRewardTexts[i2].text = additionalRewards[i].RewardText;
					break;
				}
			}
		}
		GameManager.instance.Gold += goldEarned;
		GameManager.instance.CurrentGame.playerTroop.hero.AddExpierence (expEarned);
	}
}
