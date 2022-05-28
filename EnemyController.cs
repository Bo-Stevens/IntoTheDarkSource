using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEditor;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameObject[] waypoints;
    float lastNumWayPoints;
    public GameObject enemy;
    
    [HideInInspector]
    public int waypointIndex = 0;
    public float wayPointDistance;
    public float visionDistance;
    public float attackRange;
    [SerializeField]
    float health = 10f;
    float speed;
    float previousYRotation;


    
    public float patrolSpeed;

    public float chaseSpeed;

    public bool ranged;

    [HideInInspector]
    public bool isDead;

    Rigidbody[] enemyRigidbodies;

    public float timeToAggro;
    public float timeToAlert;

    [HideInInspector]
    public Vector3 playerLostKnownPos;


    public GameObject coneOfSight;

    [SerializeField]
    Material darkVisionMat;
    Material originalMaterial;

    public static List<GameObject> allEnemies;

    [SerializeField]
    GameObject body;
    public float aggroTimer;

    
    public AudioClip[] playerNoticedSounds;
    public AudioClip[] alertSounds;
    public AudioClip[] aggroSounds;
    public AudioClip deathSound;

    // Start is called before the first frame update
    void OnEnable()
    {
        GetComponent<Animator>().SetBool("Ranged", ranged);
        InitializeEnemy();
        lastNumWayPoints = waypoints.Length;

        if(allEnemies == null)
        {
            allEnemies = new List<GameObject>();
        }
        originalMaterial = body.GetComponent<SkinnedMeshRenderer>().material;
        allEnemies.Add(gameObject);
    }

    protected void InitializeEnemy()
    {
        speed = 0f;
        enemy.GetComponent<Animator>().speed = Random.Range(0.97f, 1.03f);
        DisableRagdoll();
        EnableNavigation();

        for(int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i].transform.parent = null;
        }
    }

    public void DisableRagdoll()
    {
        enemyRigidbodies = gameObject.GetComponentsInChildren<Rigidbody>();

        for(int i = 0; i < enemyRigidbodies.Length; i++)
        {
            enemyRigidbodies[i].isKinematic = true;
        }
    }
    public void EnableNavigation()
    {
        GetComponent<NavMeshAgent>().enabled = true;
        GetComponent<Animator>().enabled = true;
        enemy.GetComponent<Animator>().enabled = true;
    }

    public void EnableRagdoll()
    {
        GetComponent<Animator>().enabled = false;

        //GetComponent<NavMeshAgent>().destination = transform.position;
        GetComponent<NavMeshAgent>().enabled = false;
        enemy.GetComponent<Animator>().enabled = false;
        for (int i = 0; i < enemyRigidbodies.Length; i++)
        {
            enemyRigidbodies[i].isKinematic = false;
            enemyRigidbodies[i].useGravity = true;

        }
    }

    public void EnableRagdollNoGravity()
    {
        EnableRagdoll();
        for(int i = 0; i < enemyRigidbodies.Length; i++)
        {
            enemyRigidbodies[i].useGravity = false;
        }
    }

    void Update()
    {
        UpdateAnimator();

        if (GetComponent<Animator>().GetBool("PlayerFound"))
        {
            CheckPlayerWithinAttackRange(GetComponent<Animator>());
        }
    }

    [ExecuteInEditMode]
#if UNITY_EDITOR
    private void OnValidate()
    {
        GameObject waypoint =  Resources.Load<GameObject>("Prefabs/Waypoint");
        if(lastNumWayPoints == waypoints.Length || EditorApplication.isPlaying)
        {
            return;
        }
        if (lastNumWayPoints == 0 && waypoints[0] != null)
        {
            lastNumWayPoints = waypoints.Length;
            return;
        }

        if(waypoints.Length > lastNumWayPoints)
        {
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (i >= lastNumWayPoints)
                {
                    waypoints[i] = Instantiate(waypoint, transform.position, Quaternion.identity, transform);
                }
            }
        }
        else
        {
            WayPointController[] allWaypoints = GetComponentsInChildren<WayPointController>();

            for(int inactiveWaypointIndex = 0; inactiveWaypointIndex < allWaypoints.Length; inactiveWaypointIndex++)
            {
                bool waypointIsValid = false;
                for(int activeWaypointIndex = 0; activeWaypointIndex < waypoints.Length; activeWaypointIndex++)
                {
                    if(allWaypoints[inactiveWaypointIndex].gameObject == waypoints[activeWaypointIndex].gameObject)
                    {
                        waypointIsValid = true;
                        break;
                    }
                }
                if (!waypointIsValid)
                {
                    IEnumerator thingother = DestroyGO(allWaypoints[inactiveWaypointIndex].gameObject);
                    StartCoroutine(thingother);

                    IEnumerator DestroyGO(GameObject go)
                    {
                        yield return new WaitForSeconds(0);
                        DestroyImmediate(go);
                    }
                }
            }
        }

        lastNumWayPoints = waypoints.Length;

    }
