using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FireLightController : MonoBehaviour
{
    public Light lightSource;
    public float maxLightValue;
    public float minLightValue;

    public float minTime;
    public float maxTime;
    // Start is called before the first frame update
    void Start()
    {
        lightSource.DOIntensity(Random.Range(minLightValue, maxLightValue), Random.Range(minTime, maxTime)).OnComplete(TweenLight);
    }

    void TweenLight()
    {
        lightSource.DOIntensity(Random.Range(minLightValue, maxLightValue), Random.Range(minTime, maxTime)).OnComplete(TweenLight);
    }
}
