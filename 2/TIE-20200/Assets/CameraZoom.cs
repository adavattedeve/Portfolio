using UnityEngine;
using System.Collections;

public class CameraZoom : MonoBehaviour {
	private Camera camera;
	[SerializeField] private float zoomSmoothing=0.1f;
	[SerializeField] private float zoomSpeed=500;
	[SerializeField] private float defaultFov=60;
	[SerializeField] private float minFov=15;
	[SerializeField] private float maxFov=60;

	private float mouseScroll;
	private float targetFov;
	void Awake(){
		camera = GetComponent<Camera> ();
		camera.fieldOfView = defaultFov;
		targetFov = maxFov;
	}
	void Update () {
		mouseScroll = -1*Input.GetAxis ("Mouse ScrollWheel");
		targetFov = Mathf.Clamp (targetFov + Time.deltaTime * mouseScroll * zoomSpeed, minFov, maxFov);
		camera.fieldOfView =Mathf.Lerp(camera.fieldOfView, targetFov, zoomSmoothing);
	}
}