#endif

    void UpdateAnimator()
    {
        //speed = Mathf.Lerp(speed, (transform.position - previousPosition).magnitude * 30f, 0.05f);
        speed = GetComponent<NavMeshAgent>().speed;
        if (enemy.GetComponent<Animator>() != null)
        {
            enemy.GetComponent<Animator>().SetFloat("Speed", speed);
            float yRotation = Mathf.Lerp(previousYRotation, Utility.GetProperEulerVal(transform.localEulerAngles.y) / 360f, 0.01f);
            enemy.GetComponent<Animator>().SetFloat("Sideways", (yRotation - previousYRotation) * 350f);
            previousYRotation = yRotation;
        }

    }

    public void EnemyShot(Collider targetHit, float damageTaken)
    {
        if(targetHit == enemy.GetComponent<EnemyAnimationContainer>().head.GetComponent<SphereCollider>())
        {
            health -= damageTaken * 2;
        }
        else
        {
            health -= damageTaken;
        }
        //GetComponent<Animator>().SetBool("AttackedFromStealth", true);

        if (health <= 0 && !isDead)
        {
            if (targetHit.gameObject.GetComponent<Rigidbody>() != null)
            {
                targetHit.gameObject.GetComponent<Rigidbody>().AddForce((transform.position - PlayerController.player.transform.position) * 300f);
            }
            else
            {
                targetHit.gameObject.GetComponentInParent<Rigidbody>().AddForce((transform.position - PlayerController.player.transform.position) * 300f);
            }
            StartCoroutine("Die");
        }
        else
        {
            AggroEnemy();
        }
    }

    public void EnemyStabbed(Collider targetHit, float damageTaken)
    {
        health -= damageTaken;
        GetComponent<Animator>().SetBool("AttackedFromStealth", true);

        if (health <= 0 && !isDead)
        {
            if(targetHit.gameObject.GetComponent<Rigidbody>() != null)
            {
                targetHit.gameObject.GetComponent<Rigidbody>().AddForce((transform.position - PlayerController.player.transform.position) * 300f);
            }
            else
            {
                GetComponent<Rigidbody>().AddForce((transform.position - PlayerController.player.transform.position) * 300f);
            }
            StartCoroutine("Die");
        }
    }

    IEnumerator Die()
    {
        isDead = true;
        gameObject.GetComponent<EnemyController>().enabled = false;
        EnableRagdoll();
        DisableDarkVisionMaterial();
        GetComponent<AudioSource>().Stop();
        if (GetComponent<Animator>().GetBool("PlayerFound"))
        {
            GetComponent<AudioSource>().PlayOneShot(deathSound);
        }

        gameObject.layer = 0;
        foreach(Transform child in transform)
        {
            child.gameObject.layer = 0;
        }

        foreach(Transform child in enemy.transform)
        {
            child.gameObject.layer = 0;
        }

        yield return new WaitForSeconds(5f);
        DisableRagdoll();
    }

    public void ExplodeOutward(Vector3 direction, float force)
    {
        GetComponent<EnemyController>().enabled = false;
        GetComponent<Animator>().enabled = false;
        GetComponent<NavMeshAgent>().enabled = false;
        enemy.GetComponent<Animator>().enabled = false;
        enemy.GetComponent<EnemyAnimationContainer>().enabled = false;

        EnableRagdoll();
        StartCoroutine("Die");
        Rigidbody[] rigidBodies = GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < rigidBodies.Length; i++)
        {
            rigidBodies[i].gameObject.GetComponent<Rigidbody>().AddForce(direction * force, ForceMode.Impulse);
        }
    }

    public void AlertEnemy(GameObject alertedBy)
    {
        playerLostKnownPos = alertedBy.transform.position;
        GetComponent<Animator>().SetBool("Alerted", true);
    }

    public void EnableDarkVisionMaterial()
    {
        SkinnedMeshRenderer[] meshes = GetComponentsInChildren<SkinnedMeshRenderer>();

        for(int i = 0; i < meshes.Length; i++)
        {
            meshes[i].material = darkVisionMat;
        }
        coneOfSight.GetComponent<MeshRenderer>().enabled = true;
    }

    public void DisableDarkVisionMaterial()
    {
        SkinnedMeshRenderer[] meshes = GetComponentsInChildren<SkinnedMeshRenderer>();

        for (int i = 0; i < meshes.Length; i++)
        {
            meshes[i].material = originalMaterial;
        }
        coneOfSight.GetComponent<MeshRenderer>().enabled = false;
    }


    public void SearchForPlayer()
    {
        aggroTimer += Time.deltaTime;
        GameObject enemyHead = enemy.GetComponent<EnemyAnimationContainer>().head;
        Debug.DrawLine(enemyHead.transform.position + (transform.forward / 4f), enemyHead.transform.position + (Vector3.Normalize(PlayerController.player.transform.position - enemyHead.gameObject.transform.position ) * visionDistance), Color.green);
        RaycastHit hit;
        if (Physics.Raycast(enemyHead.transform.position + (transform.forward / 4f), Vector3.Normalize(PlayerController.player.transform.position - enemyHead.gameObject.transform.position), out hit) && hit.collider.gameObject.tag == "Player"
            && Vector3.Dot(enemy.transform.forward, Vector3.Normalize(PlayerController.player.transform.position - enemyHead.gameObject.transform.position + new Vector3(0, 1, 0))) >= 0.5f
            )
        {
            if (!GetComponent<Animator>().GetBool("Alerted") && !PlayerController.player.GetComponent<Animator>().GetBool("CrouchingUnderObject") && GetComponent<Animator>().GetBool("PlayerInSight"))
            {
                AlertEnemy(PlayerController.player.gameObject);
                Debug.Log("EnemyWhoFoundYOuIS " + gameObject.name);
                if(!GetComponent<Animator>().GetBool("PlayerFound"))
                    GetComponentInParent<AudioSource>().PlayOneShot(GetComponentInParent<EnemyController>().playerNoticedSounds[Random.Range(0, GetComponentInParent<EnemyController>().playerNoticedSounds.Length)]);
            }
            if (aggroTimer >= timeToAggro)
            {
                AggroEnemy();
                return;
            }
            GetComponent<Animator>().SetBool("PlayerInSight", true);
            GetComponent<Animator>().SetBool("PlayerInSearchRadius", true);
        }
        else if (coneOfSight.GetComponent<SightConeController>().playerInRange)
        {
            coneOfSight.GetComponent<SightConeController>().PlayerOutOfSight();
            aggroTimer = 0;
        }
        else
        {
            aggroTimer = 0;
        }

    }

    public void AggroEnemy()
    {
        if (GetComponent<Animator>().GetBool("PlayerFound"))
        {
            return;
        }
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().PlayOneShot(aggroSounds[Random.Range(0, aggroSounds.Length)]);

        if (transform.parent.tag == "EnemyGroup")
        {
            EnemyController[] enemies = transform.parent.GetComponentsInChildren<EnemyController>();
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].GetComponent<Animator>().SetBool("PlayerFound", true);
                enemies[i].enemy.GetComponent<Animator>().SetTrigger("PlayerFound");
                enemies[i].GetComponent<Animator>().SetBool("PlayerInSearchRadius", true);
            }
        }
        else
        {
            GetComponent<Animator>().SetBool("PlayerFound", true);
            GetComponent<Animator>().SetBool("PlayerInSearchRadius", true);
            enemy.GetComponent<Animator>().SetTrigger("PlayerFound");
        }

    }

    void CheckPlayerWithinAttackRange(Animator animator)
    {

        if (Vector3.Distance(animator.transform.position, PlayerController.player.transform.position) <= attackRange)
        {
            animator.SetBool("PlayerInAttackingDistance", true);
        }
        else
        {
            animator.SetBool("PlayerInAttackingDistance", false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        GameObject enemyHead = enemy.GetComponent<EnemyAnimationContainer>().head;
        Gizmos.DrawLine(enemyHead.transform.position, enemyHead.transform.position + (transform.forward * visionDistance));
    }
}