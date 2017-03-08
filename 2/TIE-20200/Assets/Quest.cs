using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Quest : Battle {

	public float randomEventChances=0.15f;
	public Quest nextQuest;
	public Quest GetDublicate(){
		Quest returnQuest = new Quest ();
		returnQuest.minValue = minValue;
		returnQuest.maxValue = maxValue;
		returnQuest.possibleUnitIds = possibleUnitIds;
		returnQuest.randomEventChances = randomEventChances;
		returnQuest.name = name;
		returnQuest.rewardType = rewardType;
		returnQuest.descriptionText = descriptionText;
		if (nextQuest != null) {
			returnQuest.nextQuest = nextQuest.GetDublicate ();
		}
		return returnQuest;
	}
}
