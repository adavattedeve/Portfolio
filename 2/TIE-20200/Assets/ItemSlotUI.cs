using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
public enum ItemLocation{EQUIPPED, INVENTORY, SHOP, NULL}
public class ItemSlotUI : MonoBehaviour, IIndexable, IItemUI, IPointerDownHandler, IPointerUpHandler{
	
	
	private int index;
	public int Index{get{return index;}set{index=value;}}
	private ItemLocation itemLocation = ItemLocation.NULL;
	public ItemLocation ItemLocation{ set{
			itemListUI = GetComponentInParent<ItemListUI> ();
			if (itemListUI!=null){
				itemListUI.OnSelectedTypeChange+=Refresh;
			}
			itemLocation=value; 
			itemList = GetItems ();
			if (itemList != null) {
				itemList.OnItemsChange += Refresh;
			}
			Refresh ();
		}
	}
	
	private bool empty;
	public bool Empty{set{
			if (value){
				//button.interactable=false;
				childObjectsRoot.SetActive(false);
			}else{
				button.interactable=true;
				childObjectsRoot.SetActive(true);
			}empty=value;}}
	public Image itemIcon;
	public GameObject childObjectsRoot;
	private Button button;
	private ItemList itemList;
	private ItemListUI itemListUI;
	public Text price;
	void Awake(){
		button = GetComponent<Button> ();

	}
	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right && !empty) {
			itemList = GetItems ();
			if (itemLocation == ItemLocation.EQUIPPED){
				if (index < itemList.items.Count && itemList.items [index] != null) {
					GuiManager.instance.ShowEntityInfo ((Entity)itemList.items[index], eventData.pressPosition);
				}
			}
			else if (index < itemList.items.Count && itemList.items [index] != null) {
				GuiManager.instance.ShowEntityInfo ((Entity)itemList.GetItem( index, itemListUI.SelectedType), eventData.pressPosition);
			}
		}
	}
	public void OnPointerUp(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left) {
			if (itemLocation==ItemLocation.EQUIPPED){
				GameManager.instance.CurrentGame.playerTroop.hero.UnEquip(index);
			}
			else{
				itemListUI.SelectedIndex = index;
			}
		}
	}
	void OnEnable(){
		if (itemLocation != ItemLocation.NULL) {
			itemList = GetItems ();
			if (itemList != null) {
				itemList.OnItemsChange += Refresh;
			}
		}
	}
	void OnDisable(){
		itemList = GetItems ();
		if (itemList != null) {
			itemList.OnItemsChange -= Refresh;
		}
	}
	public void Refresh(){
		itemList = GetItems ();
		List<Item> items;
		if (itemListUI != null) {
			items = itemList.GetItemList (itemListUI.SelectedType);
		}
		else {
			items = itemList.GetItemList (ItemType.NULL);
		}
		if (items != null) {
			if (index < items.Count && items[index]!=null) {
				if (itemLocation==ItemLocation.SHOP){
					price.text = items[index].goldValue.ToString();
				}else if (itemLocation==ItemLocation.INVENTORY){
					price.text = (items[index].goldValue*DataBase.instance.gameData.itemOnSellGoldGainMpl).ToString();
				}else {
					price.gameObject.SetActive(false);
				}
				Empty=false;
				itemIcon.sprite = items[index].Icon;
				return;
				
			}
		}
		Empty = true;
	}
	
	private ItemList GetItems(){
		switch(itemLocation){
		case ItemLocation.EQUIPPED:
			return GameManager.instance.CurrentGame.playerTroop.hero.equipments;
		case ItemLocation.INVENTORY:
			return GameManager.instance.CurrentGame.playerTroop.hero.inventory;
		case ItemLocation.SHOP:
			return GameManager.instance.CurrentGame.shopItems;
		}
		return null;
	}
}
