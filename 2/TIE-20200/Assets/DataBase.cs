using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataBase : MonoBehaviour {
	public static DataBase instance;
	private Ability[] abilities;
	private Unit[] units;
	private Item[] items;
	private RewardTable[] rewardTables;
	private Dictionary<TileID, TileData> tileData;
	private Dictionary<string, Sprite> sprites;
	private Dictionary<string, GameObject> visualEffects;
//	[SerializeField] private string[] generalSpritePaths;
//	[SerializeField] private string[] statIconsPaths;
//	[SerializeField] private string[] ablilityIconsPaths;
	[System.NonSerialized]public Troop[] startingTroops;
	[System.NonSerialized]public AbilityTree abilityTree;
	[System.NonSerialized]public int[] itemDistributionValues;
	public GameplayData gameData;
	
	void Awake () {
		if (!instance) {
			instance = this;
			abilities = gameData.GetAbilities ();
			units = gameData.GetUnits ();
			items = gameData.GetItems ();
			startingTroops = gameData.GetStartingTroops ();
			rewardTables = gameData.GetRewardTables ();
			tileData = gameData.GetTileDataSet ();
			abilityTree = gameData.abilityTree;
			
			itemDistributionValues=new int[items.Length];
			for (int i=0; i<items.Length; ++i) {
				itemDistributionValues[i] = items[i].goldValue;
			}
			string[] spritePaths = gameData.spriteDataPaths;
			sprites = new Dictionary<string, Sprite>();
			for (int i=0; i< spritePaths.Length; ++i){
				Object[] spriteSheet = Resources.LoadAll(spritePaths[i]);
				if (spriteSheet!=null && spriteSheet.Length>0){
					for (int ii=1; ii<spriteSheet.Length; ++ii){
						sprites[spriteSheet[ii].name] = (Sprite)spriteSheet[ii] as Sprite;
					}
				}else{
					Debug.Log ("cant load sprites: "+spritePaths[i]);
				}
			}
			string visualEffectsPath = gameData.visualEffectsPath;
			visualEffects = new Dictionary<string, GameObject>();
			GameObject[] effects = Resources.LoadAll<GameObject>(visualEffectsPath);
			if (effects!=null && effects.Length>0){
				for (int i=0; i<effects.Length; ++i){
					Debug.Log (effects[i].name);
					visualEffects[effects[i].name] = effects[i];
				}
			}else{
				Debug.Log ("cant load visualEffectes : "+visualEffectsPath);
			}
		}

	}
	public GameObject GetVisualEffect(string name){
		if (visualEffects.ContainsKey (name)) {
			return visualEffects[name];
		}
		Debug.Log ("Cant find visualeffect named: " + name);
		return null;
	}
	public Sprite GetSprite(string name){
		if (sprites.ContainsKey (name)) {
			return sprites [name];
		}
		Debug.Log ("sprite doesn't exist: " + name);
		return null;
	}
	public Ability GetAbility(AbilityIdentifier id){
		for (int i=0; i<abilities.Length; ++i) {
			if ( abilities[i].id == id){
				return abilities[i];
			}
		}
		Debug.Log ("Ability with id: " + id + " not found!");
		return null;
	}
	public TileData GetTileData(TileID id){
		return tileData[id];
	}
	public Unit GetUnit(int id){
		for (int i=0; i<units.Length; ++i) {
			if ( units[i].id == id){
				Unit newUnit = new Unit();
				newUnit = units[i].GetDublicate();
				return newUnit;
			}
		}
		Debug.Log ("Unit with id: " + id + " not found!");
		return null;
	}
	public Item GetItem(int id){
		for (int i=0; i<items.Length; ++i) {
			if ( items[i].id == id){
				Item newItem = new Item();
				newItem = items[i];
				return newItem;
			}
		}
		Debug.Log ("item with id: " + id + " not found!");
		return null;
	}
	public Town[] GetTowns(){
		Town[] towns = gameData.GetTowns ();
		for (int i=0; i<towns.Length; ++i) {
			towns[i] = towns[i].GetDublicate();
		}
		return towns;
	}
	public RewardTable GetRewardTable(int id){
		for (int i=0; i<rewardTables.Length; ++i) {
			if ( rewardTables[i].id == id){
				RewardTable rewardTable = new RewardTable();
				rewardTable = rewardTables[i];
				return rewardTable;
			}
		}
		Debug.Log ("table with id: " + id + " not found!");
		return null;
	}
}
