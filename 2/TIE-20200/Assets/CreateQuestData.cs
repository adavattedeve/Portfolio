#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
public class CreateQuestData {
	[MenuItem("Assets/Create/QuestData ")]
	public static QuestData Create(){
		QuestData  asset = ScriptableObject.CreateInstance<QuestData > ();
		AssetDatabase.CreateAsset (asset, "Assets/GameDataFiles/QuestData .asset");
		AssetDatabase.SaveAssets ();
		return asset;
	}
}
#endif