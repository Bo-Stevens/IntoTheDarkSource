using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public float spellCost;
    public AudioClip castSound;
    public virtual void UseSpell()
    {
    }
}
