#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class CreateItemData{
[MenuItem("Assets/Create/Item Data")]
	public static ItemData Create(){
		ItemData asset = ScriptableObject.CreateInstance<ItemData> ();
		asset.equipments = new List<Equipment> ();
		asset.weapons = new List<Weapon> ();
		asset.consumables = new List<Consumable> ();
		asset.statRanges = new List<ItemStatRange> ();
		AssetDatabase.CreateAsset (asset, "Assets/ItemData.asset");
		AssetDatabase.SaveAssets ();
		return asset;
	}
}
#endif