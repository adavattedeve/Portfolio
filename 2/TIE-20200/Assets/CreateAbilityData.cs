#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class CreateAbilityData  {
	[MenuItem("Assets/Create/AbilityData ")]
	public static AbilityData Create(){
		AbilityData  asset = ScriptableObject.CreateInstance<AbilityData > ();
		AssetDatabase.CreateAsset (asset, "Assets/GameDataFiles/AbilityData .asset");
		AssetDatabase.SaveAssets ();
		return asset;
	}
}
#endif