using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;
using UnityEngine.AI;
using UnityEngine;

public class HoldSpell : Spell
{

    Dictionary<GameObject, List<Quaternion>> enemiesHit;

    public float floatHeight;

    void Start()
    {
        UseSpell();
    }
    public override void UseSpell()
    {
        base.UseSpell();
        Collider[] hitColliders = Physics.OverlapBox(transform.position, new Vector3(10, 10, 10));
        
        //pulling out all of the enemies from all the objects we collided with
        enemiesHit = new Dictionary<GameObject, List<Quaternion>>();
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].gameObject.GetComponentInParent<EnemyController>() != null && !enemiesHit.ContainsKey(hitColliders[i].gameObject.GetComponentInParent<EnemyController>().gameObject) && !hitColliders[i].gameObject.GetComponentInParent<EnemyController>().isDead)
            {
                enemiesHit.Add(hitColliders[i].gameObject.GetComponentInParent<EnemyController>().gameObject, new List<Quaternion>());
            }
        }

        //Making enemies float into the air
        for (int i = 0; i < enemiesHit.Count; i++)
        {
            List<Quaternion> jointRotations = new List<Quaternion>();
            Transform[] children = enemiesHit.ElementAt(i).Key.GetComponentsInChildren<Transform>();
            for (int x = 0; x < children.Length; x++)
            {
                jointRotations.Add(children[x].transform.rotation);
            }
            enemiesHit.ElementAt(i).Key.GetComponent<EnemyController>().AggroEnemy();
            enemiesHit[enemiesHit.ElementAt(i).Key] = jointRotations;

            enemiesHit.ElementAt(i).Key.GetComponent<EnemyController>().EnableRagdollNoGravity();
            enemiesHit.ElementAt(i).Key.GetComponent<Rigidbody>().useGravity = false;
            enemiesHit.ElementAt(i).Key.gameObject.transform.DOMove(enemiesHit.ElementAt(i).Key.gameObject.transform.position + new Vector3(0, floatHeight, 0), 2.5f);
        }
        StartCoroutine("DisableSpell");
    }

    IEnumerator DisableSpell()
    {
        yield return new WaitForSeconds(8f);

        //Disabling Ragdoll physics
        for (int i = 0; i < enemiesHit.Count; i++)
        {
            if (!enemiesHit.ElementAt(i).Key.GetComponent<EnemyController>().isDead)
            {
                enemiesHit.ElementAt(i).Key.GetComponent<EnemyController>().DisableRagdoll();
                enemiesHit.ElementAt(i).Key.GetComponent<Rigidbody>().useGravity = true;
            }
        }


        //Making enemies fall back to the ground
        yield return new WaitForSeconds(0.25f);
        for (int i = 0; i < enemiesHit.Count; i++)
        {
            enemiesHit.ElementAt(i).Key.gameObject.transform.DOMove(enemiesHit.ElementAt(i).Key.gameObject.transform.position + new Vector3(0, -floatHeight, 0), 0.75f).SetEase(Ease.InQuad);
        }


        //Rotating enemies limbs back into place
        for (int i = 0; i < enemiesHit.Count; i++)
        {
            if (!enemiesHit.ElementAt(i).Key.GetComponent<EnemyController>().isDead)
            {
                Transform[] children = enemiesHit.ElementAt(i).Key.GetComponentsInChildren<Transform>();
                //If we find an object we don't want anything to do with, we subtract this offset from x so we don't get an out of bounds error
                int junkOffset = 0;
                for (int x = 0; x < children.Length; x++)
                {
                    if (children[x].GetComponent<ArrowController>() != null || children[x].GetComponentInParent<ArrowController>() != null)
                    {
                        junkOffset += 1;
                        continue;
                    }

                    children[x].DORotate(enemiesHit.ElementAt(i).Value[x - junkOffset].eulerAngles, 0.75f).SetEase(Ease.OutCubic);
                    if (children[x].GetComponent<Rigidbody>() != null)
                    {
                        children[x].GetComponent<Rigidbody>().useGravity = true;
                    }
                }
            }
        }

        //Starting enemy pathfinding back up
        yield return new WaitForSeconds(0.75f);
        for (int i = 0; i < enemiesHit.Count; i++)
        {
            if (!enemiesHit.ElementAt(i).Key.GetComponent<EnemyController>().isDead)
            {
                enemiesHit.ElementAt(i).Key.GetComponent<EnemyController>().EnableNavigation();
            }
        }

        Destroy(this);
    }


    /*    private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Enemy")
            {
                other.GetComponent<Rigidbody>().useGravity = false;
                other.gameObject.transform.DOMove(other.gameObject.transform.position + new Vector3(0, 3f, 0), 2.5f);
            }
        }*/
}
