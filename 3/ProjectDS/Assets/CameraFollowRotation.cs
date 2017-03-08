using UnityEngine;
using System.Collections;

public class CameraFollowRotation : MonoBehaviour {

	public float rotationSpeed=1f;
	public float dampening = 0.35f;
	public float followRotationmaxAngle = 80;
	private float currentRotationSpeed;
	private Vector3 targetLastPos;

	public GameObject target;
	void Awake(){
		currentRotationSpeed = rotationSpeed;
		targetLastPos = target.transform.position;
	}

	void LateUpdate () {
		Vector3 translation = target.transform.position - targetLastPos;
		currentRotationSpeed = rotationSpeed;
		float angle = Vector3.Angle (transform.right, translation);
		if (Vector3.Cross (transform.right, translation).y < 0) {
			//angle*=-1;
			currentRotationSpeed /= 2;
			
		}
		bool left = false;
		bool right = false;
		if (angle < followRotationmaxAngle && angle > -followRotationmaxAngle) {
			right = true;
			
		}
		if (angle > 180-followRotationmaxAngle || angle < -(followRotationmaxAngle-120)) {
			left = true;
		}
		if (right) {
			//Mathf.SmoothDamp(
			transform.RotateAround (transform.position, Vector3.up, currentRotationSpeed*Time.deltaTime);
		} else if (left) {
			transform.RotateAround (transform.position, Vector3.up, -currentRotationSpeed*Time.deltaTime);
		}
		targetLastPos = target.transform.position;
	}
}
