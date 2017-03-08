using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Targets : MonoBehaviour {
	public LayerMask targetMask;
	public LayerMask notTargetMask;
	public float targetRange=25f;
	private List<GameObject> targets;
	private GameObject currentTarget;
	private int currentIndex;
	void Awake () {
		targets = new List<GameObject> ();
	}

	public void RefreshTargets(){
		Collider[] colliders = Physics.OverlapSphere (transform.position, targetRange, targetMask);
		List<int> used = new List<int>();
		targets.Clear ();
		for (int i=0; i<colliders.Length; ++i) {
			int closest = -1;
			float closestRange = 10000f;
			for (int ii=0; ii<colliders.Length; ++ii) {
				if (used.Contains(ii)){
					continue;
				}
				float range = Vector3.SqrMagnitude(colliders[ii].transform.position-transform.position);
				if (range<closestRange){
					closest = ii;
					closestRange = range;
				}
			}
			if (closest<0){
				break;
			}
			used.Add(closest);
			if (!Physics.Raycast(transform.position+ Vector3.up, colliders[closest].transform.position-transform.position, targetRange, notTargetMask)){
				targets.Add(colliders[closest].gameObject);
			}else {

				Debug.Log (name + " can't see: " + colliders[i].name);
			}
		}
//		for (int i=0; i<targets.Count; ++i) {
//			Debug.Log (Vector3.SqrMagnitude(targets[i].transform.position-transform.position) + "    " + targets[i].name);
//		}
	}
	public GameObject GetTarget() {
		RefreshTargets ();
		if (targets.Count > 0) {
			currentIndex = 0;
			currentTarget = targets [0];
		} else {
			currentTarget = null;
		}
		return currentTarget;
	}
	// 
	public GameObject ChangeTarget(int indexChange){

		RefreshTargets ();
		currentIndex = Mathf.Clamp (currentIndex, 0, targets.Count-1);
		if (targets.Count == 0){
			currentTarget = null;
			return null;
		}
		else if (indexChange > 0) {
			if (currentIndex < targets.Count - 1) {
				currentIndex++;
			} else {
				currentIndex = 0;
			}
		}
		else if (indexChange < 0) {
			if (currentIndex > 0) {
				currentIndex--;
			} else {
				currentIndex = targets.Count-1;;
			}
		}
		if (targets.Count > 1 && targets[currentIndex] == currentTarget) {
			return ChangeTarget(indexChange);
		}else {
			currentTarget = targets [currentIndex];
		}
		return currentTarget;
	}
}
