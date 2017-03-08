using UnityEngine;
using System.Collections;

public class LockOnMovement : MonoBehaviour {
	public float speed=1;
	private Rigidbody rB;
	// Use this for initialization
	void Awake () {
		rB = GetComponent<Rigidbody> ();
	}

	public void Move (Vector3 direction, float bigger, GameObject target){
		rB.AddRelativeForce (direction.normalized* bigger * speed*Time.fixedDeltaTime);
		Rotate (target);
	}
	private void Rotate(GameObject target){
		Vector3 targetDirection = (target.transform.position - transform.position);
		targetDirection.y = 0;
		rB.MoveRotation(Quaternion.LookRotation(targetDirection));
	}
}
