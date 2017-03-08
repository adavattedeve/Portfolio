#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class CreateItemData{
[MenuItem("Assets/Create/Itemdata")]
	public static ItemData Create(){
		ItemData asset = ScriptableObject.CreateInstance<ItemData> ();
		AssetDatabase.CreateAsset (asset, "Assets/GameDataFiles/ItemData.asset");
		AssetDatabase.SaveAssets ();
		return asset;
	}
}
#endif