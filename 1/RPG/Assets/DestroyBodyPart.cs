using UnityEngine;
using System.Collections;

public class DestroyBodyPart : MonoBehaviour {
	public float toggleColliderTime = 2.5f;
	public float destroyTime = 2.5f;
	public float speed = 1f;
	void OnEnable(){
		Invoke ("ToggleCollider", toggleColliderTime);
	}
	void OnDisable(){
		CancelInvoke ();
		StopAllCoroutines ();
	}
	void ToggleCollider(){
		Destroy (gameObject, destroyTime);
		GetComponent<Collider> ().enabled = false;
		GetComponent<Rigidbody> ().isKinematic = true;
		StartCoroutine (Fall());
	}
	IEnumerator Fall(){
		while (true) {
			transform.position -=Vector3.up*Time.deltaTime*speed;
			yield return null;
		}
	}
}
