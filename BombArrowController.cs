using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BombArrowController : ArrowController
{
    [SerializeField]
    GameObject explosionEffect;
    [SerializeField]
    GameObject lightSource;
    [SerializeField]
    AudioClip fuseBurning; 

    private void Start()
    {
        arrowHit = false;
    }
    private void OnEnable()
    {
    }

    public void PlaySound()
    {
        GetComponent<AudioSource>().PlayOneShot(fuseBurning);
    }

    void Update()
    {
        rotation = transform.rotation;
        position = transform.localPosition;

        PreemptiveCollisionCheck();
    }

    public override void HitTarget(Collider other)
    {
        List<GameObject> enemiesHit = Utility.GetEnemiesInRange(transform.position, 8f);

        for(int i = 0; i < enemiesHit.Count; i++) 
        {
            Vector3 explosionDirection = Vector3.Normalize(enemiesHit[i].gameObject.transform.position - transform.position);
            explosionDirection = new Vector3(explosionDirection.x, 0.2f, explosionDirection.z);
            enemiesHit[i].GetComponent<EnemyController>().ExplodeOutward(explosionDirection, 75f);
        }
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().Play();
        lightSource.SetActive(false);
    }




}
