using UnityEngine;
using System.Collections;

public class CameraRotationVertical : MonoBehaviour {
	public float speed = 1f;
	public Vector2 angleRange;
	private float v;
	public GameObject target;
	private float angle;
	void Awake(){
		Vector3 forward = transform.forward;
		forward.y = 0;
		transform.rotation = Quaternion.LookRotation (forward);
		angle = 0;
	}
	void Update () {
		v = -speed * Input.GetAxis("Mouse Y")*Time.deltaTime;
	}
	void LateUpdate(){
		Vector3 forward = transform.forward;
		forward.y = 0;
		if ((angle < angleRange.y && v>0) || (angle > angleRange.x && v<0)) {
			transform.RotateAround (transform.position, transform.right, v);
			angle +=v;
		}
	}
}
