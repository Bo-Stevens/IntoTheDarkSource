using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{

    [HideInInspector]
    public bool arrowShot;

    public GameObject bloodSplatter;
    [SerializeField]
    AudioClip hitEnemySound;

    [SerializeField]
    AudioClip hitObjectSound;


    protected Quaternion rotation;
    protected Vector3 position;

    public bool arrowHit = false;

    // Start is called before the first frame update
    void Start()
    {
        arrowHit = false;
    }

    // Update is called once per frame
    void Update()
    {
        rotation = transform.rotation;
        position = transform.localPosition;
        if (!arrowHit)
        {
            PreemptiveCollisionCheck();
        }
    }

    
    protected void PreemptiveCollisionCheck()
    {
        RaycastHit hit;
        if (arrowShot && Physics.Raycast(transform.position + (transform.up / 1.9f), transform.up, out hit))
        {

            if (Vector3.Distance(transform.position, hit.point) < 2f && hit.collider.gameObject.tag != "Player" && hit.collider.gameObject.tag != "Arrow" && hit.collider.tag != "ArrowPickup")
            {
                GetComponent<Rigidbody>().isKinematic = true;
                GetComponent<BoxCollider>().enabled = false;

                GetComponent<Rigidbody>().rotation = rotation;
                transform.parent = hit.transform;
                transform.position = hit.point - (transform.up / 2.5f);
                HitTarget(hit.collider);
                arrowHit = true;
                GetComponent<ArrowController>().enabled = false;
            }
        }
    }

/*    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag != "Player" && other.gameObject.tag != gameObject.tag && other.gameObject.layer != 2 && arrowShot && GetComponent<Rigidbody>().isKinematic == false)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<BoxCollider>().enabled = false;

            GetComponent<Rigidbody>().rotation = rotation;
            transform.position = position;
            transform.parent = other.transform;

            HitTarget(other);


        }
    }*/
/*    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Player")
        {
            Debug.Log("HIT " + collision.gameObject.name);

        }

        if (collision.gameObject.tag != "Player" && collision.gameObject.tag != gameObject.tag && collision.gameObject.layer != 2 && arrowShot && GetComponent<Rigidbody>().isKinematic == false)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<BoxCollider>().enabled = false;


            float distance = Vector3.Distance(transform.position, collision.GetContact(0).point);
            Debug.Log("Distance from object to arrow " + distance);

            transform.position = collision.GetContact(0).point ;

            Debug.Log("Distance AFTER: " + Vector3.Distance(transform.position, collision.GetContact(0).point));



            //GetComponent<Rigidbody>().rotation = rotation;
            //transform.position = position;
            transform.parent = collision.transform;

            HitTarget(collision);


        }
    }*/



    public virtual void HitTarget(Collider other)
    {
        GetComponent<TrailRenderer>().enabled = true;
        if (other.gameObject.GetComponentInParent<EnemyController>() != null)
        {
            other.gameObject.GetComponentInParent<EnemyController>().EnemyShot(other, 5f);
            if(bloodSplatter != null)
            {
                bloodSplatter.SetActive(true);
            }
            GetComponent<AudioSource>().PlayOneShot(hitEnemySound);
        }
        else
        {
            List<GameObject> enemies = Utility.GetEnemiesInRange(transform.position, 10f);
            GetComponent<AudioSource>().PlayOneShot(hitObjectSound);
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].GetComponent<EnemyController>().AlertEnemy(gameObject);
            }
        }
    }

}
