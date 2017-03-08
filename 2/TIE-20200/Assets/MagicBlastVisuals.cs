using UnityEngine;
using System.Collections;

public class MagicBlastVisuals : VisualEffectLauncher {
	public float waitAfterHit=2f;
	public GameObject projectile;
	public GameObject onHitEffect;
	public override void Launch (Vector3 targetPosition, System.Action callBack)
	{
		base.Launch (targetPosition, callBack);
		projectile.SetActive (true);
		onHitEffect.SetActive (false);
	}
	public override void AfterHit ()
	{
		projectile.SetActive (false);
		onHitEffect.SetActive (true);
		
		Invoke ("EndEffect", waitAfterHit);
	}
	private void EndEffect(){
		projectile.SetActive (true);
		onHitEffect.SetActive (false);
		gameObject.SetActive (false);
	}

}
