using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour {
	private ItemSlot[] inventory;
	private ItemSlot consumableSlot;
	public ItemSlot Consumable{get {return consumableSlot;}}
	private Gear gear;
	public int inventorySize;
	void Awake () {
		gear = GetComponent<Gear> ();
		inventory = new ItemSlot[inventorySize];
		for (int i=0; i<inventorySize; ++i) {
			inventory[i]=new ItemSlot(i, ItemType.ANY);
		}
		consumableSlot = new ItemSlot (-1, ItemType.CONSUMABLE);
	}

	public void AddItem(IItem item){
		if (item == null) {
			return;}
		if (item is IStackable) {
			for (int i=0; i<inventorySize; ++i) {
				if (inventory[i].Item!=null && inventory[i].Item.Name==item.Name){
					Destroy(item.GO);
					IStackable temp;
					temp =(IStackable) inventory[i].Item as IStackable;
					temp.AddToStack();
					GlobalEvents.instance.LaunchOnInventoryChange ();
					return;
				}
			}
		}
		int index=0;
		for (int i=0; i<inventorySize; ++i) {
			if (inventory[i].Item==null){
				inventory[i].Item=item;
				item.GO.transform.parent = transform;
				index=i;
				break;
			}
		}
		if (consumableSlot.Item ==null && item is Consumable) {
			consumableSlot.Item = item;
			consumableSlot.index = index;
		}
		GlobalEvents.instance.LaunchOnInventoryChange ();
	}
	public void DropItem(int itemIndex){
		inventory [itemIndex].Item = null;
	}

	public void SwapItems(int itemIndex1, int itemIndex2){
		IItem item=inventory[itemIndex2].Item;
		inventory [itemIndex2].Item = inventory [itemIndex1].Item;
		inventory [itemIndex1].Item = item;
		GlobalEvents.instance.LaunchOnInventoryChange ();
	}
	public IItem GetItem(int index){
		return inventory[index].Item;
	}
	public void SetItem(IItem item, int index){
		inventory [index].Item = item;
		GlobalEvents.instance.LaunchOnInventoryChange ();
	}
	public void UseConsumable(){
		if (consumableSlot.Item != null) {
			consumableSlot.Item.Use (consumableSlot.index);
			GlobalEvents.instance.LaunchOnInventoryChange ();
		}
	}
}
