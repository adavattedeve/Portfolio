#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


public class ItemEditor : EditorWindow {

	public ItemData itemData;
	private int viewIndex =1;

	public ItemType newItemType;
	[MenuItem("Window/Item Editor %#e")]
	static void Init(){
		EditorWindow.GetWindow (typeof(ItemEditor));
	}

	void OnEnable(){
		if (EditorPrefs.HasKey ("ObjectPath")) {
			string objectPath = EditorPrefs.GetString ("ObjectPath");
			itemData = AssetDatabase.LoadAssetAtPath (objectPath, typeof(ItemData)) as ItemData;
		}
	}
void OnGUI () {
	GUILayout.BeginHorizontal ();
	GUILayout.Label ("Item editor", EditorStyles.boldLabel);
		if (itemData != null) {
			if (GUILayout.Button ("Show Item List")) {
				EditorUtility.FocusProjectWindow ();
				Selection.activeObject = itemData;
			}
	}
		if (GUILayout.Button ("Open Item SO")) {
			OpenItemSO ();
	}
//		if (GUILayout.Button ("New Item List")) {
//			EditorUtility.FocusProjectWindow ();
//			Selection.activeObject = itemData;
//	}
	GUILayout.EndHorizontal ();

		if (itemData == null) {

			GUILayout.BeginHorizontal();
			GUILayout.Space(10);
			if (GUILayout.Button ("Create New Item SO", GUILayout.ExpandWidth(false))){
				CreateNewItemSO();}
			if (GUILayout.Button ("Open Existing Item SO", GUILayout.ExpandWidth(false))){
				OpenItemSO ();
			}
			GUILayout.EndHorizontal();
	}

		GUILayout.Space (20);

		if (itemData != null) {
			GUILayout.BeginHorizontal();
			GUILayout.Space(10);
			newItemType = (ItemType)EditorGUILayout.EnumPopup("Itemtype", newItemType);
			GUILayout.EndHorizontal();
			//consumables
			if (newItemType == ItemType.CONSUMABLE) {
				if (itemData.consumables.Count<viewIndex){
					viewIndex =itemData.consumables.Count;
				}
				GUILayout.BeginHorizontal ();
				
				GUILayout.Space (10);
				if (GUILayout.Button ("Prev", GUILayout.ExpandWidth (false))) {
					if (viewIndex > 1) {
						--viewIndex;
					}
				}
				GUILayout.Space (5);
				if (GUILayout.Button ("Next", GUILayout.ExpandWidth (false))) {
					if (viewIndex < itemData.consumables.Count) {
						viewIndex ++;
					}
				}
				if (GUILayout.Button ("AddItem", GUILayout.ExpandWidth (false))) {
					AddItem ();
				}
				if (GUILayout.Button ("Delete Item", GUILayout.ExpandWidth (false))) {
					DeleteItem (viewIndex - 1);
				}

				GUILayout.EndHorizontal ();
				
				if (itemData.consumables.Count > 0) {
					GUILayout.BeginHorizontal ();
					viewIndex = Mathf.Clamp (EditorGUILayout.IntField ("Current item", viewIndex, 
					                                                   GUILayout.ExpandWidth (false)), 1,
					                         itemData.consumables.Count);
					EditorGUILayout.LabelField ("of  " + itemData.consumables.Count.ToString () + "  consumables",
					                            "", GUILayout.ExpandWidth (false));
					GUILayout.EndHorizontal ();

					EditorGUILayout.EnumPopup("Itemtype", itemData.consumables [viewIndex - 1].Type);
					GUILayout.BeginHorizontal ();
					EditorGUILayout.IntField("Item ID", itemData.consumables[viewIndex-1].ID);
					if (GUILayout.Button ("GetNew ID", GUILayout.ExpandWidth (false))) {
						itemData.consumables [viewIndex - 1].ID = itemData.GetNewItemID();
					}
					GUILayout.EndHorizontal ();
					itemData.consumables [viewIndex - 1].Name = 
						EditorGUILayout.TextField ("Name", 
						                           itemData.consumables [viewIndex - 1].Name as string);
					itemData.consumables [viewIndex - 1].Description = 
						EditorGUILayout.TextField ("Description", 
						                           itemData.consumables [viewIndex - 1].Description as string);
					itemData.consumables[viewIndex - 1].ObjectPrefab = 
						(GameObject)EditorGUILayout.ObjectField ("Prefab", 
						                                         itemData.consumables [viewIndex - 1].ObjectPrefab,
						                                         typeof (GameObject), false);
					itemData.consumables [viewIndex - 1].Icon = 
						(Sprite)EditorGUILayout.ObjectField ("Icon", 
						                                     itemData.consumables[viewIndex - 1].Icon,
						                                     typeof (Sprite), false);

					if (GUILayout.Button ("Add restoration effect", GUILayout.ExpandWidth (false))) {
						if (itemData.consumables [viewIndex - 1].restoration==null){
							itemData.consumables [viewIndex - 1].restoration = new List<Restoration>();
						}
						itemData.consumables [viewIndex - 1].restoration.Add(new Restoration(Resource.HEALTH, 0));
					}
					if (GUILayout.Button ("Clear restoration effects", GUILayout.ExpandWidth (false))) {
						itemData.consumables [viewIndex - 1].restoration=null;
					}
					if (GUILayout.Button ("Add buff effect", GUILayout.ExpandWidth (false))) {
						if (itemData.consumables [viewIndex - 1].buff==null){
							itemData.consumables [viewIndex - 1].buff = new List<Stat>();
						}
						itemData.consumables [viewIndex - 1].buff.Add(new Stat(StatType.ARMOR, 0, 0));
					}
					if (GUILayout.Button ("Clear buff effects", GUILayout.ExpandWidth (false))) {
						itemData.consumables [viewIndex - 1].buff=null;
					}
					if (itemData.consumables [viewIndex - 1].restoration != null && itemData.consumables [viewIndex - 1].restoration.Count>0) {
						for (int i=0; i<itemData.consumables [viewIndex - 1].restoration.Count; ++i){
							Resource tempType = (Resource)EditorGUILayout.EnumPopup("Restorable resource", itemData.consumables [viewIndex - 1].restoration[i].type);
							float tempValue = EditorGUILayout.FloatField ("Restoration Amount", 
							                                              itemData.consumables [viewIndex - 1].restoration[i].amount);
							itemData.consumables [viewIndex - 1].restoration[i].SetValues(tempType, tempValue);
						}
					}
					if (itemData.consumables [viewIndex - 1].buff != null && itemData.consumables [viewIndex - 1].buff.Count > 0) {
						itemData.consumables [viewIndex - 1].buffTime = 
							EditorGUILayout.FloatField ("buffTime", 
							                           itemData.consumables [viewIndex - 1].buffTime);
						for (int i=0; i<itemData.consumables [viewIndex - 1].buff.Count; ++i){
							itemData.consumables [viewIndex - 1].buff[i].type =
								(StatType)EditorGUILayout.EnumPopup("Buffable stat", itemData.consumables [viewIndex - 1].buff[i].type);
							itemData.consumables [viewIndex - 1].buff[i].amount = EditorGUILayout.FloatField ("buff Amount", 
							                            itemData.consumables [viewIndex - 1].buff[i].amount);
						}
					}
					GUILayout.Space (10);
				} else {
					GUILayout.Label ("This List Is Empty.");
				}
			}
			//Weapon
		else if (newItemType == ItemType.WEAPON) {
			if (itemData.weapons.Count<viewIndex){
				viewIndex =itemData.weapons.Count;
			}
			GUILayout.BeginHorizontal ();
			
			GUILayout.Space (10);
			if (GUILayout.Button ("Prev", GUILayout.ExpandWidth (false))) {
				if (viewIndex > 1) {
					--viewIndex;
				}
			}
			GUILayout.Space (5);
			if (GUILayout.Button ("Next", GUILayout.ExpandWidth (false))) {
				if (viewIndex < itemData.weapons.Count) {
					viewIndex ++;
				}
			}
			if (GUILayout.Button ("AddItem", GUILayout.ExpandWidth (false))) {
				AddItem ();
			}
			if (GUILayout.Button ("Delete Item", GUILayout.ExpandWidth (false))) {
				DeleteItem (viewIndex - 1);
			}
			
			GUILayout.EndHorizontal ();
			
			if (itemData.weapons.Count > 0) {
				GUILayout.BeginHorizontal ();
				viewIndex = Mathf.Clamp (EditorGUILayout.IntField ("Current item", viewIndex, 
				                                                   GUILayout.ExpandWidth (false)), 1,
				                         itemData.weapons.Count);
				EditorGUILayout.LabelField ("of  " + itemData.weapons.Count.ToString () + "  weapons",
				                            "", GUILayout.ExpandWidth (false));
				GUILayout.EndHorizontal ();

					EditorGUILayout.EnumPopup("Itemtype", itemData.weapons [viewIndex - 1].Type);
					GUILayout.BeginHorizontal ();
					EditorGUILayout.IntField("Item ID", itemData.weapons[viewIndex-1].ID);
					if (GUILayout.Button ("GetNew ID", GUILayout.ExpandWidth (false))) {
						itemData.weapons [viewIndex - 1].ID = itemData.GetNewItemID();
					}
					GUILayout.EndHorizontal ();

				itemData.weapons [viewIndex - 1].Name = 
					EditorGUILayout.TextField ("Name", 
					                           itemData.weapons [viewIndex - 1].Name as string);
				itemData.weapons [viewIndex - 1].Description = 
					EditorGUILayout.TextField ("Description", 
					                           itemData.weapons [viewIndex - 1].Description as string);
				itemData.weapons[viewIndex - 1].ObjectPrefab = 
					(GameObject)EditorGUILayout.ObjectField ("Prefab", 
					                                         itemData.weapons [viewIndex - 1].ObjectPrefab,
					                                         typeof (GameObject), false);
				itemData.weapons [viewIndex - 1].Icon = 
					(Sprite)EditorGUILayout.ObjectField ("Icon", 
					                                     itemData.weapons[viewIndex - 1].Icon,
					                                     typeof (Sprite), false);
					itemData.weapons [viewIndex - 1].statRangeID =  EditorGUILayout.IntField("StatRange ID", itemData.weapons[viewIndex-1].statRangeID);
					if (GUILayout.Button ("Add Stat", GUILayout.ExpandWidth (false))) {
						if (itemData.weapons [viewIndex - 1].stats==null){
							itemData.weapons [viewIndex - 1].stats = new List<Stat>();
						}
						itemData.weapons [viewIndex - 1].stats.Add(new Stat(StatType.ARMOR, 0, 0));
					}
					if (GUILayout.Button ("Clear Stats", GUILayout.ExpandWidth (false))) {
						itemData.weapons [viewIndex - 1].stats=null;
					}
					if (itemData.weapons [viewIndex - 1].stats != null && itemData.weapons [viewIndex - 1].stats.Count > 0) {
						for (int i=0; i<itemData.weapons [viewIndex - 1].stats.Count; ++i){
							itemData.weapons [viewIndex - 1].stats[i].type =
								(StatType)EditorGUILayout.EnumPopup("Stat", itemData.weapons [viewIndex - 1].stats[i].type);
						}
					}
					itemData.weapons [viewIndex - 1].randomStatAmount = EditorGUILayout.IntField ("Random stat count", 
					                                                                              itemData.weapons [viewIndex - 1].randomStatAmount);
					if (GUILayout.Button ("Add RandomStat", GUILayout.ExpandWidth (false))) {
						if (itemData.weapons [viewIndex - 1].randomStatTypes==null){
							itemData.weapons [viewIndex - 1].randomStatTypes = new List<StatType>();
						}
						itemData.weapons [viewIndex - 1].randomStatTypes.Add(StatType.ARMOR);
					}
					if (GUILayout.Button ("Clear RandomStats", GUILayout.ExpandWidth (false))) {
						itemData.weapons [viewIndex - 1].randomStatTypes=null;
					}
					if (itemData.weapons [viewIndex - 1].randomStatTypes != null && itemData.weapons [viewIndex - 1].stats.Count > 0) {
						for (int i=0; i<itemData.weapons [viewIndex - 1].randomStatTypes.Count; ++i){
							itemData.weapons [viewIndex - 1].randomStatTypes[i] =
								(StatType)EditorGUILayout.EnumPopup("Stat", itemData.weapons [viewIndex - 1].randomStatTypes[i]);
						}
					}
				GUILayout.Space (10);
			} else {
				GUILayout.Label ("This List Is Empty.");
			}
			}
		// Equipment
			else{
			if (itemData.equipments.Count<viewIndex){
				viewIndex =itemData.equipments.Count;
			}
			GUILayout.BeginHorizontal ();
			
			GUILayout.Space (10);
			if (GUILayout.Button ("Prev", GUILayout.ExpandWidth (false))) {
				if (viewIndex > 1) {
					--viewIndex;
				}
			}
			GUILayout.Space (5);
			if (GUILayout.Button ("Next", GUILayout.ExpandWidth (false))) {
				if (viewIndex < itemData.equipments.Count) {
					viewIndex ++;
				}
			}
			if (GUILayout.Button ("AddItem", GUILayout.ExpandWidth (false))) {
				AddItem ();
			}
			if (GUILayout.Button ("Delete Item", GUILayout.ExpandWidth (false))) {
				DeleteItem (viewIndex - 1);
			}
			
			GUILayout.EndHorizontal ();
			
			if (itemData.equipments.Count > 0) {
				GUILayout.BeginHorizontal ();
				viewIndex = Mathf.Clamp (EditorGUILayout.IntField ("Current item", viewIndex, 
				                                                   GUILayout.ExpandWidth (false)), 1,
				                         itemData.equipments.Count);
				EditorGUILayout.LabelField ("of  " + itemData.equipments.Count.ToString () + "  equipment",
				                            "", GUILayout.ExpandWidth (false));
				GUILayout.EndHorizontal ();
				EditorGUILayout.EnumPopup("Itemtype", itemData.equipments [viewIndex - 1].Type);
				
					GUILayout.BeginHorizontal ();
					EditorGUILayout.IntField("Item ID", itemData.equipments[viewIndex-1].ID);
					if (GUILayout.Button ("GetNew ID", GUILayout.ExpandWidth (false))) {
						itemData.equipments [viewIndex - 1].ID = itemData.GetNewItemID();
					}
					GUILayout.EndHorizontal ();

				itemData.equipments [viewIndex - 1].Name = 
					EditorGUILayout.TextField ("Name", 
					                           itemData.equipments [viewIndex - 1].Name as string);
				itemData.equipments [viewIndex - 1].Description = 
					EditorGUILayout.TextField ("Description", 
					                           itemData.equipments [viewIndex - 1].Description as string);
				itemData.equipments[viewIndex - 1].ObjectPrefab = 
					(GameObject)EditorGUILayout.ObjectField ("Prefab", 
					                                         itemData.equipments [viewIndex - 1].ObjectPrefab,
					                                         typeof (GameObject), false);
				itemData.equipments [viewIndex - 1].Icon = 
					(Sprite)EditorGUILayout.ObjectField ("Icon", 
					                                     itemData.equipments[viewIndex - 1].Icon,
					                                     typeof (Sprite), false);
				
					itemData.equipments [viewIndex - 1].statRangeID =  EditorGUILayout.IntField("StatRange ID", itemData.equipments[viewIndex-1].statRangeID);
					if (GUILayout.Button ("Add Stat", GUILayout.ExpandWidth (false))) {
						if (itemData.equipments [viewIndex - 1].stats==null){
							itemData.equipments [viewIndex - 1].stats = new List<Stat>();
						}
						itemData.equipments [viewIndex - 1].stats.Add(new Stat(StatType.ARMOR, 0, 0));
					}
					if (GUILayout.Button ("Clear Stats", GUILayout.ExpandWidth (false))) {
						itemData.equipments [viewIndex - 1].stats=null;
					}
					if (itemData.equipments [viewIndex - 1].stats != null && itemData.equipments [viewIndex - 1].stats.Count > 0) {
						for (int i=0; i<itemData.equipments [viewIndex - 1].stats.Count; ++i){
							itemData.equipments [viewIndex - 1].stats[i].type =
								(StatType)EditorGUILayout.EnumPopup("Stat", itemData.equipments [viewIndex - 1].stats[i].type);
						}
					}
					itemData.equipments [viewIndex - 1].randomStatAmount = EditorGUILayout.IntField ("Random stat count", 
					                                                                                 itemData.equipments [viewIndex - 1].randomStatAmount);
					if (GUILayout.Button ("Add RandomStat", GUILayout.ExpandWidth (false))) {
						if (itemData.equipments [viewIndex - 1].randomStatTypes==null){
							itemData.equipments [viewIndex - 1].randomStatTypes = new List<StatType>();
						}
						itemData.equipments [viewIndex - 1].randomStatTypes.Add(StatType.ARMOR);
					}
					if (GUILayout.Button ("Clear RandomStats", GUILayout.ExpandWidth (false))) {
						itemData.equipments [viewIndex - 1].randomStatTypes=null;
					}
					if (itemData.equipments [viewIndex - 1].randomStatTypes != null && itemData.equipments [viewIndex - 1].stats != null && itemData.equipments [viewIndex - 1].stats.Count > 0) {
						for (int i=0; i<itemData.equipments [viewIndex - 1].randomStatTypes.Count; ++i){
							itemData.equipments [viewIndex - 1].randomStatTypes[i] =
								(StatType)EditorGUILayout.EnumPopup("Stat", itemData.equipments [viewIndex - 1].randomStatTypes[i]);
						}
					}

				GUILayout.Space (10);
			} else {
				GUILayout.Label ("This List Is Empty.");
			}
			}
			if (GUI.changed) {
				EditorUtility.SetDirty (itemData);
			}
		}

	}

