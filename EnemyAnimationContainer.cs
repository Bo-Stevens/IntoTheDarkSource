using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class EnemyAnimationContainer : MonoBehaviour
{
    public AnimationClip walkForward;
    public GameObject neck;
    public GameObject head;
    public float maxNeckRotation = 60f;

    public GameObject weapon;

    public FootStepController footStepController;
    public AudioClip rustleSound;
    public AudioClip rustleSecondary;
    public AudioClip attackLanded;
    public AudioClip weaponAttackSound;
    public AudioClip enemyAttackSound;

    [HideInInspector]
    public bool meleeAttackFinished;


    private void Start()
    {
        meleeAttackFinished = true;
    }
    public void ActivateWeaponHitBox()
    {
        weapon.GetComponent<BoxCollider>().enabled = true;
        GetComponent<AudioSource>().PlayOneShot(weaponAttackSound, 0.5f);
    }

    public void FireSpell()
    {
        Instantiate(GetComponentInParent<ProjectileEnemyController>().projectileType, GetComponentInParent<ProjectileEnemyController>().castingHand.transform.position, GetComponentInParent<ProjectileEnemyController>().playerTracker.transform.rotation, null);
        GetComponentInParent<AudioSource>().Play();
    }

    public void MeleeAttackFinished()
    {
        meleeAttackFinished = true;
    }

    public void DisableWeaponHitBox()
    {
        weapon.GetComponent<BoxCollider>().enabled = false;
        weapon.GetComponent<EnemyMeleeAttackController>().hitPlayer = false;
    }
    public void DisableMovement()
    {
        StartCoroutine("DisableNavMeshAgent");
    }


    public void FootStep()
    {
        GetComponent<AudioSource>().PlayOneShot(footStepController.footStepSound, 0.3f);
    }

    public void RustleSound()
    {
        GetComponent<AudioSource>().PlayOneShot(rustleSound, 0.2f);
    }

    public void RustleSecondary()
    {
        GetComponent<AudioSource>().PlayOneShot(rustleSecondary, 0.1f);
    }

    public void DoneLookingAround()
    {
        GetComponentInParent<EnemyController>().GetComponent<Animator>().SetBool("GoingToLastKnownPos", true);
        GetComponentInParent<NavMeshAgent>().isStopped = false;
    }

    IEnumerator DisableNavMeshAgent()
    {
        yield return new WaitForSeconds(0.001f);
        GetComponentInParent<NavMeshAgent>().isStopped = true;
        GetComponentInParent<NavMeshAgent>().destination = transform.position;
    }
}
