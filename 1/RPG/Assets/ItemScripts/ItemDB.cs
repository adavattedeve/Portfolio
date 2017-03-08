using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ItemDB : MonoBehaviour {
	public ItemData itemData;
	IItem[] items;
	public IItem[] Items{get{ return items; }}
	public static ItemDB instance;
	void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance!= this) {
			Destroy(this);
		}
		items = itemData.GetItems ();
	}

	public IItem GetItemReferenceByName(string name){
		for (int i=0; i<items.Length; ++i) {
			if ( items[i].Name == name) {
				return items[i].GetDublicate(1);
			}
		}
		return null;
	}
	public IItem GetItemByID(int id){
		for (int i=0; i<items.Length; ++i) {
			if ( items[i].ID == id) {
				return items[i].GetDublicate(1);
			}
		}
		return null;
	}
	public IItem GetItemByID(int id, int level){
		for (int i=0; i<items.Length; ++i) {
			if ( items[i].ID == id) {
				return items[i].GetDublicate(level);
			}
		}
		return null;
	}
	public IItem GetItemReferenceByObject(GameObject itemGO){
		for (int i=0; i<items.Length; ++i) {
			if (items[i].ObjectPrefab!=null && itemGO.name.StartsWith(items[i].ObjectPrefab.name)) {
				return items[i].GetDublicate(1);
			}
		}
		return null;
	}
}
