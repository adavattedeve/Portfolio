using UnityEngine;
using System.Collections;

public class CameraLockOnRotation : MonoBehaviour {
	public GameObject player;
	private PlayerControl playerControl;

	public float rotationSpeed=5;
	public float dampening=0.3f;
	void Awake(){
		playerControl = player.GetComponent<PlayerControl> (); 
	}
	void LateUpdate () {
		Vector3 targetDirection = playerControl.LockOnTarget.transform.position - player.transform.position;
		targetDirection.y = 0; 
		Vector3 currentDirection = transform.forward;
		currentDirection.y = 0;
		float angle = Vector3.Angle (currentDirection, targetDirection);
		if (Vector3.Cross (currentDirection, targetDirection).y < 0) {
			angle *= -1;
		}
		//targetAngle += angle;
		transform.Rotate (0, Time.deltaTime*angle * dampening*rotationSpeed, 0, Space.World);
		//targetAngle -= Time.deltaTime*targetAngle*dampening;
	}
}
