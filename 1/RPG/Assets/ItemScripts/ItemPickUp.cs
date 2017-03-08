using UnityEngine;
using System.Collections;

public class ItemPickUp : MonoBehaviour, IInteractable {
	private IItem item;
	public IItem Item {set{readyForPickUp = true;
			item = value;}}
	private bool readyForPickUp;
	private Collider coll;
	
	public string outlineMaterialFolderPath;
	public string outlineMaterialName;
	Material outlineMaterial;
	Material originalMaterial;
	MeshRenderer[] rends;
	public bool ReadyForInteract{
		set{
			if (value){
				for (int i=0; i<rends.Length; ++i){
					rends[i].material = outlineMaterial;}
			}else{
				for (int i=0; i<rends.Length; ++i){
					rends[i].sharedMaterial = originalMaterial;}
			}
		}
	}

	void Awake(){
		readyForPickUp = false;
		rends = GetComponentsInChildren<MeshRenderer> ();
		coll = GetComponent<CapsuleCollider> ();
		if (coll == null) {
			coll = GetComponent<BoxCollider> ();
		}
		if (coll == null) {
			Debug.Log ("Item does not have collider");
			Destroy(transform.parent.gameObject);
		}

	}
	void Start(){
		Material temp = Resources.Load (outlineMaterialFolderPath + outlineMaterialName, typeof(Material)) as Material;
		outlineMaterial = new Material (temp.shader);
		outlineMaterial.CopyPropertiesFromMaterial (temp);
		originalMaterial = rends[0].sharedMaterial;
		outlineMaterial.SetTexture ("_MainTex", originalMaterial.GetTexture ("_MainTex"));
		outlineMaterial.SetTexture ("_BumpMap", originalMaterial.GetTexture ("_BumpMap"));

	}

	public void Interact(){
		item.SetOwnerAndObjectReferences (PlayerManager.instance.Player.gameObject, transform.parent.gameObject);
		Inventory inventory = PlayerManager.instance.Player.GetComponent<Inventory> ();
		inventory.AddItem (item);
		this.gameObject.SetActive(false);
	}
}
