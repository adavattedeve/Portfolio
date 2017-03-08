#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class CreateUnitData  {
	[MenuItem("Assets/Create/Unitdata")]
	public static UnitData Create(){
		UnitData asset = ScriptableObject.CreateInstance<UnitData> ();
		AssetDatabase.CreateAsset (asset, "Assets/GameDataFiles/UnitData.asset");
		AssetDatabase.SaveAssets ();
		return asset;
	}
}
#endif