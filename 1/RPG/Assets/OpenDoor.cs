using UnityEngine;
using System.Collections;

public class OpenDoor : MonoBehaviour, IInteractable {
	Animator animator;
	Node[] nodes;
	GameObject mainGO;

	public string outlineMaterialFolderPath;
	public string outlineMaterialName;
	Material outlineMaterial;
	Material originalMaterial;
	MeshRenderer rend;
	public bool ReadyForInteract{
		set{
			if (value){
				rend.material = outlineMaterial;
			}else{
				rend.sharedMaterial = originalMaterial;
			}
		}
	}


	void Awake () {
		rend = GetComponentInParent<MeshRenderer> ();
		mainGO = transform.parent.gameObject;
		animator = mainGO.GetComponent<Animator> ();

	}
	void Start(){
		Material temp = Resources.Load (outlineMaterialFolderPath + outlineMaterialName, typeof(Material)) as Material;
		outlineMaterial = new Material (temp.shader);
		outlineMaterial.CopyPropertiesFromMaterial (temp);

		Vector3 center;
		Vector3 size;
		Vector3[] edges;
		BoxCollider coll = mainGO.GetComponent<BoxCollider> ();
		center = new Vector3 (coll.center.x, coll.center.y,  coll.center.z);
		size = coll.bounds.extents;
		edges = new Vector3[]{
			new Vector3 (center.x + size.x * 0.9f, center.y),
			new Vector3 (center.x - size.x * 0.9f, center.y)
		};
		Grid grid = PlayerManager.instance.GetComponent<Grid> ();
		nodes = new Node[edges.Length];
		for (int i=0; i<edges.Length; ++i) {
			nodes[i] = grid.NodeFromWorldPoint (transform.TransformPoint (edges[i]));
			nodes[i].occupied = true;
		}
		originalMaterial = rend.sharedMaterial;
		outlineMaterial.SetTexture ("_MainTex", originalMaterial.GetTexture ("_MainTex"));
		outlineMaterial.SetTexture ("_BumpMap", originalMaterial.GetTexture ("_BumpMap"));
	}
	public void Interact(){
		for (int i=0; i<nodes.Length; ++i) {
			nodes[i].occupied = false;
		}
		animator.SetTrigger ("Interact");
		Destroy (gameObject);
	}
}
