using UnityEngine;
using System.Collections;

public class GlowingLight : MonoBehaviour {

	Light pointLight;
	bool raiseIntensity;
	float timer;
	[Header("FILL!")]
	public float upperIntensity;
	public float lowerIntensity;
	public float changeSpeed;
	public float intensityChange;
	void Awake(){
		pointLight = GetComponent<Light> ();
	}
	void Update(){
		if (timer > changeSpeed) {
			timer = 0;
			if (pointLight.intensity > upperIntensity) {
				raiseIntensity = false;
			} else if (pointLight.intensity < lowerIntensity) {
				raiseIntensity = true;
			}
			if (raiseIntensity) {
				RaiseIntensity (intensityChange);
			} else {
				LowerIntensity (intensityChange);
			}
		}
		timer += Time.deltaTime;
	}
	void RaiseIntensity(float change){
		pointLight.intensity += change;
	}
	void LowerIntensity(float change){
		pointLight.intensity -= change;
	}
}
