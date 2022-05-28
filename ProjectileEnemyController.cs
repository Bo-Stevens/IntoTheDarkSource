using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEnemyController : EnemyController
{

    public GameObject projectileType;
    public GameObject castingHand;

    [HideInInspector]
    public GameObject playerTracker;

    void Start()
    {
        GetComponent<Animator>().SetBool("Ranged", true);
        playerTracker = new GameObject("PlayerTracker");
        playerTracker.transform.parent = transform;
        playerTracker.transform.localPosition = new Vector3(0,3f,1.5f);
        base.InitializeEnemy();
    }
}
