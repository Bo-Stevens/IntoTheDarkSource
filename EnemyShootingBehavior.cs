using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShootingBehavior : StateMachineBehaviour
{

    public float shotTimer = 0f;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<EnemyController>().enemy.GetComponent<Animator>().SetTrigger("Attacking");
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        shotTimer += Time.deltaTime;

        if(shotTimer >= 3f)
        {
            animator.GetComponent<EnemyController>().enemy.GetComponent<Animator>().SetTrigger("Attacking");
            shotTimer = 0f;
        }

        //animator.transform.LookAt(PlayerController.player.gameObject.transform);
        animator.GetComponentInParent<ProjectileEnemyController>().playerTracker.transform.LookAt(PlayerController.player.gameObject.transform);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
