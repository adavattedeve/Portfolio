using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum GameStatus{MAINMENU, GAMEVIEW, BATTLE}
public class GameManager : MonoBehaviour {
	
	private GameState currentGame;
	public GameState CurrentGame{
		get{return currentGame;}
		set{
//			if (OnUnlockedUnitsChange==null){
//				OnUnlockedUnitsChange += RefreshRecruitableUnits;
//			}
			if (OnCurrentGameChange!=null){
				OnCurrentGameChange(value);
			}
			currentGame=value;
		}
	}
	public GameStatus state=GameStatus.MAINMENU;
	public int currenGameIndex=-1;
	private int selectedQuest=-1;
	public int SelectedQuest{
		get{return selectedQuest;}
		set{
			selectedQuest = value;
			if (OnQuestSelected!=null){
				OnQuestSelected();
			}
			Debug.Log ("quest selected " + value);
			}
	}

	[System.NonSerialized]public Quest currentQuest;
	[System.NonSerialized]public RandomQuestEvent currentRandomEvent;

	public int Gold{get{
			return currentGame.gold;
		}set{
			currentGame.gold=value;
			if (OnGoldChange!=null){
				OnGoldChange();
			}
		}}

	public delegate void GoldChangeAction();
	public static event GoldChangeAction OnGoldChange;

	public delegate void CurrenGameChangeAction(GameState newState);
	public static event CurrenGameChangeAction OnCurrentGameChange;

	public delegate void DayChangeAction();
	public static event DayChangeAction OnDayChange;

	public delegate void WeekChangeAction ();
	public static event WeekChangeAction OnWeekChange;

	public delegate void MonthChangeAction ();
	public static event MonthChangeAction OnMonthChange;

	public delegate void QuestSelectedAction ();
	public static event QuestSelectedAction OnQuestSelected;

	public delegate void TownChangeAction ();
	public static event TownChangeAction OnTownChange;

	public delegate void SelectedForRecruitChangeAction ();
	public static event SelectedForRecruitChangeAction OnSelectedForRecruitChange;

	public delegate void UnitsBoughtAction ();
	public static event UnitsBoughtAction OnUnitsBought;

//	public delegate void UnlockedUnitsChangeAction ();
//	public static event UnlockedUnitsChangeAction OnUnlockedUnitsChange;
//	public void LaunchOnUnlockedUnitsChange(){
//		if (OnUnlockedUnitsChange != null) {
//			OnUnlockedUnitsChange ();
//		}}

	
	private List<int> selectedForRecruit; // unit amounts
	public  List<int> SelectedForRecruit{
		get{return selectedForRecruit;}
	}

	// just for testing!!!
	public GameState[] savedGames;
	public static GameManager instance;

	public static int HEROES_NAME_MIN_CHAR=3;

	public int gameSceneIndex=0;
	public int mainMenuSceneIndex=1;
	public int battleSceneIndex=2;

	void Awake () {
		if (!instance) {
			instance = this;
			DontDestroyOnLoad (gameObject);
		} else {
			Destroy (gameObject);
			return;
		}
		OnCurrentGameChange += SetPlayerEvents;
		SaveLoad.LoadSaveGames ();
		savedGames = SaveLoad.savedGames.ToArray();
		selectedForRecruit = new List<int> ();
		OnDayChange += AddDailyRecruitableUnits ;
		state = GameStatus.MAINMENU;
	}
	private void SetPlayerEvents(GameState newGameState){
		if (currentGame != null) {
			OnDayChange -= currentGame.playerTroop.hero.RegenMana;
		}
		if (newGameState != null) {
			OnDayChange += newGameState.playerTroop.hero.RegenMana;
		}
	}
	public void CreateNewSave(Troop troop){
		CurrentGame = new GameState (troop);
		SaveLoad.Save ();
		savedGames = SaveLoad.savedGames.ToArray();
		Application.LoadLevel (mainMenuSceneIndex);
	}
	public void LoadGame(int index){
		SaveLoad.LoadGame (index);
		Application.LoadLevel (mainMenuSceneIndex);
	}
	public void ToMainMenu(){
		state = GameStatus.MAINMENU;
		SaveLoad.Save (currenGameIndex);
		Application.LoadLevel (mainMenuSceneIndex);
	}
	public void ToGameView(){
		if (currentGame != null) {

			Application.LoadLevel (gameSceneIndex);
			state = GameStatus.GAMEVIEW;
		} else {
			Debug.Log ("currentGameIsNull");
		}

	}

