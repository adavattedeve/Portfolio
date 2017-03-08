using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour {

	public GameObject descriptionFieldPrefab;
	private ItemInfoField itemDescription1;
	private ItemInfoField itemDescription2;

	public Sprite slotEmpty;
	public Sprite slotFull;












	private InputController input;
	private GUIInventory guiInventory;
	public GUIInventory GuiInventory{
		get{
			if (!guiInventory) {
				guiInventory = GameObject.FindObjectOfType(typeof(GUIInventory)) as GUIInventory;
			} 
			if (guiInventory) {
				return guiInventory;
			}
			return null;
		}
	}
	private Inventory inventory;
	public Inventory Inventory{
		get{
			if (!inventory) {
				inventory = GameObject.FindObjectOfType(typeof(Inventory)) as Inventory;
			} 
			if (inventory) {
				return inventory;
			}
			return null;
		}
	}

	private GUIGear guiGear;
	private Gear gear;
	public Gear Gear{
		get{
			if (!gear) {
				gear = GameObject.FindObjectOfType(typeof(Gear)) as Gear;
			} 
			if (gear) {
				return gear;
			}
			return null;
		}
	}

	private GameObject menu;
	private bool guiInventoryOpen;
	private bool guiGearOpen;
	private bool menuOpen;
	public bool GuiOpen {get{return menuOpen || guiInventoryOpen || guiGearOpen;}}

	public static GUIManager instance;
	void Awake () {
		if (!instance) {
			instance = this;
		}

	}
	void Start(){

	}
	void OnLevelWasLoaded(int level){
		if (level != 0) {
			Menu temp = GameObject.FindObjectOfType(typeof(Menu)) as Menu;
			if (temp){
				menu = temp.gameObject;
				menu.SetActive (false);
			}
			guiInventoryOpen = false;
			guiGearOpen = false;
			menuOpen = false;
		}
	}

	public void ToggleInventory (){
		if (!guiInventory) {
			guiInventory = GameObject.FindObjectOfType(typeof(GUIInventory)) as GUIInventory;
		} 
		if (guiInventory) {
			guiInventory.ToggleView ();
		}
		if (!guiGear) {
			guiGear = GameObject.FindObjectOfType(typeof(GUIGear)) as GUIGear;
		}
		if (guiGear) {
			guiGear.ToggleView ();
		}
		guiInventoryOpen = !guiInventoryOpen;
		guiGearOpen = !guiGearOpen;
	}

	public void ToggleMenu(){

		if (menu.activeSelf) {
			menu.SetActive (false);
		} else {
			menu.SetActive(true);
		}
		menuOpen = !menuOpen;
	}

	public void ItemInfoField(bool on, IItem item){
		if (!itemDescription1) {
			itemDescription1 = (Instantiate (descriptionFieldPrefab) as GameObject).GetComponent<ItemInfoField> ();
			itemDescription1.GetComponent<RectTransform>().SetParent(GuiInventory.GetComponent<RectTransform>().parent);
			itemDescription2 = (Instantiate (descriptionFieldPrefab) as GameObject).GetComponent<ItemInfoField> ();
			itemDescription2.GetComponent<RectTransform>().SetParent(GuiInventory.GetComponent<RectTransform>().parent);
			itemDescription1.gameObject.SetActive(false);
			itemDescription2.gameObject.SetActive(false);
		}
		if (on && item!=null) {
			if (!itemDescription1.gameObject.activeSelf){
				itemDescription1.gameObject.SetActive(true);
			}
			float xPositionForNext = itemDescription1.DescriptionOn (Input.mousePosition, item);
			if (item is Equipment ){
				IItem temp = guiGear.GetEquipment(item.Type);
				if (temp!=null && temp!=item){
					itemDescription2.gameObject.SetActive(true);
					itemDescription2.DescriptionOn (new Vector3(xPositionForNext, Input.mousePosition.y, Input.mousePosition.z), temp);
				}
			}
		} else {
			itemDescription1.gameObject.SetActive(false);
			itemDescription2.gameObject.SetActive(false);
		}
	}

	void Update(){
		if (Input.GetButtonDown ("Menu")) {
			ToggleMenu();
		}
	}
}
