#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class CreateBattleBranchData  {
	[MenuItem("Assets/Create/BattleBranchData")]
	public static BattleBranchData Create(){
		BattleBranchData asset = ScriptableObject.CreateInstance<BattleBranchData> ();
		AssetDatabase.CreateAsset (asset, "Assets/GameDataFiles/BattleBranchData.asset");
		AssetDatabase.SaveAssets ();
		return asset;
	}
}
#endif