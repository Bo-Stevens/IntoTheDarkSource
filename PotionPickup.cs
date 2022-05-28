using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionPickup : Pickup
{
    public override void GetItem()
    {
        if(PlayerController.player.numPotions < PlayerController.player.maxNumPotions)
        {
            PlayerController.player.GetComponent<AudioSource>().PlayOneShot(pickupSound);
            PlayerController.player.numPotions += 1;
            Destroy(this.gameObject);
        }
    }
}
