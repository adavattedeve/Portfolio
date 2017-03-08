using UnityEngine;
using System.Collections;

public class AnimatorEnemyAttack : StateMachineBehaviour {
    [Range(0f, 1f)]
    public float executeAttackAt = 0.4f;
    [Range(0f, 1f)]
    public float rotateUntil = 0.3f;
    public float rotationSmoothing = 0.1f;

    private bool attackExecuted = false;
    private EnemyController enemyController;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackExecuted = false;
        if (enemyController == null)
        {
            enemyController = animator.GetComponent<EnemyController>();
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!attackExecuted && stateInfo.normalizedTime >= executeAttackAt)
        {
            enemyController.ExecuteAttackLogic();
            attackExecuted = true;
        }

        if (rotateUntil < stateInfo.normalizedTime)
        {
            Quaternion targetRotation = Quaternion.LookRotation(enemyController.Target.position - animator.transform.position, Vector3.up);
            animator.transform.rotation = Quaternion.Lerp(animator.transform.rotation, targetRotation, rotationSmoothing);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
       // enemyController.ControlsEnabled = true;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
