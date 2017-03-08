using UnityEngine;
using System.Collections;
using System;

public class VisualEffectLauncher : MonoBehaviour {
	public float speed=10f;
	public float errorRange=0.3f;
	protected Action projectileFinished;
	public virtual void Launch(Vector3 targetPosition, Action callBack){
		transform.parent = null;
		projectileFinished = callBack;
		StartCoroutine (travel (targetPosition));
	}
	protected IEnumerator travel(Vector3 target){
		transform.LookAt(target);
		Vector3 dir = transform.forward;
		while (true) {
			transform.LookAt(target);
			if (transform.forward.x*dir.x<0 || transform.forward.z*dir.z<0){
				projectileFinished();
				AfterHit ();
				break;
			}
			transform.position+=transform.forward*Time.deltaTime*speed;
			yield return new WaitForEndOfFrame();
			if (Vector3.SqrMagnitude(target-transform.position)<errorRange*errorRange){
				projectileFinished();
				AfterHit ();
				break;
			}
		}
	}
	public virtual void AfterHit(){
		gameObject.SetActive (false);
	}
}
