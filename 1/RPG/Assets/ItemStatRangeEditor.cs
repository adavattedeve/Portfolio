#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ItemStatRangeEditor: EditorWindow{
	public ItemData itemData;
	private int statRangeViewIndex =1;
	
	[MenuItem("Window/StatRange Editor %#e")]
	static void Init(){
		EditorWindow.GetWindow (typeof(ItemStatRangeEditor));
	}
	void OnEnable(){
		if (EditorPrefs.HasKey ("ObjectPath")) {
			string objectPath = EditorPrefs.GetString ("ObjectPath");
			itemData = AssetDatabase.LoadAssetAtPath (objectPath, typeof(ItemData)) as ItemData;
		}
	}
	
	void OnGUI(){

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Statrange editor", EditorStyles.boldLabel);
		if (itemData != null) {
			if (GUILayout.Button ("Show ItemData")) {
				EditorUtility.FocusProjectWindow ();
				Selection.activeObject = itemData;
			}
		}
		
		GUILayout.EndHorizontal ();
		
		GUILayout.Space (20);
		

		if (itemData != null && itemData.statRanges !=null) {
			
			if (itemData.statRanges.Count < statRangeViewIndex) {
				statRangeViewIndex = itemData.statRanges.Count;
			}
			GUILayout.BeginHorizontal ();
			
			GUILayout.Space (10);
			if (GUILayout.Button ("Prev Statrange", GUILayout.ExpandWidth (false))) {
				if (statRangeViewIndex > 1) {
					--statRangeViewIndex;
				}
			}
			GUILayout.Space (5);
			if (GUILayout.Button ("Next Statrange", GUILayout.ExpandWidth (false))) {
				if (statRangeViewIndex < itemData.statRanges.Count) {
					statRangeViewIndex ++;
				}
			}
			if (GUILayout.Button ("Add Statrange", GUILayout.ExpandWidth (false))) {
				AddStatRange();
			}
			if (GUILayout.Button ("Delete Statrange", GUILayout.ExpandWidth (false))) {
				DeleteLootTable (statRangeViewIndex - 1);
			}
			
			GUILayout.EndHorizontal ();
			//Creating Loots to current LootTable List
			if (itemData.statRanges.Count > 0 && itemData.statRanges[statRangeViewIndex-1]!=null){
				statRangeViewIndex = Mathf.Clamp (EditorGUILayout.IntField ("Current Loot Table", statRangeViewIndex, 
				                                                            GUILayout.ExpandWidth (false)), 1,
				                                  itemData.statRanges.Count);
				EditorGUILayout.IntField ("Statrange ID ", itemData.statRanges[statRangeViewIndex-1].ID);

				itemData.statRanges [statRangeViewIndex - 1].name = 
					EditorGUILayout.TextField ("Name", 
					                           itemData.statRanges [statRangeViewIndex - 1].name as string);
				for (int i=0; i<itemData.statRanges [statRangeViewIndex - 1].max.Length; ++i){
					EditorGUILayout.EnumPopup("StatType", itemData.statRanges [statRangeViewIndex - 1].max[i].type);
					GUILayout.BeginHorizontal ();
					itemData.statRanges [statRangeViewIndex - 1].min[i].amount = EditorGUILayout.FloatField ("Min", 
					                                                                                         itemData.statRanges [statRangeViewIndex - 1].min[i].amount);
					itemData.statRanges [statRangeViewIndex - 1].max[i].amount = EditorGUILayout.FloatField ("Max", 
					                                                                                         itemData.statRanges [statRangeViewIndex - 1].max[i].amount);
					itemData.statRanges [statRangeViewIndex - 1].min[i].perLevel = EditorGUILayout.FloatField ("MinPerLevel", 
					                                                                                 itemData.statRanges [statRangeViewIndex - 1].min[i].perLevel);
					itemData.statRanges [statRangeViewIndex - 1].max[i].perLevel = EditorGUILayout.FloatField ("MaxPerLevel", 
					                                                                                         itemData.statRanges [statRangeViewIndex - 1].max[i].perLevel);
					
					GUILayout.EndHorizontal ();

				}


				}
				
			}
			if (GUI.changed) {
				EditorUtility.SetDirty (itemData);
			}
		}

	void OpenItemData(){
		string absPath = EditorUtility.OpenFilePanel ("Select ItemData SO", "", "");
		if (absPath.StartsWith(Application.dataPath)){
			string relPath = absPath.Substring(Application.dataPath.Length-"Assets".Length);
			itemData = AssetDatabase.LoadAssetAtPath(relPath, typeof(ItemData)) as ItemData;
			if (itemData){
				EditorPrefs.SetString("ObjectPath", relPath);
			}
		}
	}
	
	void AddStatRange(){
		itemData.statRanges.Add (new ItemStatRange(itemData.GetNewItemStatRangeID()));
		statRangeViewIndex = itemData.statRanges.Count;
	}
	
	void DeleteLootTable(int index){
		if (index < itemData.statRanges.Count) {
			itemData.statRanges.RemoveAt (index);
			--statRangeViewIndex;
		}
	}
}
#endif

