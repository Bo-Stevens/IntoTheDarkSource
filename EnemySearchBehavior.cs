using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class EnemySearchBehavior : StateMachineBehaviour
{
    EnemyController enemy;
    int numLocationsVisited = 0;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        enemy = animator.GetComponent<EnemyController>();
        enemy.GetComponent<NavMeshAgent>().isStopped = false;
        enemy.GetComponent<NavMeshAgent>().SetDestination(enemy.playerLostKnownPos);
        enemy.GetComponent<AudioSource>().PlayOneShot(enemy.alertSounds[Random.Range(0, enemy.alertSounds.Length)]);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CheckPath(animator);
    }

    void CheckPath(Animator animator)
    {
        if(numLocationsVisited > 3)
        {
            animator.SetBool("Alerted", false);
        }
        if (animator.GetComponent<NavMeshAgent>().remainingDistance - animator.GetComponent<NavMeshAgent>().stoppingDistance <= 0)
        {
            Vector3 newPos = Utility.RandomNavPositionInRadius(animator.transform.position, 30f);
            animator.SetBool("GoingToLastKnownPos", false);
            animator.GetComponent<NavMeshAgent>().SetDestination(newPos);
            enemy.enemy.GetComponent<Animator>().SetTrigger("LookAround");
            enemy.GetComponent<NavMeshAgent>().isStopped = true;
            numLocationsVisited += 1;
        }
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
