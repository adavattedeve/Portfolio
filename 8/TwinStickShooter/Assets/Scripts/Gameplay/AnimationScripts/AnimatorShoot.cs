using UnityEngine;
using System.Collections;

public class AnimatorShoot : StateMachineBehaviour {
    [Range(0f, 1f)]
    public float time = 0f;
    private WeaponController weaponControl;
    private bool enabled = false;
    bool triggered;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        
        enabled = animator.GetLayerWeight(layerIndex) == 1;
        if (!enabled)
            return;
        if (weaponControl == null) {
            weaponControl = animator.transform.parent.GetComponent<WeaponController>();
        }
        triggered = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!enabled)
            return;
        
	    if (stateInfo.normalizedTime >= time && !triggered) {
            weaponControl.Shoot();
            triggered = true;
        }
	}
}
