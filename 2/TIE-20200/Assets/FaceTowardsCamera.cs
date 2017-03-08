using UnityEngine;
using System.Collections;

public class FaceTowardsCamera : MonoBehaviour {
	Transform cameraTransform;
	// Use this for initialization
	void Start () {
		cameraTransform = Camera.main.transform;
	}
	
	// Update is called once per frame
	void Update () {
		transform.rotation = cameraTransform.rotation;
	}
}
