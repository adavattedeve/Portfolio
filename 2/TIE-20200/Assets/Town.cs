using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class Town {
	public string name;
	public List<Quest> quests;
	public List<RandomQuestEvent> randomEvents;
	public int baseDifficultyValue;
	public float dailyDifficultyMpl=1.2f;
	public float weeklyDifficultyMpl=1.8f;
	public int questAmount = 2;
	public List<Quest> currentQuests;
	public Town(){
		quests = new List<Quest> ();
		currentQuests = new List<Quest> ();
		randomEvents = new List<RandomQuestEvent>();
	}

	public RandomQuestEvent GetRandomEvent(){
		int random = Random.Range (0, randomEvents.Count);
		RandomQuestEvent temp = randomEvents [random];
		temp.CalculateTroops (baseDifficultyValue);
		return temp;
	}
	public void RemoveQuest(int index){
		Debug.Log ("removing at index: " + index + " curren tQuests.Length: " + currentQuests.Count);
		if (currentQuests [index].rewardType == QuestReward.NEWUNIT) {
			Quest temp = GetRandomQuest (QuestReward.UNITPOPULATION);
			if (temp ==null){
				currentQuests.RemoveAt(index);
			}else{
				currentQuests [index] = temp;
			}
		} else if (currentQuests [index].rewardType == QuestReward.NORMAL) {
			currentQuests.RemoveAt(index);
			currentQuests.Add (GetRandomQuest (QuestReward.NORMAL));
		}
		else {
			currentQuests.RemoveAt(index);
		}
	}
	private Quest GetRandomQuest(QuestReward type){
		List<string> usedQuests= new List<string>();
		for (int i=0; i<currentQuests.Count; ++i) {
			usedQuests.Add(currentQuests[i].name);
		}

		List<Quest> validQuests = new List<Quest> ();
		for (int i=0; i<quests.Count; ++i) {

			if (quests[i].rewardType == type && !usedQuests.Contains(quests[i].name)){
				validQuests.Add(quests[i]);
			}
		}
		if (validQuests.Count == 0) {
			Debug.Log ("no valid quests. type: " + type.ToString());
			return null;
		}
		int random  = Random.Range(0, validQuests.Count);
		Quest quest = validQuests [random].GetDublicate();
		quest.CalculateTroops (baseDifficultyValue);
		return quest;
	}
	public Town GetDublicate(){
		Town town = new Town ();
		town.name = name;
		town.baseDifficultyValue = baseDifficultyValue;
		town.questAmount = questAmount;
		for (int i=0; i<randomEvents.Count; ++i) {
			town.randomEvents.Add(randomEvents[i]);
		}
		for (int i=0; i<quests.Count; ++i) {
			town.quests.Add(quests[i]);
		}
		for (int i=0; i<questAmount; ++i) {
			town.currentQuests.Add (GetRandomQuest (QuestReward.NORMAL));
		}
		Quest temp = GetRandomQuest (QuestReward.NEWUNIT);
		if (temp != null) {
			town.currentQuests.Add (temp);
		}
		temp = GetRandomQuest (QuestReward.NEWTOWN);
		if (temp != null) {
			town.currentQuests.Add (temp);
		}

		return town;
	}
	public void PrepareForSaving(){
		for (int i=0; i<currentQuests.Count; ++i) {
			currentQuests[i].troop.PrepareForSaving();
		}
	}
	public void DayUpdateUnitAmounts(int month, int week, int day){
		float mpl;
		if (day == 1) {
			mpl = weeklyDifficultyMpl;
		} else {
			mpl = dailyDifficultyMpl;
		}
		baseDifficultyValue = (int)(mpl * baseDifficultyValue);
		for (int i=0; i<currentQuests.Count; ++i) {
			currentQuests[i].UnitGrowth(mpl);
		}
		for (int i=0; i<randomEvents.Count; ++i) {
			randomEvents[i].UnitGrowth(mpl);
		}
		Debug.Log ("more units day");
	}
	public void FinishLoading(){
		for (int i=0; i<currentQuests.Count; ++i) {
			Debug.Log ("finish loading quest");
			currentQuests[i].troop.FinishLoading();
		}
	}
}
