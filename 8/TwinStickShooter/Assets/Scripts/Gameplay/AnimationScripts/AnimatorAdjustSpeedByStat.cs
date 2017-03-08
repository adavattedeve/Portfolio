using UnityEngine;
using System.Collections;

public class AnimatorAdjustSpeedByStat : StateMachineBehaviour
{
    [SerializeField]
    private StatType type;
    private float baseSpeed = -1;
    private Stat stat;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stat == null)
        {
            CharacterStats stats = (CharacterStats)animator.GetComponent(typeof(CharacterStats));
            stat = stats.GetStat(type);
        }
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