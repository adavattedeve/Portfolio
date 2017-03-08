#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
public class CreateRewardTableData {
	[MenuItem("Assets/Create/RewardTableData ")]
	public static RewardTableData Create(){
		RewardTableData  asset = ScriptableObject.CreateInstance<RewardTableData > ();
		AssetDatabase.CreateAsset (asset, "Assets/GameDataFiles/RewardTableData .asset");
		AssetDatabase.SaveAssets ();
		return asset;
	}
}
#endif