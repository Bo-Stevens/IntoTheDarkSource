using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField]
    GameObject healthBar;
    [SerializeField]
    GameObject magicBar;
    [SerializeField]
    GameObject potionCount;
    [SerializeField]
    GameObject ammoCount;
    public GameObject staminaBar;
    public GameObject hitScreenEffect;
    public GameObject activeSpellContainer;
    public GameObject[] spellIcons;


    public static UIController controller;
    // Start is called before the first frame update
    void Start()
    {
        if(controller == null)
        {
            controller = this;
        }
        StartCoroutine("UpdateAtEndOfFrame");
    }

    IEnumerator UpdateAtEndOfFrame()
    {
        yield return new WaitForEndOfFrame();


        UpdateUI();
    }

    public void UpdateUI()
    {
        healthBar.GetComponent<TextMeshProUGUI>().text = "" + PlayerController.player.GetHealth();
        magicBar.GetComponent<TextMeshProUGUI>().text = " " + PlayerController.player.currentMagic;
        potionCount.GetComponent<TextMeshProUGUI>().text = PlayerController.player.numPotions + "/" + PlayerController.player.maxNumPotions;
        staminaBar.GetComponent<StaminaBarController>().UpdateStaminaBar();

        if (PlayerController.player.bow.activeSelf)
        {
            ammoCount.GetComponent<TextMeshProUGUI>().text = "" + PlayerController.player.bow.GetComponent<BowController>().currAmmoType.currAmmo + "/" + PlayerController.player.bow.GetComponent<BowController>().currAmmoType.maxAmmo;
        }
    } 

    public void FadeToBlack()
    {
        Image hitEffect = hitScreenEffect.GetComponent<Image>();
        hitEffect.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        hitEffect.gameObject.SetActive(true);
        DOTween.To(() => hitEffect.color.a,
            x => hitEffect.color = new Color(hitEffect.color.r, hitEffect.color.g, hitEffect.color.b, x),
            1f, 2f).OnComplete(PlayerController.player.KillPlayer).SetEase(Ease.Linear);
    }

    
}
