using UnityEngine;
using System.Collections;

public class CameraRotation : MonoBehaviour {
	public float hSpeed=1f;
	public float vSpeed = 1f;
	private float h;
	private float v;
	public GameObject target;

	public float rotationSpeed=1f;
	public float followRotationmaxAngle = 80;
	private float currentRotationSpeed;
	private Vector3 targetLastPos;

	void Awake(){
		currentRotationSpeed = rotationSpeed;
		targetLastPos = target.transform.position;
	}
	// Update is called once per frame
	void Update () {
		h = hSpeed * Input.GetAxis("Mouse X");
		v = -vSpeed * Input.GetAxis("Mouse Y");
	
	}
	void LateUpdate(){
		if (h == 0 && v == 0) {
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
				transform.RotateAround (transform.position, Vector3.up, currentRotationSpeed);
			} else if (left) {
				transform.RotateAround (transform.position, Vector3.up, -currentRotationSpeed);
			}
		} else {
			transform.RotateAround (transform.position, Vector3.up, h);
			transform.RotateAround (transform.position, transform.right, v);
		}
		targetLastPos = target.transform.position;
	}
}
