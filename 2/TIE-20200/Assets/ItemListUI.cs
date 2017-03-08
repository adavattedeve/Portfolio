using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class ItemListUI : MonoBehaviour {
	public ItemLocation itemLocation;

	public Button allCatergory;
	public Button weaponCatergory;
	public Button armorCatergory;
	public Button artifactCatergory;
	public Button[] actionButtons;

	private ItemType selectedType=ItemType.NULL;
	private Dictionary<int, MouseOnButtonAnimation> itemSlots;
	public delegate void SelectedTypeChangeAction();
	public event SelectedTypeChangeAction OnSelectedTypeChange;
	public ItemType SelectedType{
		set{
			GetButtonByType(selectedType).GetComponent<MouseOnButtonAnimation>().Selected=false;
			GetButtonByType(value).GetComponent<MouseOnButtonAnimation>().Selected=true;
			SelectedIndex=0;
			selectedType=value;
			if (OnSelectedTypeChange!=null){
				OnSelectedTypeChange();
			}
		}
		get {return selectedType;}
	}
	public delegate void SelectedIndexChangeAction();
	public event SelectedIndexChangeAction OnSelectedIndexChange;
	private int selectedIndex;
	public int SelectedIndex{
		set{
			if (selectedIndex>=0){
				itemSlots[selectedIndex].Selected=false;
			}
			itemSlots[value].Selected=true;
			selectedIndex=value;
			if (OnSelectedTypeChange!=null){
				OnSelectedTypeChange();
			}
		}
	}
	void Awake () {
		allCatergory.GetComponent<MouseOnButtonAnimation>().Selected=true;
		allCatergory.onClick.AddListener (delegate {
			SelectedType = ItemType.NULL;
		});
		weaponCatergory.onClick.AddListener (delegate {
			SelectedType = ItemType.WEAPON;
		});
		armorCatergory.onClick.AddListener (delegate {
			SelectedType = ItemType.ARMOR;
		});
		artifactCatergory.onClick.AddListener (delegate {
			SelectedType = ItemType.ARTIFACT;
		});
		if (itemLocation == ItemLocation.INVENTORY) {
			actionButtons[0].onClick.AddListener (delegate {
				Hero hero = GameManager.instance.CurrentGame.playerTroop.hero;
				int itemSlotIndex=0;
				ItemType type = hero.inventory.GetItem(selectedIndex, selectedType).type;
				for (int i=0; i<hero.equipmentSlots.Length; ++i){
					if (hero.equipmentSlots[i] == type){
						itemSlotIndex = i;
						break;
					}
				}
				if (hero.equipments.items[itemSlotIndex]!=null){
					hero.UnEquip(itemSlotIndex);
				}
				hero.Equip(selectedIndex, selectedType);
			});
			actionButtons[1].onClick.AddListener (delegate {
				GameManager.instance.SellItem(selectedIndex, selectedType);
			});
		}else if (itemLocation == ItemLocation.SHOP){
			actionButtons[0].onClick.AddListener (delegate {
				GameManager.instance.BuyItem(selectedIndex, selectedType);
				Refresh ();
			});
		}

		Refresh ();
	}
	void Start(){
		ItemSlotUI[] temp = GetComponentsInChildren<ItemSlotUI> ();
		itemSlots = new Dictionary<int, MouseOnButtonAnimation> ();
		for (int i=0; i<temp.Length; ++i) {
			itemSlots[temp[i].Index] = temp[i].GetComponent<MouseOnButtonAnimation>();
		}
	}
	void OnEnable(){
		OnSelectedIndexChange += Refresh;
		OnSelectedTypeChange += Refresh;

		if (itemLocation == ItemLocation.INVENTORY) {
			GameManager.instance.CurrentGame.playerTroop.hero.inventory.OnItemsChange +=Refresh;
		}else if (itemLocation == ItemLocation.SHOP){
			GameManager.instance.CurrentGame.shopItems.OnItemsChange +=Refresh;
		}

	}
	void OnDisable(){
		OnSelectedIndexChange -= Refresh;
		OnSelectedTypeChange -= Refresh;

		if (itemLocation == ItemLocation.INVENTORY) {
			GameManager.instance.CurrentGame.playerTroop.hero.inventory.OnItemsChange -=Refresh;
		}else if (itemLocation == ItemLocation.SHOP){
			GameManager.instance.CurrentGame.shopItems.OnItemsChange -=Refresh;
		}

	}
	private Button GetButtonByType(ItemType type){
		switch (type) {
		case ItemType.NULL:
			return allCatergory;
		case ItemType.ARMOR:
			return armorCatergory;
		case ItemType.ARTIFACT:
			return artifactCatergory;
		case ItemType.WEAPON:
			return weaponCatergory;
		}
		return null;
	}
	public void SelectCatergory(ItemType type){
		SelectedType = type;
	}
	public void Refresh(){
		bool isSelectedEmpty = true;
		if (itemLocation == ItemLocation.INVENTORY) {
			isSelectedEmpty = GameManager.instance.CurrentGame.playerTroop.hero.inventory.GetItem (selectedIndex, selectedType) == null;
		} else if (itemLocation == ItemLocation.SHOP) {
			isSelectedEmpty = GameManager.instance.CurrentGame.shopItems.GetItem (selectedIndex, selectedType) == null;
			if (!isSelectedEmpty){
				isSelectedEmpty = GameManager.instance.CurrentGame.gold < GameManager.instance.CurrentGame.shopItems.GetItem (selectedIndex, selectedType).goldValue;
			}
		}
		for (int i=0; i<actionButtons.Length; ++i) {
			actionButtons [i].interactable = !isSelectedEmpty;
		}
	}
}