	public void WaitForNextDay(){

		if (currentGame.day == 7) {
			if (currentGame.week == 4) {
				currentGame.day = 1;
				currentGame.week = 1;
				currentGame.month++;
				if (OnMonthChange != null) {
					OnMonthChange ();
				}
			} else {
				currentGame.day = 1;
				currentGame.week++;
			}
			if (OnWeekChange != null) {
				OnWeekChange ();
			}
		}
		else {
			currentGame.day++;
		}
		if (OnDayChange != null) {
			OnDayChange();
		}

	}
	public void StartQuest(){
		currentQuest = currentGame.CurrentTown.currentQuests [selectedQuest];
		float random = Random.Range (0f,1f);
		if (random <= currentGame.CurrentTown.currentQuests [selectedQuest].randomEventChances) {
			currentRandomEvent = currentGame.CurrentTown.GetRandomEvent();
			GuiManager.instance.ShowRandomEvent ();
			return;
		}
		ContinueQuest ();
	}

	void OnLevelWasLoaded(int levelIndex){
		if (levelIndex == battleSceneIndex) {
			Troop enemyTroop;
			if (currentRandomEvent == null) {
				enemyTroop = currentQuest.troop;
			} else {
				enemyTroop = currentRandomEvent.troop;
			}
			CombatManager.instance.StartBattle (currentGame.playerTroop, Control.INPUT, enemyTroop, Control.AI);
		} else if (levelIndex == gameSceneIndex) {
			List<Unit> playerUnits = CurrentGame.playerTroop.units;
			bool unitsLeft = false;
			for (int i=0; i<playerUnits.Count; ++i){
				if (playerUnits[i]!=null){
					unitsLeft=true;
					break;
				}
			}
			if (!unitsLeft){
				SaveLoad.DeleteGame(currenGameIndex);
				CurrentGame = null;
				Application.LoadLevel(mainMenuSceneIndex);
			}else {
				if (currentQuest != null) {
					GuiManager.instance.ShowRandomEvent ();
				}
				RefreshRecruitableUnits ();
			}
		} 
	}
	public void ContinueQuest(){
		state = GameStatus.BATTLE;
		Application.LoadLevel (battleSceneIndex);
	}
	public void CancelQuest(){
		SelectedQuest = -1;
			currentRandomEvent = null;
			currentQuest=null;
		WaitForNextDay ();
	}
	public void EndQuest(){
		GuiManager.instance.ShowQuestReward ();
		if (currentRandomEvent != null) {
			currentRandomEvent = null;
		} else {
			currentGame.CurrentTown.RemoveQuest (selectedQuest);
			currentQuest=null;
			SelectedQuest = -1;
		}
		WaitForNextDay ();
	}
	public void StartRandomEvent(){
		state = GameStatus.BATTLE;
		Application.LoadLevel (battleSceneIndex);
	}
	public void TravelToTown(int index){
		currentGame.currentTownIndex = index;
		if (OnTownChange != null) {
			OnTownChange();
		}
	}
	public void SelectToRecruit(int index, int amount){
		while (selectedForRecruit.Count<=index) {
			selectedForRecruit.Add (0);
		}
		selectedForRecruit [index] = amount;
		if (OnSelectedForRecruitChange!=null){
			OnSelectedForRecruitChange();
		}
	}

	public void SellItem(int index, ItemType type){
		Gold += (int)(currentGame.playerTroop.hero.inventory.GetItem(index, type).goldValue*DataBase.instance.gameData.itemOnSellGoldGainMpl);
		currentGame.playerTroop.hero.inventory.DeleteItem (index, type);
	}
	public void BuyItem(int index, ItemType type){
		currentGame.playerTroop.hero.inventory.AddItem (currentGame.shopItems.GetItem(index, type));
		Gold -= currentGame.shopItems.GetItem(index, type).goldValue;
	}

