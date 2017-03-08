using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {
	public float speed=30;
	private Rigidbody rB;
	void Awake () {
		rB = GetComponent<Rigidbody> ();
	}

	public void Move(Vector3 direction, float bigger){
		if (direction == Vector3.zero) {
			Rotate (Vector3.zero);
			return;
		}
		Rotate (direction);
		Debug.Log ("Force: " + speed*bigger*Time.deltaTime);
		rB.AddRelativeForce (Vector3.forward*speed*bigger*Time.fixedDeltaTime);

	}
	private void Rotate(Vector3 direction){
		if (direction == Vector3.zero) {
			rB.MoveRotation(Quaternion.LookRotation(transform.forward));
			return;
		}
		rB.MoveRotation(Quaternion.LookRotation(direction));
	}
}
