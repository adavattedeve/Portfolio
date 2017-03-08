using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	public Image itemImage;
	[System.NonSerialized] public Image slotImage;
	GUIInventory guiInventory; 
	private int index;
	GUIGear guiGear;
	public int Index{ get{return index;}set{ index = value; }}

	void Awake(){
		slotImage = GetComponent<Image> ();
	}
	void Start () {
		guiInventory = GetComponentInParent<GUIInventory> ();
		guiGear = guiInventory.transform.parent.GetComponentInChildren<GUIGear> ();
		itemImage.enabled =false;
	}
	public void OnPointerEnter (PointerEventData eventData){
		GUIManager.instance.ItemInfoField (true, GUIManager.instance.Inventory.GetItem (index));
		guiInventory.mouseOnSlotIndex = index;
	}

	public void OnPointerExit (PointerEventData eventData){
		guiInventory.mouseOnSlotIndex = -1;
		GUIManager.instance.ItemInfoField (false, null);
	}
}
