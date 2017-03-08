using UnityEngine;
using System.Collections;

public enum RewardType{GOLD, EXP, ITEM}
public class Reward{

	public RewardType type;
	public int rewardValue; // amount or id in item's case
	private string rewardText;
	public string RewardText{get{
			switch(type){
			case RewardType.EXP:
				rewardText= rewardValue.ToString() + " Experience";
				break;
			case RewardType.GOLD:
				rewardText= rewardValue.ToString() + " Gold";
				break;
			case RewardType.ITEM:
				rewardText= "New item: " + DataBase.instance.GetItem(rewardValue).name;
				break;
			}
			return rewardText;
		}
	}
}
