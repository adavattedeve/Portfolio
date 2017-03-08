#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class CreateLootTables {
[MenuItem("Assets/Create/LootTables")]
	public static LootTables Create(){
		LootTables asset = ScriptableObject.CreateInstance<LootTables> ();
		asset.lootTables = new List<LootTable> ();
		AssetDatabase.CreateAsset (asset, "Assets/LootTables.asset");
		AssetDatabase.SaveAssets ();
		return asset;
	}
}
#endif