using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using UnityEngine;

public class TextFade : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        DOTween.To(() => GetComponent<TextMeshProUGUI>().color.a,
            x => GetComponent<TextMeshProUGUI>().color = new Color(GetComponent<TextMeshProUGUI>().color.r, GetComponent<TextMeshProUGUI>().color.g, GetComponent<TextMeshProUGUI>().color.b, x),
            0.25f, 2.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
    
}
