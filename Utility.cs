using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine.AI;
using UnityEngine;

public class Utility : MonoBehaviour
{
    //Rotations in Unity like to flip-flop in a really weird way. This function just returns them in a more intuitive format
    public static float GetProperEulerVal(float eulerVal)
    {
        if (eulerVal > 180) eulerVal -= 360f;
        return eulerVal;
    }

    public static List<GameObject> GetEnemiesInRange(Vector3 position, float range)
    {
        Collider[] hitColliders = Physics.OverlapSphere(position, range);
        Dictionary<GameObject, GameObject> enemiesHit = new Dictionary<GameObject, GameObject>();

        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].gameObject.GetComponentInParent<EnemyController>() != null && !enemiesHit.ContainsKey(hitColliders[i].gameObject.GetComponentInParent<EnemyController>().gameObject))
            {
                enemiesHit.Add(hitColliders[i].gameObject.GetComponentInParent<EnemyController>().gameObject, hitColliders[i].gameObject.GetComponentInParent<EnemyController>().gameObject);
            }
        }

        List<GameObject> enemies = new List<GameObject>();
        for(int i = 0; i < enemiesHit.Count; i++)
        {
            enemies.Add(enemiesHit.ElementAt(i).Key);
        }
        return enemies;
    }

    public static Vector3 RandomNavPositionInRadius(Vector3 position, float radius)
    {
        Vector3 finalPosition = Vector3.zero;
        Vector3 randomDirection = position + (Random.insideUnitSphere * radius);
        NavMeshHit navHit;

            if (NavMesh.SamplePosition(randomDirection, out navHit, radius, NavMesh.AllAreas))
            {
                finalPosition = navHit.position;
            }

        return finalPosition;
    }
    public static void TweenValue(float value)
    {
        DOTween.To(() => value, x => value += x, 0f, 0.25f);
    }
}