	public void RecruitUnits(){

		int goldCost = 0;
		for (int i=0; i<SelectedForRecruit.Count; ++i) {
			if (SelectedForRecruit[i]>0 && CurrentGame.recruitableUnits.units[i]!=null ){
				goldCost+= SelectedForRecruit[i]*CurrentGame.recruitableUnits.units[i].goldValue;
				currentGame.barracksUnits.AddUnits(CurrentGame.recruitableUnits.units[i], SelectedForRecruit[i]);
				currentGame.recruitableUnits.AddUnits(CurrentGame.recruitableUnits.units[i], -SelectedForRecruit[i], -1, false);
			}
		}
	
		Gold -= goldCost;
		if (OnUnitsBought != null) {
			OnUnitsBought();
		}
	}
	public void AddDailyRecruitableUnits(){
		Debug.Log ("addingDaily units");
		for (int i=0; i<currentGame.recruitableUnits.units.Count; ++i) {
			currentGame.recruitableUnits.AddUnits(currentGame.recruitableUnits.units[i], (int)(currentGame.GetPopulateMpl(currentGame.recruitableUnits.units[i].id)*currentGame.recruitableUnits.units[i].basePopulateValue));
		}
		for (int i=0; i<currentGame.towns.Length; ++i) {
			currentGame.towns[i].DayUpdateUnitAmounts(currentGame.month, currentGame.week, currentGame.day);
		}
	}
	public void RefreshRecruitableUnits(){
		bool foundUnit = false;
		for (int i=0; i<currentGame.UnlockedUnits.Count; ++i) {
			foundUnit = false;
			for (int i2=0; i2<currentGame.recruitableUnits.units.Count; ++i2) {
				if (currentGame.UnlockedUnits[i] == currentGame.recruitableUnits.units[i2].id){
					Debug.Log ("Found unit from recruitable with id: " + currentGame.recruitableUnits.units[i2].id);
					foundUnit = true;
					break;
				}
			}
			if (!foundUnit){
				Unit newUnit = DataBase.instance.GetUnit(currentGame.UnlockedUnits[i]);
				currentGame.recruitableUnits.AddUnits(newUnit, (int)(currentGame.GetPopulateMpl(newUnit.id)*newUnit.basePopulateValue), -1, false);
			}
		}

	}
	public void TransferUnits(TroopLocation from, int fromIndex, TroopLocation to, int toIndex, int amount=-1){
		Unit tempUnit;
		switch(from){
		case TroopLocation.BARRACKS:
			if (amount==-1){
				amount = CurrentGame.barracksUnits.units[fromIndex].amount;
			}
			switch(to){
			case TroopLocation.BARRACKS:
				if (fromIndex==toIndex){
					return;
				}
				tempUnit = currentGame.barracksUnits.AddUnits(CurrentGame.barracksUnits.units[fromIndex], amount, toIndex);
				currentGame.barracksUnits.AddUnits(CurrentGame.barracksUnits.units[fromIndex], -amount, fromIndex);
				if (tempUnit!=null){
					currentGame.barracksUnits.AddUnits(tempUnit,tempUnit.amount, fromIndex);
				}
				return;
			case TroopLocation.PLAYER:
				tempUnit = currentGame.playerTroop.AddUnits(CurrentGame.barracksUnits.units[fromIndex], amount, toIndex);
				currentGame.barracksUnits.AddUnits(CurrentGame.barracksUnits.units[fromIndex], -amount, fromIndex);
				if (tempUnit!=null){
					currentGame.barracksUnits.AddUnits(tempUnit,tempUnit.amount, fromIndex);
				}
				return;
			default:
				return;
			}
		case TroopLocation.RECRUITABLE:
			if (amount==-1){
				amount = CurrentGame.recruitableUnits.units[fromIndex].amount;
			}
			switch(to){
			case TroopLocation.BARRACKS:
				currentGame.barracksUnits.AddUnits(CurrentGame.recruitableUnits.units[fromIndex], amount, toIndex);
				currentGame.recruitableUnits.AddUnits(CurrentGame.recruitableUnits.units[fromIndex], -amount, fromIndex);
				return;
			default:
				return;
			}
		case TroopLocation.PLAYER:
			if (amount==-1){
				amount = CurrentGame.playerTroop.units[fromIndex].amount;
			}
			switch(to){
			case TroopLocation.BARRACKS:
				tempUnit = currentGame.barracksUnits.AddUnits(CurrentGame.playerTroop.units[fromIndex], amount, toIndex);
				currentGame.playerTroop.AddUnits(CurrentGame.playerTroop.units[fromIndex], -amount, fromIndex);
				if (tempUnit!=null){
					currentGame.playerTroop.AddUnits(tempUnit,tempUnit.amount, fromIndex);
				}
				return;
			case TroopLocation.PLAYER:
				tempUnit = currentGame.playerTroop.AddUnits(CurrentGame.playerTroop.units[fromIndex], amount, toIndex);
				currentGame.playerTroop.AddUnits(CurrentGame.playerTroop.units[fromIndex], -amount, fromIndex);
				if (tempUnit!=null){
					currentGame.playerTroop.AddUnits(tempUnit,tempUnit.amount, fromIndex);
				}
				return;
			default:
				return;
			}
		default:
			return;
		}
	}
	public void ExitGame(){
		Application.Quit ();
	}
}
