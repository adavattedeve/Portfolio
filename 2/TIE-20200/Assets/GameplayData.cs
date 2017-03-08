using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameplayData : ScriptableObject {
	public int startingGold;
	public float offenceDefenceMpl=0.025f;
	public float goldToExperienceMpl=0.5f;
	public float unitOnKillGoldGainMpl;
	public float itemOnSellGoldGainMpl;
	public int rangePenaltyRange=5;
	public float rangePenaltyMpl=0.5f;
	public GameObject defaultTilePrefab;
	public GameObject visualizationTilePrefab;
	public Color visualizationTileGreen;
	public Color visualizationTileYellow;
	public Color visualizationTileRed;
	public float[] levelExpierenceRequirements;
	public List<int> shopItemIds;
	public List<int> startingTroopHeroIds;
	public List<SerializableIntList> startingTroopUnitIds;
	public List<SerializableIntList> startingTroopUnitAmounts;
	public List<BattleBranchData> battleBranches;

	public ItemData itemData;
	public UnitData unitData;
	public HeroData heroData;
	public AbilityData abilityData;
	public TileDataSet tileDataSet;
	public RewardTableData rewardTableData;
	public AbilityTree abilityTree;

	public TownData townData;
	public Font font;
	public Color textColorInActive;
	public Color textColorActive;
//	public Battle[] GetBattles(){
//		
//	}
//
	public string visualEffectsPath;
	public string bloodParticleName;
	public string unwalkableObjectsPrefabPath = "UnwalkableObjects";
	public string[] spriteDataPaths;

	public Dictionary<TileID, TileData>GetTileDataSet(){
		Dictionary<TileID, TileData> tileData = new Dictionary<TileID, TileData> ();
		for (int i=0; i<tileDataSet.tileDatas.Count; ++i) {
			tileData.Add(tileDataSet.tileDatas[i].id, tileDataSet.tileDatas[i]);
		}
		return tileData;
	}
	public Troop[] GetStartingTroops(){
		Troop[] troops = new Troop[startingTroopHeroIds.Count];
		for (int i=0; i<startingTroopHeroIds.Count; ++i) {
			Troop newTroop = new Troop();
			newTroop.units = new List<Unit>();
			newTroop.hero = GetHeroes()[startingTroopHeroIds[i]].GetDublicate();
			for (int i2=0; i2<startingTroopUnitIds[i].intList.Count; ++i2){
				int unitID = startingTroopUnitIds[i].intList[i2];
				int unitAmount = startingTroopUnitAmounts[i].intList[i2];
				newTroop.units.Add (GetUnits()[unitID].GetDublicate());
				newTroop.units[i2].amount = unitAmount;

			}
			troops[i] = newTroop;
		}
		return troops;
	}
	public Hero[] GetHeroes(){
		return heroData.GetHeroes();
	}
	
	public Unit[] GetUnits(){

		return unitData.GetUnits();
	}
	public Ability[] GetAbilities(){
		return abilityData.GetAbilities();
	}
	public Item[] GetItems(){
		return itemData.GetItems();
	}
	public Town[] GetTowns(){
		return townData.GetTowns();
	}
	public RewardTable[] GetRewardTables(){
		return rewardTableData.GetTables ();
	}
} 
