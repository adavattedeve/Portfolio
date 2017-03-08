using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class TownData : ScriptableObject {
	public List<Town> towns;
	public List<QuestData> townQuests;

	public Town[] GetTowns(){
		for (int i=0; i<towns.Count; ++i) {
			towns[i].quests = townQuests[i].GetQuests();
			towns[i].randomEvents = townQuests[i].GetRandomEvents();
		}
		return towns.ToArray();
	}
}
