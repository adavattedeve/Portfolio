using UnityEngine;
using System.Collections;
public enum VisualEffectSize{NULL ,SMALL, BIG}
public class GraphicalEffectsManager : MonoBehaviour {
	public static GraphicalEffectsManager instance;

	[Header("CAMERA SHAKE PARAMS")]
	private VisualEffectSize currentShake;
	public float comeBackSpeed = 3f;
	[Header("small shake")]
	public float tSmall=0.5f;
	public float aSmall = 5;
	public float fSmall = 0.025f;
	[Header("big shake")]
	public float tBig = 0.65f;
	public float aBig = 10f;
	public float fBig = 0.025f;
	void Awake(){
		if (instance == null) {
			instance = this;
		}
	}
	void OnLevelWasLoaded(int level){
		StopAllCoroutines ();
	}
	//float t, float amplitude, float frequency
	public void ShakeCamera(VisualEffectSize size){

		if (size < currentShake) {
			return;
		}
		StopAllCoroutines ();
		currentShake = size;
		switch (size) {
		case VisualEffectSize.SMALL:
			StartCoroutine (Shake (tSmall, aSmall, fSmall));
			break;
		case VisualEffectSize.BIG:
			StartCoroutine (Shake (tBig, aBig, fBig));
			break;
		}
	}

	private IEnumerator Shake(float _t, float _amplitude, float _frequency){
		Transform trans = Camera.main.transform;
		Vector3 deltaPosition = Vector3.zero;
		Vector3 movement=Vector3.zero;
		float timeFromDirectionChange = 0;
		float timeFromStart=0;
		while (true) {
			if (timeFromDirectionChange>=_frequency){
				movement = new Vector3(Random.Range(-_amplitude, _amplitude), Random.Range(-_amplitude, _amplitude), Random.Range(-_amplitude, _amplitude));
			}
			yield return new WaitForEndOfFrame();
			trans.position += movement*Time.deltaTime;
			deltaPosition += movement*Time.deltaTime;
			timeFromDirectionChange+=Time.deltaTime;
			timeFromStart +=Time.deltaTime;
			if (timeFromStart>=_t){
				break;
			}
		}
		while (true) {
			yield return new WaitForEndOfFrame();
			trans.position -= deltaPosition *Time.deltaTime*comeBackSpeed;
			deltaPosition -= deltaPosition *Time.deltaTime*comeBackSpeed;
			if ((Vector3.SqrMagnitude(deltaPosition)<= 0.2f*0.2f) || timeFromStart>_t*1.5f){
				break;
			}
		}
		currentShake = VisualEffectSize.NULL;
	}
}
