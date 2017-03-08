using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ItemData : ScriptableObject {
	//private List<int> usedIDs;
	public List<Equipment> equipments;
//	public int GetNewItemID(){
//		int[] usedIDs = new int[equipments.Count+weapons.Count+consumables.Count];
//		for (int i=0; i<usedIDs.Length; ++i) {
//			for (int i2=0; i2<equipments.Count; ++i2) {
//				usedIDs[i] = equipments[i2].ID;
//				++i;
//			}
//			for (int i2=0; i2<weapons.Count; ++i2) {
//				usedIDs[i] = weapons[i2].ID;
//				++i;
//			}
//			for (int i2=0; i2<consumables.Count; ++i2) {
//				usedIDs[i] = consumables[i2].ID;
//				++i;
//			}
//		}
//		int tryID = 0;
//		for (int i=0; i<usedIDs.Length; ++i) {
//			if (usedIDs[i]==tryID){
//				i=-1;
//				++tryID;
//			}
//		}
//		return tryID;
//	}

	public Item[] GetItems(){
		Item[] items = new Item[equipments.Count];
		for (int i=0; i<items.Length; ++i) {
			for (int i2=0; i2<equipments.Count; ++i2) {
				items[i] = (Item)equipments[i2];
				++i;
			}
		}
		return items;
	}
}
