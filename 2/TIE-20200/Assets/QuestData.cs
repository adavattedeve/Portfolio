using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class QuestData: ScriptableObject {
	[SerializeField]private List<Quest> quests;
	[SerializeField]private List<RandomQuestEvent> randomEvents;

	public List<Quest> GetQuests(){
		return quests;
	}
	public List<RandomQuestEvent> GetRandomEvents(){
		return randomEvents;
	}
}
