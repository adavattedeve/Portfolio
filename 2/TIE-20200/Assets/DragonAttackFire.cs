using UnityEngine;
using System.Collections;

public class DragonAttackFire : VisualEffectLauncher {
	public float waitBeforeHit=0.3f;
	public float waitAfterHit=1f;
	public override void Launch (Vector3 targetPosition, System.Action callBack)
	{
		projectileFinished = callBack;
		Invoke ("AfterHit", waitBeforeHit);
	}
	public override void AfterHit ()
	{
		projectileFinished ();
		Invoke ("EndEffect", waitAfterHit);
	}
	private void EndEffect(){
		gameObject.SetActive (false);
	}
}
