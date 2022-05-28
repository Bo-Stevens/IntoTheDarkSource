using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using DG.Tweening;
using UnityEngine;

public class EnemyAttackBehavior : StateMachineBehaviour
{
    public int attackCooldown;
    int attackTimer;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        attackTimer = attackCooldown;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log(animator.GetComponent<EnemyController>().enemy.GetComponent<EnemyAnimationContainer>().meleeAttackFinished);
        if (animator.GetComponent<EnemyController>().enemy.GetComponent<EnemyAnimationContainer>().meleeAttackFinished == true)
        {
            animator.GetComponent<EnemyController>().enemy.GetComponent<EnemyAnimationContainer>().meleeAttackFinished = false;
            Vector3 previousRotation = animator.transform.rotation.eulerAngles;
            Vector3 lookPos = animator.transform.position - PlayerController.player.transform.position;
            Vector3 rotation = Quaternion.LookRotation(lookPos).eulerAngles;
            //animator.transform.LookAt(PlayerController.player.transform.position);

            animator.transform.DORotate(new Vector3(previousRotation.x, rotation.y + 180f, previousRotation.z), 0.2f);
            animator.GetComponent<EnemyController>().enemy.GetComponent<Animator>().SetTrigger("Attacking");

            //animator.GetComponent<AudioSource>().PlayOneShot(animator.GetComponent<EnemyController>().enemy.GetComponent<EnemyAnimationContainer>().enemyAttackSound, 0.5f);

            attackTimer = 0;
        }
        else if(animator.GetComponent<EnemyController>().enemy.GetComponent<EnemyAnimationContainer>().meleeAttackFinished)
        {
            attackTimer += 1;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetComponentInParent<NavMeshAgent>() != null)
        {
            animator.GetComponentInParent<NavMeshAgent>().isStopped = false;
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
