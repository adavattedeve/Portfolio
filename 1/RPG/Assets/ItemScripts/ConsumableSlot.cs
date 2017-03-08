using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
public class ConsumableSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	GUIInventory guiInventory; 
	Image slotImage;
	public Image itemImage;
	void Start () {
		slotImage = GetComponent<Image> ();
		GlobalEvents.instance.OnInventoryChange += RefreshSlot;
		RefreshSlot ();
	}
	
	public void OnPointerEnter (PointerEventData eventData){
		GUIManager.instance.ItemInfoField (true, GUIManager.instance.Inventory.Consumable.Item);
		GUIManager.instance.GuiInventory.mouseOnConsumableSlot = true;
	}
	
	public void OnPointerExit (PointerEventData eventData){
		GUIManager.instance.GuiInventory.mouseOnConsumableSlot = false;
			GUIManager.instance.ItemInfoField (false, null);
	}

	public void RefreshSlot(){
		if (GUIManager.instance.Inventory.Consumable.Item != null) {
				itemImage.sprite = GUIManager.instance.Inventory.Consumable.Item.Icon;
				itemImage.enabled=true;
				slotImage.sprite = GUIManager.instance.slotFull;
		} else {
				itemImage.enabled=false;
				slotImage.sprite = GUIManager.instance.slotEmpty;
		}
	}
}
