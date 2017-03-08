using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class ItemList {
	[System.NonSerialized]public List<Item> items;
	[SerializeField]private List<int> itemIds;
	public delegate void ItemsChange ();
	[field: System.NonSerialized]public event ItemsChange OnItemsChange;
	public ItemList(){
		items = new List<Item> ();
		itemIds = new List<int>();
	}
	public void AddItem(Item item, int index=-1){
		if (index > -1) {
			items [index] = item;
		} else {
			for (int i=0; i<items.Count; ++i) {
				if (items [i] == null) {
					items [i] = item;
					return;
				}
			}
			items.Add (item);
		}
		if (OnItemsChange != null) {
			OnItemsChange();
		}
	}
	public void DeleteItem(int index, ItemType type){
		items.Remove(GetItemList(type)[index]);
		if (OnItemsChange != null) {
			OnItemsChange();
		}
	}
	public Item GetItem(int index, ItemType type){
		List<Item> itemsList = GetItemList (type);
		if (itemsList.Count <= index) {
			return null;
		}
		return itemsList[index];
	}
	public List<Item> GetItemList(ItemType type){
		List<Item> returnItems = new List<Item> ();
		if (type == ItemType.NULL) {
			return items;
		}
		for (int i=0; i<items.Count; ++i) {
			if (items [i]!=null && items [i].type == type) {
				returnItems.Add(items [i]);
			}
		}
		return returnItems;
	}
	public void PrepareForSaving(){
		itemIds = new List<int> ();
		for (int i=0; i<items.Count; ++i) {
			if (items[i] !=null){
				itemIds.Add (items[i].id);
			}else{
				itemIds.Add (-1);
			}
		}
	}
	public void FinishLoading(){
		items = new List<Item>();
		for (int i=0; i<itemIds.Count; ++i) {
			items.Add (DataBase.instance.GetItem(itemIds[i]));
		}
	}
}