	void CreateNewItemSO(){
		viewIndex = 1;
		itemData = CreateItemData.Create ();
		if (itemData){
			string relPath = AssetDatabase.GetAssetPath(itemData);
			EditorPrefs.SetString("ObjectPath", relPath);
		}
	}
	void OpenItemSO(){
		string absPath = EditorUtility.OpenFilePanel ("Select Item SO", "", "");
		if (absPath.StartsWith(Application.dataPath)){
			string relPath = absPath.Substring(Application.dataPath.Length-"Assets".Length);
			itemData = AssetDatabase.LoadAssetAtPath(relPath, typeof(ItemData)) as ItemData;
			if (itemData){
				EditorPrefs.SetString("ObjectPath", relPath);
			}
		}
	}

		void AddItem(){
		if (newItemType == ItemType.CONSUMABLE) {
			Consumable newItem = new Consumable ();
			newItem.Name = "new item";
			itemData.consumables.Add (newItem);
			newItem.Type=newItemType;
			newItem.ID = itemData.GetNewItemID();
			viewIndex = itemData.consumables.Count;
		}else if (newItemType == ItemType.WEAPON) {
			Weapon newItem = new Weapon ();
			newItem.Name = "new item";
			itemData.weapons.Add (newItem);
			newItem.Type=newItemType;
			newItem.ID = itemData.GetNewItemID();
			viewIndex = itemData.weapons.Count;
		}
		else{
			Equipment newItem = new Equipment ();
			newItem.Name = "new item";
			itemData.equipments.Add (newItem);
			newItem.Type=newItemType;
			newItem.ID = itemData.GetNewItemID();
			viewIndex = itemData.equipments.Count;
		}

	}

		void DeleteItem(int index){
		if (newItemType == ItemType.CONSUMABLE) {
			if (index < itemData.consumables.Count) {
				itemData.consumables.RemoveAt (index);
			}
		}else if (newItemType == ItemType.WEAPON) {
			if (index < itemData.weapons.Count) {
				itemData.weapons.RemoveAt (index);
			}
		}
		else{
			if (index < itemData.equipments.Count) {
				itemData.equipments.RemoveAt (index);
			}
		}
		}
}
#endif
