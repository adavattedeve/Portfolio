using UnityEngine;
using System.Collections;

public class CameraRotationHorizontal : MonoBehaviour {
	public float speed = 1f;
	public float dampening = 0.4f;
	private float h;
	private float currentAngle;
	private float targetAngle;
	public GameObject target;
	void Awake(){
		currentAngle = 0;
		targetAngle = 0;
	}
	void Update () {
		h = speed * Input.GetAxis("Mouse X")*Time.deltaTime;

	}
	void LateUpdate(){
		targetAngle += h;
		float deltaAngle = Mathf.Lerp (0, targetAngle, dampening);
		transform.RotateAround (transform.position, Vector3.up, deltaAngle);
		targetAngle-=deltaAngle;
		//currentAngle += deltaAngle;
	}
}
