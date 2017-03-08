using UnityEngine;
using System.Collections;

public class OpenChest : MonoBehaviour, IInteractable {
	Animator animator;
	LootSpawning loot;
	GameObject mainGO;

	public string outlineMaterialFolderPath;
	public string outlineMaterialName;
	Material outlineMaterial;
	Material originalMaterial;
	MeshRenderer[] renderers;
	public bool ReadyForInteract{
		set{
			if (value){
				for (int i=0; i<renderers.Length; ++i){
					renderers[i].material = outlineMaterial;}
			}else{
				for (int i=0; i<renderers.Length; ++i){
					renderers[i].sharedMaterial = originalMaterial;}
			}
		}
	}


	void Awake () {
		renderers = transform.parent.GetComponentsInChildren<MeshRenderer> ();
		mainGO = transform.parent.gameObject;
		animator = mainGO.GetComponent<Animator> ();
		loot = mainGO.GetComponent<LootSpawning> ();
		
	}
	void Start(){
		Material temp = Resources.Load (outlineMaterialFolderPath + outlineMaterialName, typeof(Material)) as Material;
		outlineMaterial = new Material (temp.shader);
		outlineMaterial.CopyPropertiesFromMaterial (temp);
		originalMaterial = renderers[0].sharedMaterial;
		outlineMaterial.SetTexture ("_MainTex", originalMaterial.GetTexture ("_MainTex"));
		outlineMaterial.SetTexture ("_BumpMap", originalMaterial.GetTexture ("_BumpMap"));
	}
//		Node[] nodes;
//		Vector3 center;
//		Grid grid = PlayerManager.instance.GetComponent<Grid> ();
//		Vector3 size;
//		Vector3[] edges;
//		BoxCollider coll = GetComponent<BoxCollider> ();
//		center = new Vector3 (coll.center.x, coll.center.y,  coll.center.z);
//		size = coll.bounds.extents;
//		edges = new Vector3[]{
//			new Vector3 (center.x + size.x * grid.occupiedRangeMultiplier, center.y + size.y * grid.occupiedRangeMultiplier,center.z ),
//			new Vector3 (center.x - size.x * grid.occupiedRangeMultiplier, center.y + size.y * grid.occupiedRangeMultiplier,center.z ),
//			new Vector3 (center.x + size.x * grid.occupiedRangeMultiplier, center.y - size.y * grid.occupiedRangeMultiplier,center.z ),
//			new Vector3 (center.x - size.x * grid.occupiedRangeMultiplier, center.y - size.y * grid.occupiedRangeMultiplier, center.z)
//			
//		};
//		
//		nodes = new Node[edges.Length];
//		for (int i=0; i<edges.Length; ++i) {
//			nodes[i] = grid.NodeFromWorldPoint (transform.TransformPoint (edges[i]));
//			nodes[i].walkable = false;
//		}
	public void Interact(){

		animator.SetTrigger ("Interact");
		loot.SpawnLoot ();
		Destroy (gameObject);
	}
}
