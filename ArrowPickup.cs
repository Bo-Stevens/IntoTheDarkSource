using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPickup : Pickup
{
    //Jank and needs changing
    [SerializeField]
    bool isNormalArrow;
    [SerializeField]
    bool isBombArrow;

    [SerializeField]
    int ammoAmount;

    public override void GetItem()
    {
        if (isNormalArrow)
        {
            PlayerController.player.bow.GetComponent<BowController>().AddAmmo(PlayerController.player.bow.GetComponent<BowController>().normalArrow, ammoAmount);
            if(PlayerController.player.bow.GetComponent<BowController>().currAmmoType.arrow.gameObject.name == PlayerController.player.bow.GetComponent<BowController>().normalArrow.arrow.gameObject.name)
            {
                PlayerController.player.bow.GetComponent<BowController>().SpawnArrow();
            }
        }
        else if (isBombArrow)
        {
            PlayerController.player.bow.GetComponent<BowController>().AddAmmo(PlayerController.player.bow.GetComponent<BowController>().bombArrow, ammoAmount);
            if (PlayerController.player.bow.GetComponent<BowController>().currAmmoType.arrow.gameObject.name == PlayerController.player.bow.GetComponent<BowController>().bombArrow.arrow.gameObject.name)
            {
                PlayerController.player.bow.GetComponent<BowController>().SpawnArrow();
            }
        }
        PlayerController.player.GetComponent<AudioSource>().PlayOneShot(pickupSound);
        Destroy(this.gameObject);
    }
}
