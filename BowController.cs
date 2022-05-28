using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class BowController : Weapon
{
    public GameObject arrowContainer;
    public float maxArrowsToInstantiate;
    [HideInInspector]
    public bool readyToFire = false;

    [SerializeField]
    float chargeTime;
    float superChargeTimer;

    [SerializeField]
    float arrowFireForce;

    public class AmmoType
    {
        public GameObject arrow;
        public int maxAmmo;
        public int currAmmo;
        public int arrowIndex;
        public List<GameObject> arrows;

        public AmmoType(GameObject arrow, int currAmmo, int maxAmmo)
        {
            this.arrow = arrow;
            this.maxAmmo = maxAmmo;
            this.currAmmo = currAmmo;
            arrowIndex = 0;
            arrows = new List<GameObject>();

            for (int i = 0; i < PlayerController.player.bow.GetComponent<BowController>().maxArrowsToInstantiate && i < maxAmmo; i++)
            {
                GameObject arrowInstance = Instantiate(arrow, new Vector3(0, 0, 0), Quaternion.identity);
                arrowInstance.GetComponent<Rigidbody>().isKinematic = true;
                arrowInstance.transform.parent = PlayerController.player.bow.GetComponent<BowController>().arrowContainer.transform;
                arrowInstance.SetActive(false);
                arrows.Add(arrowInstance);
            }
        }
    }

    [SerializeField]
    GameObject arrowPrefab;
    [SerializeField]
    GameObject bombArrowPrefab;

    public AmmoType normalArrow;
    public AmmoType bombArrow;

    public AmmoType currAmmoType;

    [SerializeField]
    AudioClip arrowFireSound;

    public AudioClip arrowPullSound;

    // Start is called before the first frame update
    void Start()
    {
        normalArrow = new AmmoType(arrowPrefab, 8, 16);
        bombArrow = new AmmoType(bombArrowPrefab, 1, 3);

        SetActiveArrowType(normalArrow);
    }


    public void SetActiveArrowType(AmmoType ammoType)
    {
        if(currAmmoType != null)
        {
            currAmmoType.arrows[currAmmoType.arrowIndex].SetActive(false);
        }


        currAmmoType = ammoType;
        if(currAmmoType.currAmmo > 0)
        {
            currAmmoType.arrows[currAmmoType.arrowIndex].SetActive(true);
            currAmmoType.arrows[currAmmoType.arrowIndex].GetComponent<ArrowController>().arrowHit = false;
            currAmmoType.arrows[currAmmoType.arrowIndex].GetComponent<ArrowController>().enabled = false;
            currAmmoType.arrows[currAmmoType.arrowIndex].GetComponent<ArrowController>().bloodSplatter.SetActive(false);
            currAmmoType.arrows[currAmmoType.arrowIndex].GetComponent<TrailRenderer>().enabled = false;
            currAmmoType.arrows[currAmmoType.arrowIndex].transform.parent = arrowContainer.transform;
            currAmmoType.arrows[currAmmoType.arrowIndex].GetComponent<ArrowController>().arrowShot = false;
            currAmmoType.arrows[currAmmoType.arrowIndex].GetComponent<Rigidbody>().isKinematic = true;
            currAmmoType.arrows[currAmmoType.arrowIndex].transform.localPosition = new Vector3(0, 0, 0);
            currAmmoType.arrows[currAmmoType.arrowIndex].transform.localEulerAngles = new Vector3(0, 0, -90);
        }
        if(currAmmoType == bombArrow)
        {
            currAmmoType.arrows[currAmmoType.arrowIndex].GetComponent<BombArrowController>().PlaySound();

        }

        /*        currAmmoType.arrows[currAmmoType.arrowIndex].SetActive(true);
                currAmmoType.arrows[currAmmoType.arrowIndex].transform.localEulerAngles = new Vector3(0, 0, -90);
                currAmmoType.arrows[currAmmoType.arrowIndex].transform.localPosition = new Vector3(0, 0, 0);*/
        if (UIController.controller != null)
        {
            UIController.controller.UpdateUI();
        }
    }

    public void AddAmmo(AmmoType ammoType, int amount)
    {
        ammoType.currAmmo += amount;
        if(ammoType.currAmmo > ammoType.maxAmmo)
        {
            ammoType.currAmmo = ammoType.maxAmmo;
        }

        UIController.controller.UpdateUI();
    }

    public override void AttackButtonClicked(InputAction.CallbackContext context)
    {
        if (currAmmoType.currAmmo <= 0)
        {
            return;
        }
        else
        {
            if (context.phase == InputActionPhase.Started && currAmmoType.currAmmo > 0)
            {
                ReadyBow(context);
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                StartCoroutine("fireBow");
            }
        }
    }


    IEnumerator fireBow()
    {
        yield return new WaitForEndOfFrame();
        if (readyToFire)
        {
            PlayerController.player.GetComponent<Animator>().SetTrigger("BowFired");
            FireArrow();
        }
        else
        {
            PlayerController.player.GetComponent<Animator>().SetBool("ReadyBow", false);
        }
    }

    public void ReadyBow(InputAction.CallbackContext context)
    {
        PlayerController.player.GetComponent<Animator>().SetBool("ReadyBow", true);
        superChargeTimer = Time.time;
    }


    public void FireArrow()
    {
        if (Cursor.visible == true)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            if (readyToFire)
            {
                currAmmoType.arrows[currAmmoType.arrowIndex].GetComponent<ArrowController>().enabled = true;
                currAmmoType.arrows[currAmmoType.arrowIndex].GetComponent<Rigidbody>().isKinematic = false;
                currAmmoType.arrows[currAmmoType.arrowIndex].transform.localPosition += new Vector3(0, -.035f, 0.049f);

                currAmmoType.arrows[currAmmoType.arrowIndex].GetComponent<Rigidbody>().transform.parent = null;
                GetComponent<AudioSource>().PlayOneShot(arrowFireSound);
                currAmmoType.arrows[currAmmoType.arrowIndex].GetComponent<BoxCollider>().enabled = true;


                currAmmoType.arrows[currAmmoType.arrowIndex].GetComponent<Rigidbody>().AddForce(PlayerController.player.cam.transform.forward * arrowFireForce);
                currAmmoType.arrows[currAmmoType.arrowIndex].GetComponent<ArrowController>().arrowShot = true;
                currAmmoType.arrows[currAmmoType.arrowIndex].GetComponent<TrailRenderer>().enabled = true;
                PlayerController.player.GetComponent<Animator>().SetBool("BowFired", true);
                PlayerController.player.GetComponent<Animator>().SetBool("ReadyBow", false);
                currAmmoType.currAmmo -= 1;
                UIController.controller.UpdateUI();
                PlayerController.player.cam.DOFieldOfView(60, .1f);

            }

        }
    }

    public void SpawnArrow()
    {
        if (currAmmoType.arrowIndex < currAmmoType.arrows.Count - 1)
        {
            currAmmoType.arrowIndex += 1;
        }
        else
        {
            currAmmoType.arrowIndex = 0;
        }
        currAmmoType.arrows[currAmmoType.arrowIndex].SetActive(true);
        currAmmoType.arrows[currAmmoType.arrowIndex].GetComponent<ArrowController>().arrowHit = false;
        currAmmoType.arrows[currAmmoType.arrowIndex].GetComponent<ArrowController>().enabled = false;
        currAmmoType.arrows[currAmmoType.arrowIndex].GetComponent<ArrowController>().bloodSplatter.SetActive(false);
        currAmmoType.arrows[currAmmoType.arrowIndex].GetComponent<TrailRenderer>().enabled = false;
        currAmmoType.arrows[currAmmoType.arrowIndex].transform.parent = arrowContainer.transform;
        currAmmoType.arrows[currAmmoType.arrowIndex].GetComponent<ArrowController>().arrowShot = false;
        currAmmoType.arrows[currAmmoType.arrowIndex].GetComponent<Rigidbody>().isKinematic = true;
        currAmmoType.arrows[currAmmoType.arrowIndex].transform.localPosition = new Vector3(0, 0, 0);
        currAmmoType.arrows[currAmmoType.arrowIndex].transform.localEulerAngles = new Vector3(0, 0, -90);
    }
}
