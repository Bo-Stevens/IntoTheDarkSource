using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehavior : StateMachineBehaviour
{

    bool isBow;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (animator.GetComponent<PlayerController>().bow.activeSelf){
            isBow = true;
        }
        animator.GetComponent<PlayerController>().bow.SetActive(false);
        animator.GetComponent<PlayerController>().sword.SetActive(false);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("UsingItem", false);
        if (isBow)
        {
            animator.GetComponent<PlayerController>().bow.SetActive(true);
        }
        else
        {
            animator.GetComponent<PlayerController>().sword.SetActive(true);
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
