using UnityEngine;
using System.Collections;

public class CameraRotationAndMovement : MonoBehaviour {
	[SerializeField] private float movementSmoothing=0.25f;
	[SerializeField] private float maxPosX=15;
	[SerializeField] private float minPosX=-15;
	[SerializeField] private float maxPosZ=25;
	[SerializeField] private float minPosZ=-15;
	[SerializeField] private float moveSpeed=5;
	[SerializeField] private float rotationSmoothing=0.125f;
	[SerializeField] private float maxRotation=90;
	[SerializeField] private float rotationSpeed=2000;

	private Vector3 targetPosition;
	private Vector3 deltaPosition;
	private float rotated=0;
	private float targetRotation=0;
	private float deltaRotation=0;
	void Awake(){
		targetPosition = transform.position;
	}
	void LateUpdate () {
		if (Input.GetMouseButton (2)) {
			float mouseX = Input.GetAxis("Mouse X");
			if (mouseX !=0){
				if (!(targetRotation+Time.deltaTime*mouseX*rotationSpeed>maxRotation || targetRotation+Time.deltaTime*mouseX*rotationSpeed<-maxRotation)){
					targetRotation += Time.deltaTime*mouseX*rotationSpeed;
				}
			}
		}
		//Rotation
		deltaRotation = Mathf.Lerp(rotated, targetRotation, rotationSmoothing)-rotated;
		rotated += deltaRotation;
		transform.Rotate(Vector3.up, deltaRotation);
		//Movement
		float h = Input.GetAxisRaw ("Horizontal");
		float v  = Input.GetAxisRaw ("Vertical");
		deltaPosition = transform.right*h * Time.deltaTime * moveSpeed + transform.forward*v * Time.deltaTime * moveSpeed;
		if (targetPosition.x + deltaPosition.x >= maxPosX || targetPosition.x + deltaPosition.x <= minPosX) {
			deltaPosition.x = 0;
		}
		if (targetPosition.z + deltaPosition.z >= maxPosZ || targetPosition.z + deltaPosition.z <= minPosZ) {
			deltaPosition.z = 0;
		}
		targetPosition += deltaPosition;
		Vector3 position = transform.position;
		position.x = Mathf.Lerp (position.x, targetPosition.x, movementSmoothing);
		position.z = Mathf.Lerp (position.z, targetPosition.z, movementSmoothing);
		transform.position = position;
	}
}
