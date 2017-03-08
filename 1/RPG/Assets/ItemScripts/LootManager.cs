using UnityEngine;
using System.Collections;

public class LootManager : MonoBehaviour {
	public LootTables lootTablesSO;

	public static LootManager instance;

	void Awake(){
		if (instance == null) {
			instance = this;
		}
	}
	public GameObject GetLootFromTable(int tableID){
		IItem item = lootTablesSO.GetLootFromTable (tableID);
		GameObject returnObject = Instantiate (item.ObjectPrefab);
		returnObject.GetComponentInChildren<ItemPickUp> ().Item = item;
		return returnObject;
	}
}
