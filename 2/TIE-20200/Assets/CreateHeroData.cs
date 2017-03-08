#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class CreateHeroData  {
	[MenuItem("Assets/Create/Herodata")]
	public static HeroData Create(){
		HeroData asset = ScriptableObject.CreateInstance<HeroData> ();
		AssetDatabase.CreateAsset (asset, "Assets/GameDataFiles/HeroData.asset");
		AssetDatabase.SaveAssets ();
		return asset;
	}
}
#endif