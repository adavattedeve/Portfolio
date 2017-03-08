using UnityEngine;
using System.Collections;
[System.Serializable]
public enum RandomEventType{OPTIONAL, AGGRESSIVE};
[System.Serializable]
public class RandomQuestEvent : Battle {
	public RandomEventType type;
	public RandomQuestEvent GetDublicate(){
		RandomQuestEvent randomEvent = new RandomQuestEvent();
		randomEvent.descriptionText = descriptionText;
		randomEvent.name = name;
		randomEvent.rewardType = rewardType;
		randomEvent.possibleUnitIds = possibleUnitIds;
		randomEvent.minValue = minValue;
		randomEvent.maxValue = maxValue;
		return randomEvent;
	}
}
