#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class LootTableEditor: EditorWindow{
	public LootTables lootTables;
	private int lootTableViewIndex =1;
	private int lootViewIndex=1;
	
	[MenuItem("Window/LootTable Editor %#e")]
	static void Init(){
		EditorWindow.GetWindow (typeof(LootTableEditor));
	}
	void OnEnable(){
		if (EditorPrefs.HasKey ("LootTablePath")) {
			string objectPath = EditorPrefs.GetString ("LootTablePath");
			lootTables = AssetDatabase.LoadAssetAtPath (objectPath, typeof(LootTables)) as LootTables;
		}
	}

	void OnGUI(){
		//Checking if loot table SO exists
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("LootTable editor", EditorStyles.boldLabel);
		if (lootTables != null) {
			if (GUILayout.Button ("Show LootTable SO")) {
				EditorUtility.FocusProjectWindow ();
				Selection.activeObject = lootTables;
			}
		}
		if (GUILayout.Button ("Open LootTable SO")) {
			OpenLootTables ();
		}

		GUILayout.EndHorizontal ();
		
		if (lootTables == null) {
			
			GUILayout.BeginHorizontal ();
			GUILayout.Space (10);
			if (GUILayout.Button ("Create New LootTable List", GUILayout.ExpandWidth (false))) {
				CreateNewLootTables ();
			}
			GUILayout.EndHorizontal ();
		}
		
		GUILayout.Space (20);

		//Creating LootTableLists
		if (lootTables != null &&lootTables.lootTables !=null) {

			if (lootTables.lootTables.Count < lootTableViewIndex) {
				lootTableViewIndex = lootTables.lootTables.Count;
			}
			GUILayout.BeginHorizontal ();
				
			GUILayout.Space (10);
			if (GUILayout.Button ("Prev LootTable", GUILayout.ExpandWidth (false))) {
				if (lootTableViewIndex > 1) {
					--lootTableViewIndex;
				}
			}
			GUILayout.Space (5);
			if (GUILayout.Button ("Next LootTable", GUILayout.ExpandWidth (false))) {
				if (lootTableViewIndex < lootTables.lootTables.Count) {
					lootTableViewIndex ++;
				}
			}
			if (GUILayout.Button ("Add LootTable", GUILayout.ExpandWidth (false))) {
				AddLootTable ();
			}
			if (GUILayout.Button ("Delete LootTable", GUILayout.ExpandWidth (false))) {
				DeleteLootTable (lootTableViewIndex - 1);
			}
				
			GUILayout.EndHorizontal ();
			//Creating Loots to current LootTable List
			if (lootTables.lootTables.Count > 0 && lootTables.lootTables[lootTableViewIndex-1]!=null){
				lootTableViewIndex = Mathf.Clamp (EditorGUILayout.IntField ("Current Loot Table", lootTableViewIndex, 
				                                                   GUILayout.ExpandWidth (false)), 1,
				                                  lootTables.lootTables.Count);
				EditorGUILayout.IntField ("LootTable ID ", lootTables.lootTables[lootTableViewIndex-1].TableID);
				lootTables.lootTables[lootTableViewIndex-1].name = EditorGUILayout.TextField ("Name", 
				                                                                              lootTables.lootTables[lootTableViewIndex-1].name as string);

				if (lootTables.lootTables[lootTableViewIndex-1].lootTable.Count < lootViewIndex) {
					lootViewIndex = lootTables.lootTables[lootTableViewIndex-1].lootTable.Count;
				}
				GUILayout.BeginHorizontal ();
				
				GUILayout.Space (10);
				if (GUILayout.Button ("Prev Loot", GUILayout.ExpandWidth (false))) {
					if (lootViewIndex > 1) {
						--lootViewIndex;
					}
				}
				GUILayout.Space (5);
				if (GUILayout.Button ("Next Loot", GUILayout.ExpandWidth (false))) {
					if (lootViewIndex < lootTables.lootTables[lootTableViewIndex-1].lootTable.Count) {
						lootViewIndex ++;
					}
				}
				if (GUILayout.Button ("Add Loot", GUILayout.ExpandWidth (false))) {
					AddLoot ();
				}
				if (GUILayout.Button ("Delete Loot", GUILayout.ExpandWidth (false))) {
					DeleteLoot (lootViewIndex - 1);
				}
				GUILayout.EndHorizontal();
				//Editing Loot in loottable
				if (lootTables.lootTables[lootTableViewIndex-1].lootTable.Count>0){
					lootViewIndex = Mathf.Clamp (EditorGUILayout.IntField ("Current Loot", lootViewIndex, 
					                                                            GUILayout.ExpandWidth (false)), 1,
					                             lootTables.lootTables[lootTableViewIndex-1].lootTable.Count);
					lootTables.lootTables[lootTableViewIndex-1].lootTable[lootViewIndex - 1].distributionValue = 
						EditorGUILayout.IntField ("Loot DistributionValue", 
						                          lootTables.lootTables[lootTableViewIndex-1].lootTable[lootViewIndex - 1].distributionValue);
					lootTables.lootTables[lootTableViewIndex-1].lootTable[lootViewIndex - 1].itemID = 
						EditorGUILayout.IntField ("Item ID", 
						                           lootTables.lootTables[lootTableViewIndex-1].lootTable[lootViewIndex - 1].itemID);
				}

			}
			if (GUI.changed) {
				EditorUtility.SetDirty (lootTables);
			}
		}
	}
	void CreateNewLootTables(){
		lootTableViewIndex = 1;
		lootTables = CreateLootTables.Create ();
		if (lootTables){
			string relPath = AssetDatabase.GetAssetPath(lootTables);
			EditorPrefs.SetString("LootTablePath", relPath);
		}
	}
	void OpenLootTables(){
		string absPath = EditorUtility.OpenFilePanel ("Select LootTables SO", "", "");
		if (absPath.StartsWith(Application.dataPath)){
			string relPath = absPath.Substring(Application.dataPath.Length-"Assets".Length);
			lootTables = AssetDatabase.LoadAssetAtPath(relPath, typeof(LootTables)) as LootTables;
			if (lootTables){
				EditorPrefs.SetString("LootTablePath", relPath);
			}
		}
	}

	void AddLootTable(){
		lootTables.lootTables.Add (new LootTable(lootTables.GetNewTableID()));
		lootTableViewIndex = lootTables.lootTables.Count;
	}

	void DeleteLootTable(int index){
		if (index < lootTables.lootTables.Count) {
			lootTables.lootTables.RemoveAt (index);
			--lootTableViewIndex;
		}
	}
	void AddLoot(){
		lootTables.lootTables [lootTableViewIndex-1].lootTable.Add (new Loot());
		lootViewIndex = lootTables.lootTables[lootTableViewIndex-1].lootTable.Count;
	}
	void DeleteLoot(int index){
		if (index < lootTables.lootTables[lootTableViewIndex-1].lootTable.Count) {
			lootTables.lootTables[lootTableViewIndex-1].lootTable.RemoveAt (index);
			if (lootViewIndex>1){
				--lootViewIndex;
			}
		}
	}
}
#endif
