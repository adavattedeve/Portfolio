using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour {
	private Inventory inventory;
	private ActionBuffer buffer;
	private Animator animator;
	private Ray ray;
	private RaycastHit hit;
	private Vector3 tempVector;
	private int layerMask;
	private AbilityManager aManager;
	private Gear gear;
	private Interact interactor;
	void Start () {
		animator = GetComponent<Animator> ();
		buffer = GetComponent<ActionBuffer> ();
		layerMask = LayerMask.GetMask ("Input");
		aManager = GetComponent<AbilityManager> ();
		gear = GetComponent<Gear> ();
		interactor = GetComponent<Interact> ();
		inventory = GetComponent<Inventory> ();
	}

	void Update () {
		if (!GUIManager.instance.GuiOpen) {
			if (Input.GetButtonDown ("Roll")) {
				ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				if (Physics.Raycast (ray, out hit, 100, layerMask)) {
					tempVector = hit.point;
					tempVector.y = 0;
					buffer.AddToBuffer (1, (tempVector - transform.position));
				}
			}
			if (Input.GetButtonDown ("BasicAttack")) {
				if (gear.GetEquipment (ItemType.WEAPON) != null) {
					ray = Camera.main.ScreenPointToRay (Input.mousePosition);
					if (Physics.Raycast (ray, out hit, 100, layerMask)) {
						tempVector = hit.point;
						tempVector.y = 0;
						buffer.AddToBuffer (aManager.EquippedAbilities [0].ActionID, (tempVector - transform.position));
					}
				}
			}
		}
		animator.SetFloat ("h", Input.GetAxisRaw("Horizontal"));
		animator.SetFloat ("v", Input.GetAxisRaw ("Vertical"));

	
		if (Input.GetButtonDown("Consumable")) {
			inventory.UseConsumable();
		}
		if (Input.GetButtonDown("Inventory")) {
			GUIManager.instance.ToggleInventory();
		}
		if (Input.GetButtonDown ("Interact")) {
			interactor.InteractFirst();
		}
	}
}
