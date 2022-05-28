using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkVisionSpell : Spell
{
    public static DarkVisionSpell instance;

    void Start()
    {
        UseSpell();
        instance = this;
    }
    public override void UseSpell()
    {
        StartCoroutine("SpellActive");
        PlayerController.player.GetComponent<Animator>().SetTrigger("DarkVision");

        for(int i = 0; i < EnemyController.allEnemies.Count; i++)
        {
            if (!EnemyController.allEnemies[i].GetComponent<EnemyController>().isDead)
            {
                EnemyController.allEnemies[i].GetComponent<EnemyController>().EnableDarkVisionMaterial();
            }
        }
    }

    IEnumerator SpellActive()
    {
        while (gameObject.activeSelf)
        {
            yield return new WaitForSeconds(0.25f);
            PlayerController.player.currentMagic -= (int) (spellCost);
            UIController.controller.UpdateUI();

            if(PlayerController.player.currentMagic <= 0)
            {
                StartCoroutine("DisableSpell");
            }
        }
    }

    IEnumerator DisableSpell()
    {

        for (int i = 0; i < EnemyController.allEnemies.Count; i++)
        {
            EnemyController.allEnemies[i].GetComponent<EnemyController>().DisableDarkVisionMaterial();
        }

        yield return new WaitForSeconds(0.001f);
        instance = null;
        Destroy(this.gameObject);
    }

}
