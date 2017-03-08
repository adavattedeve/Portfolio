#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
public class CreateTownData {
	[MenuItem("Assets/Create/TownData ")]
	public static TownData Create(){
		TownData  asset = ScriptableObject.CreateInstance<TownData > ();
		AssetDatabase.CreateAsset (asset, "Assets/GameDataFiles/TownData .asset");
		AssetDatabase.SaveAssets ();
		return asset;
	}
}
#endif