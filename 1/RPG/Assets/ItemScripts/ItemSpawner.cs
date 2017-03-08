using UnityEngine;
using System.Collections;

public class ItemSpawner : MonoBehaviour {
	public int[] itemIDs;
	public Transform[] spawnpoints;

	void Start(){
		for (int i=0; i<itemIDs.Length; ++i) {
			if (spawnpoints[i]==null){
				continue;
			}
			IItem item = ItemDB.instance.GetItemByID(itemIDs[i]);
			GameObject gO = (GameObject)Instantiate(item.ObjectPrefab, spawnpoints[i].position, spawnpoints[i].rotation);
			gO.GetComponentInChildren<ItemPickUp> ().Item = item;
		}

	}
}
