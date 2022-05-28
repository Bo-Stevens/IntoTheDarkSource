using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class EnemyVisionController : StateMachineBehaviour
{
    float aggroTimer;
    EnemyController enemy;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<EnemyController>();
    }


    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
/*        if (animator.GetBool("PlayerInSight"))
        {
            aggroTimer += Time.deltaTime;
            CheckPlayerWithinAttackRange(animator);
        }
        SearchForPlayer(animator);*/
    }

    void SearchForPlayer(Animator animator)
    {
/*        aggroTimer += Time.deltaTime;
        GameObject enemyHead = enemy.enemy.GetComponent<EnemyAnimationContainer>().head;
        Debug.DrawLine(enemyHead.transform.position + (animator.transform.forward/4f), enemyHead.transform.position + (Vector3.Normalize(PlayerController.player.transform.position - enemyHead.gameObject.transform.position ) * enemy.visionDistance), Color.green);
        Vector3 playerDir = Vector3.Normalize(PlayerController.player.transform.position - animator.transform.position);
        RaycastHit hit;
        Physics.Raycast(enemyHead.transform.position + (animator.transform.forward / 4f), Vector3.Normalize(PlayerController.player.transform.position - enemyHead.gameObject.transform.position + new Vector3(0,1,0)), out hit);
        if(Vector3.Dot(playerDir, animator.transform.forward) > 0.6f && Vector3.Distance(animator.transform.position, PlayerController.player.transform.position) <= enemy.visionDistance && (hit.collider.GetComponentInParent<PlayerController>() != null || hit.collider.GetComponent<PlayerController>() != null))
        {
            if (!animator.GetBool("Alerted") && !PlayerController.player.GetComponent<Animator>().GetBool("CrouchingUnderObject"))
            {
                enemy.AlertEnemy(PlayerController.player.gameObject);
            }
            if (aggroTimer >= enemy.timeToAggro)
            {
                animator.SetBool("PlayerFound", true);
                enemy.enemy.GetComponent<Animator>().SetBool("PlayerFound", true);
                return;
            }
            animator.SetBool("PlayerInSight", true);
            animator.SetBool("PlayerInSearchRadius", true);
        }
        else
        {
            aggroTimer = 0;
            animator.SetBool("PlayerInSight", false);
        }*/
    }


    void CheckPlayerWithinAttackRange(Animator animator)
    {
        if(Vector3.Distance(animator.transform.position, PlayerController.player.transform.position) >= enemy.visionDistance)
        {
            animator.SetBool("PlayerInSight", false);
            animator.SetBool("PlayerInSearchRadius", false);
        }
        if (Vector3.Distance(animator.transform.position, PlayerController.player.transform.position) <= enemy.attackRange)
        {
            animator.SetBool("PlayerInAttackingDistance", true);
        }
        else
        {
            animator.SetBool("PlayerInAttackingDistance", false);
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
