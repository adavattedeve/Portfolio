using UnityEngine;
using System.Collections;

public class IdleActionGenerator : StateMachineBehaviour {
	private float timer;
	public float timeBetweenIdleActions;
	public int maxIdleActions;
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		timer = 0;
	}
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		timer += Time.deltaTime;
		if (timer > timeBetweenIdleActions) {
			animator.SetInteger ("IdleAction", Random.Range (1, maxIdleActions + 1));
			timer = 0;
		} else {
			animator.SetInteger ("IdleAction", 0);
		}
	}
}
