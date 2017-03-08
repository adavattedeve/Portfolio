#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class CreateGameplayData  {
	[MenuItem("Assets/Create/Gameplaydata")]
	public static GameplayData Create(){
		GameplayData asset = ScriptableObject.CreateInstance<GameplayData> ();
		AssetDatabase.CreateAsset (asset, "Assets/GameDataFiles/GameplayData.asset");
		AssetDatabase.SaveAssets ();
		return asset;
	}
}
#endif