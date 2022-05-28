using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class FootstepSoundManger : MonoBehaviour
{
    public static FootstepSoundManger soundManager;

    [Serializable]
    public struct FootStepSound
    {
        public Material material;
        public AudioClip footstep;
    }
    [SerializeField]
    FootStepSound[] footstepSoundArray;

    public Dictionary<string, AudioClip> footstepSounds = new Dictionary<string, AudioClip>();
    // Start is called before the first frame update
    void Awake()
    {
        soundManager = this;

        for(int i = 0; i < footstepSoundArray.Length; i++)
        {
            footstepSounds.Add(footstepSoundArray[i].material.name, footstepSoundArray[i].footstep);
            //footstepSounds[footstepSoundArray[i].material.name] = footstepSoundArray[i].footstep; 
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
