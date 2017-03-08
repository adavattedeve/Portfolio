using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
	public GameObject target;
	private Vector3 offset;
	private Vector3 velocity=Vector3.zero;
	public float followSpeed = 50f;
	public float smoothTime=0.13f;

	void Awake(){
		offset = transform.position - target.transform.position;
	}
	void LateUpdate () {
		//Vector3 target = target.transform.position + offset;
		Vector3 newPos = Vector3.SmoothDamp (transform.position, target.transform.position + offset, ref velocity, smoothTime);
		transform.position = newPos;
	}
}
