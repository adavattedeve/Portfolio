using UnityEngine;
using System.Collections;

public class FireExplosionVisual : VisualEffectLauncher {
	public float waitBeforeHit=0.3f;
	public float waitAfterHit=3f;
	public override void Launch (Vector3 targetPosition, System.Action callBack)
	{
		projectileFinished = callBack;
		targetPosition.y = 0;
		transform.position = targetPosition;
		Invoke ("AfterHit", waitBeforeHit);
	}
	public override void AfterHit ()
	{
		GraphicalEffectsManager.instance.ShakeCamera (VisualEffectSize.BIG);
		projectileFinished ();
		Invoke ("EndEffect", waitAfterHit);
	}
	private void EndEffect(){
		gameObject.SetActive (false);
	}
}
