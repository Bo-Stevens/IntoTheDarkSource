using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController cam;
    public GameObject cameraContainer;
    // Start is called before the first frame update
    void Start()
    {
        cam = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShakeCamera(float duration, float roughness, float intensity)
    {
        StartCoroutine(ShakeCameraActual(duration, roughness, intensity, 1f));
    }

    public void ShakeCamera(float duration, float roughness, float intensity, float falloff)
    {
        StartCoroutine(ShakeCameraActual(duration, roughness, intensity, falloff));
    }

    IEnumerator ShakeCameraActual(float duration, float roughness, float intensity, float falloff)
    {
        float elapsedTime = 0f;
        roughness = 1 / roughness;

        while (elapsedTime < duration)
        {
            cameraContainer.transform.DOLocalMove(Random.insideUnitSphere * intensity * falloff, roughness).SetEase(Ease.InOutBack);

            falloff *= 0.9f;

            elapsedTime += Time.deltaTime + roughness;
            yield return new WaitForSeconds(roughness);
        }

        cameraContainer.transform.DOLocalMove(Vector3.zero, roughness);

    }
}
