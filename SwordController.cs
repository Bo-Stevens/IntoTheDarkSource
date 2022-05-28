using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordController : Weapon
{
    [HideInInspector]
    public bool swordSwung;
    [SerializeField]
    GameObject bloodSplatter;
    [SerializeField]
    GameObject swordTip;
    [SerializeField]
    AudioClip hitEnemySound;
    [SerializeField]
    AudioClip hitObjectSound;
    [SerializeField]
    AudioClip parrySound;

    public AudioClip swingSound;

    [SerializeField]
    float weaponDamage;

    [HideInInspector]
    public bool hitEnemyOnce;

    [SerializeField]
    int framesToWaitForCheckBackstab;
    int frameCounter;

    [SerializeField]
    float backStabRadius;



    // Update is called once per frame
    void Update()
    {
        frameCounter += 1;
        if(frameCounter >= framesToWaitForCheckBackstab || PlayerController.player.GetComponent<Animator>().GetBool("ReadyForBackStab"))
        {
            CanBackstab();
            frameCounter = 0;
        }
    }

    void CanBackstab()
    {
        List<GameObject> enemies = Utility.GetEnemiesInRange(PlayerController.player.transform.position, backStabRadius);

        for(int i = 0; i < enemies.Count; i++)
        {
            if(Vector3.Dot(Vector3.Normalize(PlayerController.player.transform.position - enemies[i].transform.position), enemies[i].transform.forward) <= -0.65f && !enemies[i].GetComponent<EnemyController>().isDead && !enemies[i].GetComponent<Animator>().GetBool("PlayerFound"))
            {
                PlayerController.player.GetComponent<Animator>().SetBool("ReadyForBackStab", true);
                return;
            }
        }
        PlayerController.player.GetComponent<Animator>().SetBool("ReadyForBackStab", false);
    }

    public override void AttackButtonClicked(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Canceled)
        {
            return;
        }
        PlayerController.player.GetComponent<Animator>().SetTrigger("MeleeAttack");
    }

    public void PlayAttackSound()
    {
        GetComponent<AudioSource>().PlayOneShot(swingSound, 0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player" && other.gameObject.tag != gameObject.tag && other.gameObject.layer != 2 && swordSwung && !hitEnemyOnce)
        {
            HitTarget(other);
        }
    }

    public virtual void HitTarget(Collider other)
    {
        float damage = weaponDamage;
        if (PlayerController.player.GetComponent<Animator>().GetBool("ReadyForBackStab"))
        {
            damage *= 10f;
        }

        if (other.gameObject.GetComponentInParent<EnemyController>() != null)
        {
            StartCoroutine("HitStop");
            hitEnemyOnce = true;
            other.gameObject.GetComponentInParent<EnemyController>().EnemyStabbed(other, damage);
            if (bloodSplatter != null)
            {
                Instantiate(bloodSplatter, swordTip.transform.position, Quaternion.identity, null);
            }
            GetComponent<AudioSource>().PlayOneShot(hitEnemySound);
        }
        else
        {
            GetComponent<AudioSource>().PlayOneShot(hitObjectSound);
        }
    }


    IEnumerator HitStop()
    {
        PlayerController.player.GetComponent<Animator>().SetFloat("SwordAttackSpeed", 0.025f);
        CameraController.cam.ShakeCamera(0.2f, 10, 0.0075f);
        yield return new WaitForSeconds(0.075f);
        PlayerController.player.GetComponent<Animator>().SetFloat("SwordAttackSpeed", 1f);

    }
}
