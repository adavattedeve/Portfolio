#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class CreateTileDataSet {
	[MenuItem("Assets/Create/TileDataSet ")]
	public static TileDataSet Create(){
		TileDataSet  asset = ScriptableObject.CreateInstance<TileDataSet > ();
		AssetDatabase.CreateAsset (asset, "Assets/GameDataFiles/TileDataSet .asset");
		AssetDatabase.SaveAssets ();
		return asset;
	}
}
#endif