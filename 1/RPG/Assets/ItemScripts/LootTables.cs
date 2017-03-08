using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class Loot{
//	public Loot(int _id, IItem _item){
//		id = _id;
//		item=_item;
//	}
	public int distributionValue;
	public int itemID;
}
[System.Serializable]
public class LootTable {

	public List<Loot> lootTable;
	public int TableID;
	private int idSum;
	public string name;

	public LootTable(int ID){
		TableID = ID;
		lootTable = new List<Loot> ();
	}
	public IItem GetLoot(){
		idSum = 0;
		for (int i=0; i<lootTable.Count; ++i) {
			idSum +=lootTable[i].distributionValue;
		}

		int random = Random.Range (0, idSum);
		int currentDistribution=0;
		for (int i=0; i<lootTable.Count; ++i) {
			if (random <lootTable[i].distributionValue+currentDistribution){
				if (ItemDB.instance!=null){
					return ItemDB.instance.GetItemByID(lootTable[i].itemID);
				}
				return null;
			}
			currentDistribution+=lootTable[i].distributionValue;
		}
		Debug.Log ("something wrong with loot id and distributions");
		return null;
	}
}
public class LootTables : ScriptableObject {
	private List<int> usedIDs;
	public List<LootTable> lootTables;

	public IItem GetLootFromTable(int tableID){
		for (int i=0; i<lootTables.Count; ++i) {
			if (lootTables[i].TableID == tableID){
				return lootTables[i].GetLoot();
			}
		}
		return null;
	}

	public int GetNewTableID(){
		int[] usedIDs = new int[lootTables.Count];
		for (int i=0; i<usedIDs.Length; ++i) {
			usedIDs[i] = lootTables[i].TableID;
		}
		int tryID = 0;
		for (int i=0; i<usedIDs.Length; ++i) {
			if (usedIDs[i]==tryID){
				i=-1;
				++tryID;
			}
		}
		return tryID;
	}

}
