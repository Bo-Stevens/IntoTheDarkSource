using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine;

public class StartMenuController : MonoBehaviour
{

    public AudioClip startGameSound;
    // Start is called before the first frame update
    void Start()
    {
        PlayerController.player.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
        AudioListener.volume = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void KeyPressed(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started )
        {
            return;
        }
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        AudioListener.volume = 1;
        GetComponent<AudioSource>().PlayOneShot(startGameSound);

        DOTween.To(() => GetComponent<Image>().color.a,
            x => GetComponent<Image>().color = new Color(GetComponent<Image>().color.r, GetComponent<Image>().color.g, GetComponent<Image>().color.b, x),
            0f, 5f).SetEase(Ease.InQuad).OnComplete(DisableObject);
        PlayerController.player.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");

    }

    void DisableObject()
    {
        gameObject.SetActive(false);
    }
}
