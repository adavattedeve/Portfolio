using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
public class UIEquipmentSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	GUIGear guiGear;
	public int index;
	public Image itemImage;

	void Start () {
		guiGear = GetComponentInParent<GUIGear> ();
		itemImage.enabled =false;
	}
	public void OnPointerEnter (PointerEventData eventData){
		GUIManager.instance.ItemInfoField (true, GUIManager.instance.Gear.GearSlots [index].Item);
		guiGear.mouseOnSlotIndex = index;
	}
	
	public void OnPointerExit (PointerEventData eventData){
		GUIManager.instance.ItemInfoField (false, null);
		guiGear.mouseOnSlotIndex = -1;
	}

}
