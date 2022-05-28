using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using DG.Tweening;
using UnityEngine;

public class EnemyChasingBehavior : StateMachineBehaviour
{
    EnemyController enemy;
    Vector3 playerPreviousPosition;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<NavMeshAgent>().SetDestination(PlayerController.player.transform.position);
        animator.GetComponent<NavMeshAgent>().speed = animator.GetComponent<EnemyController>().chaseSpeed;
        animator.GetComponent<NavMeshAgent>().isStopped = false;
        playerPreviousPosition = PlayerController.player.transform.position;

        enemy = animator.GetComponent<EnemyController>();

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetComponent<NavMeshAgent>().isStopped)
        {
            return;
        }

        if(PlayerController.player.transform.position != playerPreviousPosition)
        {
            animator.GetComponent<NavMeshAgent>().SetDestination(PlayerController.player.transform.position);
            playerPreviousPosition = PlayerController.player.transform.position;
        }

        if(animator.GetComponent<NavMeshAgent>().path == null)
        {
            animator.GetComponent<Transform>().LookAt(PlayerController.player.transform.position);
        }

        animator.gameObject.GetComponent<EnemyController>().enemy.GetComponent<EnemyAnimationContainer>().neck.transform.LookAt(PlayerController.player.transform.position);

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<NavMeshAgent>().SetDestination(animator.transform.position);
        animator.GetComponent<Transform>().LookAt(PlayerController.player.transform.position);
        //DOTween.To(() => animator.GetComponent<NavMeshAgent>().speed, x => animator.GetComponent<NavMeshAgent>().speed = x, 0f, 0.25f);
    }

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
