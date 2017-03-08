using UnityEngine;
using System.Collections;

public class LootSpawning : MonoBehaviour {
	public int lootTableID;
	public float averageLootCount;

	public void SpawnLoot(){
		float tempLootCount = averageLootCount;
		while (tempLootCount>0) {
			tempLootCount -= Random.Range (0.4f, 1.6f);
			if (tempLootCount<=0){
				return;
			}
			GameObject loot = LootManager.instance.GetLootFromTable (lootTableID);
			loot.transform.position = transform.position;
		}
	}
}
