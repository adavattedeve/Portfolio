using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIGear : MonoBehaviour {

	private Gear gear;
	UIEquipmentSlot[] guiSlots;
	private int openDescriptionIndex;
	private Image[] slots;
	private Image gearImage;
	private GameObject statPanel;
	private int mouseOnSlot=-1;
	public int mouseOnSlotIndex {
		get{ return mouseOnSlot;}
		set{mouseOnSlot=value;}
	}

	void Awake () {
		gearImage = GetComponent<Image> ();
		statPanel = GetComponentInChildren<StatTexts> ().gameObject;

	}
	void Start(){
		gear = PlayerManager.instance.Player.GetComponent<Gear> ();
		SetClickEvents ();
		ToggleView ();

		GlobalEvents.instance.OnGearChange += RefreshEquipment;
	}

	private void SetClickEvents (){

		guiSlots = GetComponentsInChildren<UIEquipmentSlot> ();
		Button[] slotButtons =new Button[guiSlots.Length]; 
		slots = new Image[guiSlots.Length];
		for (int i=0; i<guiSlots.Length; ++i) {
			for (int i2=0; i2<guiSlots.Length; ++i2) {
				if (guiSlots[i2].index==i){
					slots[i] = guiSlots[i2].GetComponent<Image>();
					guiSlots[i2].GetComponent<Button> ().onClick.AddListener(()=> {gear.UnEquip(i2);});
					break;
				}
			}
		}
	}
	public void RefreshEquipment(){
		for (int i=0; i<slots.Length; ++i) {
			if (gear.GearSlots[i].Item!=null){
				slots[i].sprite = GUIManager.instance.slotFull;
				guiSlots[i].itemImage.enabled=true;
				guiSlots[i].itemImage.sprite = gear.GearSlots[i].Item.Icon;
			}else {
				guiSlots[i].itemImage.enabled=false;
				slots[i].sprite= GUIManager.instance.slotEmpty;
			}
		}
		if (mouseOnSlotIndex >= 0) {
			GUIManager.instance.ItemInfoField (true, gear.GearSlots[mouseOnSlotIndex].Item);
		}
	}

	public void ToggleView(){
		if (gearImage.enabled) {
			gearImage.enabled = false;
			statPanel.SetActive(false);
			for (int i=0; i<slots.Length; ++i) {
				slots [i].enabled = false;
				guiSlots[i].itemImage.enabled=false;
			}
		} else {
			gearImage.enabled = true;
			statPanel.SetActive(true);
			for (int i=0; i<slots.Length; ++i) {
				slots [i].enabled = true;
				if (gear.GearSlots[i].Item!=null){
				guiSlots[i].itemImage.enabled=true;
				}
			}
		}
	}
	public IItem GetEquipment(ItemType type){
		for (int i=0; i<gear.GearSlots.Length; ++i){
			if (type == gear.GearSlots[i].typeAllowed){
				return gear.GearSlots[i].Item;
			}
		}
		return null;
	}
}
