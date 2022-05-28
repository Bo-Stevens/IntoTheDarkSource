using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FireballController : MonoBehaviour
{
    public float speed;

    [SerializeField]
    float damage;
    Vector3 direction;
    // Start is called before the first frame update
    void Start()
    {
        //GetComponent<Rigidbody>().AddForce(transform.forward * force);
        direction = transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * Time.deltaTime * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().ApplyDamage(damage);
        }
    }
}
