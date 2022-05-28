using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.Animations;

public class EnemyPatrollingBehavior : StateMachineBehaviour
{
    EnemyController enemy;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<EnemyController>();
        DOTween.To(() => enemy.GetComponent<NavMeshAgent>().speed,
            x => enemy.GetComponent<NavMeshAgent>().speed = x,
            enemy.patrolSpeed, 1f);
        if (enemy.waypoints.Length > 0)
        {
            animator.GetComponent<NavMeshAgent>().SetDestination(enemy.waypoints[enemy.waypointIndex].transform.position);
            animator.GetComponent<NavMeshAgent>().speed = enemy.patrolSpeed;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(enemy.waypoints.Length > 0)
        {
            CheckPath(animator);
        }
    }



    void CheckPath(Animator animator)
    {
        if (animator.GetComponent<NavMeshAgent>().remainingDistance - animator.GetComponent<NavMeshAgent>().stoppingDistance <= 0)
        {
            if (enemy.waypoints[enemy.waypointIndex].GetComponent<WayPointController>().waitTime > 0)
            {
                animator.SetFloat("TimeToIdle", enemy.waypoints[enemy.waypointIndex].GetComponent<WayPointController>().waitTime);
            }

            enemy.waypointIndex += 1;
            if (enemy.waypointIndex >= enemy.waypoints.Length)
            {
                enemy.waypointIndex = 0;
            }
            animator.GetComponent<NavMeshAgent>().SetDestination(enemy.waypoints[enemy.waypointIndex].transform.position);
        }
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
