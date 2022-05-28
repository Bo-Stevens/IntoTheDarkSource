using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine;

public class StaminaBarController : MonoBehaviour
{
    [SerializeField]
    Image[] segments;

    [SerializeField]
    Vector3 onScreenPosition;

    [SerializeField]
    Vector3 offScreenPosition;


    float staminaPerSegment;
    int segmentIndex;
    Color normalColor;
    Color fadedColor;
    bool staminaOnScreen;
    // Start is called before the first frame update
    void Start()
    {
        staminaPerSegment = PlayerController.player.maxStamina / segments.Length;
        segmentIndex = segments.Length - 1;
        staminaOnScreen = false;

        normalColor = segments[0].GetComponent<Image>().color;
        fadedColor = new Color(segments[0].GetComponent<Image>().color.r, segments[0].GetComponent<Image>().color.g, segments[0].GetComponent<Image>().color.b, 0.15f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateStaminaBar()
    {
        if (!staminaOnScreen && PlayerController.player.stamina < PlayerController.player.maxStamina)
        {
            ShowStaminaBar();
        }

        float staminaUsed = PlayerController.player.maxStamina - PlayerController.player.stamina;

        int segmentsToDim = Mathf.CeilToInt(staminaUsed / staminaPerSegment);

        for (int i = 0; i < segments.Length; i++)
        {
            if(i <= segments.Length - segmentsToDim)
            {
                segments[i].GetComponent<Image>().color = normalColor;
            }
            else
            {
                segments[i].GetComponent<Image>().color = fadedColor;
            }
        }
    }

    public void ShowStaminaBar()
    {
        GetComponent<RectTransform>().DOPivot(new Vector2(0.5f, -2.5f), 0.5f);
        staminaOnScreen = true;
    }

    public void HideStaminaBar()
    {
        GetComponent<RectTransform>().DOPivot(new Vector2(0.5f, 0.5f), 0.5f);
        staminaOnScreen = false;
    }
}
