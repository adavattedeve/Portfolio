using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIInventory : MonoBehaviour {
	public int perRow;
	public float edgeOffsetX;
	public float edgeOffsetY;
	public float betweenX;
	public float betweenY;
	public float width;
	public float height;


	private Inventory inventory;
	private GUIGear guiGear;

	private RectTransform inventoryRect;
	private InventorySlot[] slots;
	//private InventorySlot[] slots;
	private Image inventoryImage;
	public Image mouseDragImage;
	public GameObject slotPrefab;
	delegate void UseItemOnClick(int i);

	private int itemGrabbedIndex;
	[System.NonSerialized]public int mouseOnSlotIndex = -1;
	[System.NonSerialized]public bool mouseOnConsumableSlot;

	void Awake () {
		guiGear = transform.parent.GetComponentInChildren<GUIGear> ();
	
		inventoryRect = GetComponent<RectTransform> ();
		itemGrabbedIndex = -1;
		mouseDragImage.enabled = false;
		inventoryImage = GetComponent<Image> ();
	}
	void Start(){
		inventory = PlayerManager.instance.Player.GetComponent<Inventory> ();
		GlobalEvents.instance.OnInventoryChange += RefreshInventory;
		CreateInventoryUI ();
		ToggleView ();
	}
	void Update (){
		if (inventoryImage.enabled) {
			if (Input.GetMouseButtonDown (0) && mouseOnSlotIndex >= 0 && inventory.GetItem (mouseOnSlotIndex) != null) {
				itemGrabbedIndex = mouseOnSlotIndex;
				mouseDragImage.sprite = inventory.GetItem (itemGrabbedIndex).Icon;
				mouseDragImage.enabled = true;
			} else if (Input.GetMouseButtonUp (0) && mouseOnSlotIndex < 0 && guiGear.mouseOnSlotIndex<0) {
				itemGrabbedIndex = -1;
				mouseDragImage.enabled = false;
			}
			if (itemGrabbedIndex >= 0 && mouseOnSlotIndex >= 0) {

				if (Input.GetMouseButtonUp (0)) {

					inventory.SwapItems (itemGrabbedIndex, mouseOnSlotIndex);
					mouseDragImage.enabled = false;
					itemGrabbedIndex = -1;
				}
			}
			if (itemGrabbedIndex >= 0 && guiGear.mouseOnSlotIndex>=0){
				if (Input.GetMouseButtonUp (0)) {
					if (inventory.GetItem(itemGrabbedIndex) is Equipment){
						Equipment temp = (Equipment) inventory.GetItem(itemGrabbedIndex);
						temp.Use (itemGrabbedIndex, guiGear.mouseOnSlotIndex);
					}
					mouseDragImage.enabled = false;
					itemGrabbedIndex = -1;
				}
			}
			if (itemGrabbedIndex >= 0 && mouseOnConsumableSlot){
				if (Input.GetMouseButtonUp (0)) {
					if (inventory.GetItem(itemGrabbedIndex) is Consumable){
						inventory.Consumable.Item = inventory.GetItem(itemGrabbedIndex);
						inventory.Consumable.index = itemGrabbedIndex;
					}
					mouseDragImage.enabled = false;
					itemGrabbedIndex = -1;
				}
			}
			if (itemGrabbedIndex >= 0) {
				mouseDragImage.transform.position = Input.mousePosition;
			}
		}
	}
	void CreateInventoryUI(){
		slots = new InventorySlot[inventory.inventorySize];
		for (int i=0; i< slots.Length; ++i) {
			GameObject newSlot = Instantiate (slotPrefab);
			InventorySlot slot = newSlot.GetComponent<InventorySlot>();
			slot.Index = i;
			RectTransform rect = newSlot.GetComponent<RectTransform>();
			rect.SetParent(inventoryRect, false);
			rect.anchorMax= new Vector2(((i%perRow)+1)*width+((i%perRow)+1)*betweenX + edgeOffsetX, 1-(i/perRow)*height-edgeOffsetY);
			rect.anchorMin= new Vector2((i%perRow)*width+ ((i%perRow)+1)*betweenX+edgeOffsetX, 1-((i/perRow)+1)*height-edgeOffsetY);
			rect.anchoredPosition3D=Vector3.zero;
			rect.offsetMax =Vector2.zero;
			slots[i] = slot;
			newSlot.GetComponent<Button>().onClick.AddListener(()=>{UseOnClick(slot.Index);});
		}
	}

	void UseOnClick(int index){
		if (inventory.GetItem (index) != null) {
			inventory.GetItem (index).Use (index);
		}
	}

	public void ToggleView(){
		if (inventoryImage.enabled) {
			inventoryImage.enabled = false;
			for (int i=0; i<slots.Length; ++i) {
				slots [i].slotImage.enabled = false;
				slots [i].itemImage.enabled = false;
			}
		} else {
			inventoryImage.enabled = true;
			for (int i=0; i<slots.Length; ++i) {
				slots [i].slotImage.enabled = true;
				if (inventory.GetItem(i)!=null){
					slots [i].itemImage.enabled = true;
				}
			}
		}
	}

	public void RefreshInventory(){
		for (int i=0; i< slots.Length; ++i) {
			if (inventory.GetItem(i)!=null){
				slots[i].slotImage.sprite = GUIManager.instance.slotFull;
				if (slots[i].slotImage.enabled){
					slots[i].itemImage.enabled=true;
				}
				slots[i].itemImage.sprite = inventory.GetItem(i).Icon;
			}
			else {
				slots[i].slotImage.sprite = GUIManager.instance.slotEmpty;
				slots[i].itemImage.enabled=false;
			}
		}
		if (mouseOnSlotIndex >= 0) {
			GUIManager.instance.ItemInfoField (true, inventory.GetItem (mouseOnSlotIndex));
		}
	}
}
