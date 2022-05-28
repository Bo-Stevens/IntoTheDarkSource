using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    protected AudioClip equipSound;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnEnable()
    {
        if (PlayerController.player != null && !PlayerController.player.GetComponent<Animator>().GetBool("ReadyForBackStab"))
        {
            StartCoroutine("PlayEquipSound");
        }
    }

    IEnumerator PlayEquipSound()
    {
        yield return new WaitForEndOfFrame();
        PlayerController.player.GetComponent<AudioSource>().PlayOneShot(equipSound, 0.5f);
    }

    public virtual void AttackButtonClicked(InputAction.CallbackContext context)
    {

    }
}
