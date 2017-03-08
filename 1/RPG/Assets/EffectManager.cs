using UnityEngine;
using System.Collections;

public enum EffectSize{SMALL, MEDIUM, LARGE}
public class EffectManager : MonoBehaviour {

	public static EffectManager instance;

	public string smallImpactSmoke = "ImpactSmokeSmall";
	public string mediumImpactSmoke = "ImpactSmokeMedium";
	public string largeImpactSmoke = "ImpactSmokeLarge";
	public string bloodDecal = "BloodDecal";
	void Awake(){
		if (instance == null) {
			instance = this;
		}
	}
	public void BloodDecal(Vector3 position){
		position.y += 0.01f;
		ObjectPool.instance.GetObjectForType (bloodDecal, false).transform.position = position;
	}
	public void ImpactSmoke(Vector3 position, EffectSize size){
		switch(size){
		case EffectSize.SMALL:
			ObjectPool.instance.GetObjectForType (smallImpactSmoke, false).transform.position = position;
			break;
		case EffectSize.MEDIUM:
			ObjectPool.instance.GetObjectForType (mediumImpactSmoke, false).transform.position = position;
			break;
		case EffectSize.LARGE:
			ObjectPool.instance.GetObjectForType (largeImpactSmoke, false).transform.position = position;
			break;
		}
	}
	public float amplitude;
	public int shakeTimes;
	public void CameraShake(){

		StopCoroutine (RandomMovement(Camera.main.transform));
		StartCoroutine (RandomMovement(Camera.main.transform));
	}
	IEnumerator RandomMovement(Transform target){
		int counter = 0;
		while (true) {
			target.position+=new Vector3 (Random.Range(-amplitude, amplitude),Random.Range(-amplitude, amplitude),Random.Range(-amplitude, amplitude));
			yield return new WaitForSeconds (0.05f);
			++counter;
			if (counter>shakeTimes){
				break;
			}

		}

	}
}
