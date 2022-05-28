using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GateSwitch : MonoBehaviour
{
    [SerializeField]
    GameObject door;
    [SerializeField]
    GameObject gateSwitch;


    [Header("CameraShake")]
    [SerializeField]
    float duration;
    [SerializeField]
    float roughness;
    [SerializeField]
    float intensity;
    [SerializeField]
    [Range(0f,1f)]
    float falloffPerCycle;

    // Start is called before the first frame update
    void Start()
    {
        //SwitchActivated();
    }
    

    public void SwitchActivated()
    {
        door.transform.DOMove(door.transform.position - new Vector3(0, 5f, 0), 5f);
        door.GetComponent<AudioSource>().Play();
        door.GetComponent<AudioSource>().DOFade(0f, 5f);
        CameraController.cam.ShakeCamera(duration, roughness, intensity, falloffPerCycle);
        if (gateSwitch != null)
        {
            gateSwitch.transform.DORotate(new Vector3(0, gateSwitch.transform.eulerAngles.y, -40f), 1f);
        }
    }
}
