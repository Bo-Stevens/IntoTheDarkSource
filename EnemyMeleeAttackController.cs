using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttackController : MonoBehaviour
{
    [SerializeField]
    int damageDealt;
    [SerializeField]
    AudioClip parrySound;
    [HideInInspector]
    public bool hitPlayer = false;


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && !hitPlayer)
        {

            if (PlayerController.player.parrying)
            {
                GetComponentInParent<EnemyAnimationContainer>().GetComponent<Animator>().SetTrigger("Parried");
                GetComponent<AudioSource>().PlayOneShot(parrySound);
                GetComponentInParent<EnemyAnimationContainer>().meleeAttackFinished = true;
                GetComponentInParent<EnemyController>().GetComponent<Animator>().SetBool("Recovering", true);
                PlayerController.player.GetComponent<Animator>().SetBool("Parry", false);
                CameraController.cam.ShakeCamera(0.25f, 25f, 0.045f);
                hitPlayer = true;
            }
            else
            {
                CameraController.cam.ShakeCamera(0.25f, 25f, 0.038f);
                PlayerController.player.ApplyDamage(damageDealt);
                hitPlayer = true;
                GetComponent<AudioSource>().Play();
            }
        }
    }

}
