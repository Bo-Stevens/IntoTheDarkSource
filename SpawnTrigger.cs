using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnTrigger : MonoBehaviour
{

    [SerializeField]
    GameObject[] entitiesToSpawn;

    bool spawnedEntities = false;


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < entitiesToSpawn.Length; i++)
        {
            entitiesToSpawn[i].gameObject.SetActive(false);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !spawnedEntities)
        {
            spawnedEntities = true;

            for(int i = 0; i < entitiesToSpawn.Length; i++)
            {
                entitiesToSpawn[i].gameObject.SetActive(true);
            }

        }
    }



}
