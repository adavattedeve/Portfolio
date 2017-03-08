using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameState {
	public int day;
	public int week;
	public int month;
	public int gold;

	private List<int> unlockedUnits;
	public List<int> UnlockedUnits{get{return unlockedUnits;}}
	private List<float> recruitablePopulateMpl;
	public int UnlockUnit{set{
			Debug.Log ("unlocking unit, id: " + value.ToString());
			if (!unlockedUnits.Contains(value)){
				Debug.Log ("unlock succesful");
				unlockedUnits.Add(value);
				recruitablePopulateMpl.Add (1);
			}
		}}

	public int unlockedTown=0;

	public Troop playerTroop, recruitableUnits, barracksUnits;
	public Town[] towns;
	public int currentTownIndex;
	public Town CurrentTown{get {return towns[currentTownIndex];}}
	public ItemList shopItems;
	public GameState(Troop troop){
		day = 1;
		week = 1;
		month = 1;
		unlockedTown = 0;
		gold = DataBase.instance.gameData.startingGold;
		playerTroop = troop;
		towns = DataBase.instance.GetTowns ();
		currentTownIndex = 0;
		recruitableUnits = new Troop();
		unlockedUnits = new List<int> ();
		recruitablePopulateMpl = new List<float> ();
		UnlockUnit = 0;
		for (int i=0; i<unlockedUnits.Count; ++i){
			recruitableUnits.units.Add(DataBase.instance.GetUnit(unlockedUnits[i]));
			recruitableUnits.units[i].amount = (int)recruitableUnits.units[i].basePopulateValue;
		}
		shopItems = new ItemList ();
		for (int i=0; i<DataBase.instance.gameData.shopItemIds.Count; ++i) {
			shopItems.AddItem(DataBase.instance.GetItem(DataBase.instance.gameData.shopItemIds[i]));
		}
		barracksUnits  = new Troop();
	}
	public GameState(){

	}
	public void AddToPopulateMpl (int unitID, float addedValue) {
		for (int i=0; i<unlockedUnits.Count; ++i) {
			if (unlockedUnits[i]==unitID){
				recruitablePopulateMpl[i]+=addedValue;
			}
		}
	}
	public float GetPopulateMpl(int unitID){
		for (int i=0; i<unlockedUnits.Count; ++i) {
			if (unlockedUnits[i]==unitID){
				return recruitablePopulateMpl[i];
			}
		}
		return 1;
	}
	public void PrepareForSaving(){
		playerTroop.PrepareForSaving ();
		for (int i=0; i<towns.Length; ++i) {
			if (towns[i]!=null){
				towns[i].PrepareForSaving();
			}
		}
		recruitableUnits.PrepareForSaving();
		barracksUnits.PrepareForSaving();
		shopItems.PrepareForSaving ();
		
	}
	public void FinishLoading(){
		playerTroop.FinishLoading ();
		for (int i=0; i<towns.Length; ++i) {
			if (towns[i]!=null){
				towns[i].FinishLoading();
			}
		}
		recruitableUnits.FinishLoading();
		barracksUnits.FinishLoading();
		shopItems.FinishLoading ();
	}
}
