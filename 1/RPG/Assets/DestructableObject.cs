using UnityEngine;
using System.Collections;

public class DestructableObject : MonoBehaviour, IDamageable {

	GameObject afterDestruction;
	Node[] nodes;
	Vector3 center;

	void Start () {
		Grid grid = PlayerManager.instance.GetComponent<Grid> ();
		Vector3 center;
		Vector3 size;
		Vector3[] edges;
		BoxCollider coll = GetComponent<BoxCollider> ();
		center = new Vector3 (coll.center.x, coll.center.y,  coll.center.z);
		size = coll.bounds.extents;
		edges = new Vector3[]{
			new Vector3 (center.x + size.x * grid.occupiedRangeMultiplier, center.y + size.y * grid.occupiedRangeMultiplier,center.z ),
			new Vector3 (center.x - size.x * grid.occupiedRangeMultiplier, center.y + size.y * grid.occupiedRangeMultiplier,center.z ),
			new Vector3 (center.x + size.x * grid.occupiedRangeMultiplier, center.y - size.y * grid.occupiedRangeMultiplier,center.z ),
			new Vector3 (center.x - size.x * grid.occupiedRangeMultiplier, center.y - size.y * grid.occupiedRangeMultiplier, center.z)

		};

		nodes = new Node[edges.Length];
		for (int i=0; i<edges.Length; ++i) {
			nodes[i] = grid.NodeFromWorldPoint (transform.TransformPoint (edges[i]));
			nodes[i].occupied = true;
		}

		afterDestruction = GetComponentsInChildren<Transform> ()[1].gameObject;
		afterDestruction.SetActive (false);
	}

	public void TakeDamage(HitInfo hit){
		for (int i=0; i<nodes.Length; ++i) {
			nodes[i].occupied = false;
		}
		afterDestruction.SetActive (true);
		afterDestruction.transform.SetParent (transform.parent);
		EffectManager.instance.ImpactSmoke (transform.position, EffectSize.MEDIUM);
		Destroy (gameObject);
	}
	

}
