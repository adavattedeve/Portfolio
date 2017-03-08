using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
	private Transform target;
	[Range(0f, 1f)]public float smoothnes;
	public float forwardMpl;
	private Vector3 offSet;
	private Vector3 targetPos;

	void Start(){
		target = PlayerManager.instance.Player.transform;
		offSet =  transform.position - target.position ;
		targetPos.y = transform.position.y;
	}

	void LateUpdate () {
		if (target != null) {
			targetPos.x = Mathf.Lerp (transform.position.x, target.position.x + offSet.x+ (target.forward*forwardMpl).x, smoothnes);
			targetPos.z = Mathf.Lerp (transform.position.z, target.position.z + offSet.z+ (target.forward*forwardMpl).z, smoothnes);
			transform.position = targetPos;
		}
	}
}
