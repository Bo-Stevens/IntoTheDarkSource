using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject bow;
    public GameObject sword;
    public GameObject animatorOnlySword;

    public float walkSpeed;
    public float runSpeed;
    float moveSpeed;
    float moveSpeedModifier;
    public float jumpForce;

    [Range(0.0f, 1.0f)]
    public float verticalSensitivity;
    [Range(0.0f, 1.0f)]
    public float horizontalSensitivity;

    public float cameraSensitivity;
    public float cameraMaxRotationZ;
    public float cameraMinRotationZ;
    public float spineMaxRotationZ;
    public float spineMinRotationZ;

    [SerializeField]
    float maxHealth;
    [HideInInspector]
    float health;

    [SerializeField]
    int maxMagic;

    [HideInInspector]
    public int currentMagic;

    [SerializeField]
    public float maxStamina;
    [HideInInspector]
    public float stamina;
    float staminaRegenTimer;
    [SerializeField]
    float timeToWaitForStaminaRegen;
    [SerializeField]
    float staminaRegenPerSecond;

    [HideInInspector]
    public int numPotions;
    public int maxNumPotions;

    public Camera cam;
    public GameObject head;
    public GameObject spine;
    public GameObject spell;


    Vector2 movementDirection;
    Vector2 lookingDirection;

    [SerializeField]
    GameObject fakeArrow;

    public FootStepController footstepController;

    public static PlayerController player;

    bool isGrounded = true;
    bool dead = false;
    float jumpCheckDelay = 5f;
    float jumpCheckTimer = 0f;

    Spell activeSpell;

    
    public Spell[] spells;


    public GameObject potion;

    public AudioClip catchBreath;

    public bool parrying;

    // Start is called before the first frame update
    void Awake()
    {
        player = this;
        GetComponent<PlayerInput>().enabled = false;
        GetComponent<PlayerInput>().enabled = true;
        health = maxHealth;
        currentMagic = maxMagic;
        stamina = maxStamina;
        moveSpeed = walkSpeed;
        numPotions = 1;
        moveSpeedModifier = 1;
        activeSpell = spells[1];
        parrying = false;

        AudioListener.volume = 1.0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
        if (!isGrounded )
        {
            if(jumpCheckTimer >= jumpCheckDelay)
            {
                CheckForGround();
            }
            else
            {
                jumpCheckTimer += 1;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "CrouchUnderObject" && GetComponent<Animator>().GetBool("Crouching") && !GetComponent<Animator>().GetBool("CrouchingUnderObject"))
        {
            GetComponent<Animator>().SetBool("CrouchingUnderObject", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "CrouchUnderObject")
        {
            GetComponent<Animator>().SetBool("CrouchingUnderObject", false);
        }
    }

    void CheckForGround()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, -transform.up, out hit))
        {
            if(Vector3.Distance(hit.point, transform.position) < 0.95f && GetComponent<Rigidbody>().velocity.y < 0f)
            {
                isGrounded = true;
                jumpCheckTimer = 0;
            }
        }
    }

    public void FootStep()
    {
        if (isGrounded)
        {
            if(moveSpeed == runSpeed)
            {
                GetComponent<AudioSource>().PlayOneShot(footstepController.footStepSound, 0.2f);
            }
            else
            {
                GetComponent<AudioSource>().PlayOneShot(footstepController.footStepSound, 0.1f);
            }
        }
    }

    public void Move()
    {
        if (GetComponent<Animator>().GetBool("Dead"))
        {
            return;
        }
        Vector3 newPosition;
        Vector3 camForward = cam.transform.forward;
        Vector3 camRight = cam.transform.right;

        camForward.y = 0;
        camRight.y = 0;
        camForward = camForward.normalized;
        camRight = camRight.normalized;

        //Position
        newPosition = new Vector3(movementDirection.x, 0, movementDirection.y).normalized * moveSpeed * moveSpeedModifier;
        Vector3 movementDir = (newPosition.x * camRight + newPosition.z * camForward) * Time.deltaTime;

        transform.position += movementDir;
        
        if (movementDirection != Vector2.zero)
        {
            GetComponent<Animator>().SetBool("Walking", true);
        }
        else
        {
            GetComponent<Animator>().SetBool("Walking", false);
        }


        //Stamina Management
        if (moveSpeed == runSpeed && movementDir != Vector3.zero)
        {
            UIController.controller.UpdateUI();
            stamina -= Time.deltaTime * 14f;
            if (stamina <= 0)
            {
                IEnumerator slow = SlowMovement(2f);
                StartCoroutine(slow);
                GetComponent<AudioSource>().PlayOneShot(catchBreath);
                moveSpeed = walkSpeed;
            }
        }
        else if (stamina < maxStamina)
        {
            UIController.controller.UpdateUI();
            staminaRegenTimer += Time.deltaTime;
            if(staminaRegenTimer >= timeToWaitForStaminaRegen)
            {
                stamina += Time.deltaTime * staminaRegenPerSecond;
                if(stamina > maxStamina)
                {
                    stamina = maxStamina;
                    staminaRegenTimer = 0f;
                    UIController.controller.staminaBar.GetComponent<StaminaBarController>().HideStaminaBar();
                }
            }
        }
    }

    public void PlaySwordSound()
    {
        sword.GetComponent<SwordController>().PlayAttackSound();
    }
    public void BowReadied()
    {
        bow.GetComponent<BowController>().readyToFire = true;
    }

    public void ArrowGrabbed()
    {
        PlayerController.player.GetComponent<Animator>().SetBool("BowFired", false);
        GetComponent<Animator>().SetTrigger("ArrowGrabbed");
        if (bow.GetComponent<BowController>().currAmmoType.currAmmo > 0)
        {
            bow.GetComponent<BowController>().SpawnArrow();
        }
        fakeArrow.SetActive(false);
    }

    public void SpawnArrow()
    {
        if(bow.GetComponent<BowController>().currAmmoType.currAmmo > 0)
        {
            fakeArrow.SetActive(true);
        }
    }

    public void ApplyDamage(float damageAmount)
    {
        if (GetComponent<Animator>().GetBool("Dead"))
        {
            return;
        }
        GetComponent<Animator>().SetBool("ReadyBow", false);
        bow.GetComponent<BowController>().readyToFire = false;
        health -= damageAmount;
        UIController.controller.UpdateUI();

        IEnumerator routine =  SlowMovement(2f);
        StartCoroutine(routine);
        StartCoroutine("FlashHitEffect");
        if(health <= 0 )
        {
            DOTween.To(() => AudioListener.volume, x => AudioListener.volume = x, 0f, 1.5f);
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<CapsuleCollider>().enabled = false;
            GetComponent<Animator>().SetBool("Dead", true);
        }
    }

    public void KillPlayer()
    {
        EnemyController.allEnemies = null;
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.UnloadSceneAsync(scene.name);
        SceneManager.LoadScene(scene.name);
    }

    public void FadeToBlack()
    {
        UIController.controller.FadeToBlack();
    }

    IEnumerator SlowMovement(float multiplier)
    {
        moveSpeedModifier = 1f / multiplier;

        yield return new WaitForSeconds(0.5f);

        DOTween.To(() => moveSpeedModifier, x => moveSpeedModifier = x, 1, 1.5f);
    }

    IEnumerator FlashHitEffect()
    {
        UIController.controller.hitScreenEffect.SetActive(true);
        Image hitEffect = UIController.controller.hitScreenEffect.GetComponent<Image>();
        hitEffect.color = new Color(hitEffect.color.r, hitEffect.color.g, hitEffect.color.b, 0);

        DOTween.To(() => hitEffect.color.a,
            x => hitEffect.color = new Color(hitEffect.color.r, hitEffect.color.g, hitEffect.color.b, x),
            0.1f, 0.05f);
        yield return new WaitForSeconds(0.05f);
        DOTween.To(() => hitEffect.color.a,
            x => hitEffect.color = new Color(hitEffect.color.r, hitEffect.color.g, hitEffect.color.b, x),
            0f, 0.25f);
        yield return new WaitForSeconds(0.3f);
        UIController.controller.hitScreenEffect.SetActive(false);

        //DOTween.To(() => UIController.controller.hitScreenEffect.GetComponent<Image>().color.a, x => UIController.controller.hitScreenEffect.GetComponent<Image>().color = new Color(1,1,1,x), 1.5f);
    }
    IEnumerator RegenMagicOverTime()
    {
        int endMagic = currentMagic + potion.GetComponent<Potion>().restoreAmount;
        float endHealth = health + potion.GetComponent<Potion>().restoreAmount / 2f;
        float waitTime = 0.1f;
        if(endMagic > maxMagic)
        {
            endMagic = maxMagic;
        }
        if(endHealth > maxHealth)
        {
            endHealth = maxHealth;
        }

        float increaseAmount = (endMagic - currentMagic) / 15f;
        while (currentMagic < endMagic)
        {
            yield return new WaitForSeconds(waitTime);
            waitTime *= 0.925f;
            SetMagic((int) (currentMagic + increaseAmount));

            if(health < maxHealth && health < health + endHealth)
            {
                health += Mathf.Floor(increaseAmount);
            }
        }
        SetMagic(endMagic);
        health = endHealth;
    }

    public float GetHealth()
    {
        return health;
    }

    public void SetMagic(int amount)
    {
        if(amount > maxMagic)
        {
            currentMagic = maxMagic;
        }
        else
        {
            currentMagic = amount;
        }
        UIController.controller.UpdateUI();
    }

    public void DoneUsingItem()
    {
        GetComponent<Animator>().SetBool("UsingItem", false);
    }

    public void ActivatePotionEffect()
    {
        potion.SetActive(false);
        StartCoroutine("RegenMagicOverTime");
    }

    public void AimCamera()
    {
        //Camera
        Vector3 lookDir = (new Vector3(-lookingDirection.x * horizontalSensitivity, 0, -lookingDirection.y * verticalSensitivity) * Time.deltaTime * cameraSensitivity);
        Quaternion newRotation = Quaternion.Euler(new Vector3(lookDir.x + head.transform.localEulerAngles.x, head.transform.localEulerAngles.y, lookDir.z));

        transform.localRotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.root.eulerAngles.y - newRotation.eulerAngles.x, transform.rotation.eulerAngles.z));


        //Handles Vertical head/torso movement
        float zVal = Utility.GetProperEulerVal(newRotation.eulerAngles.z);


        if (Utility.GetProperEulerVal(spine.transform.localEulerAngles.z) + zVal > spineMaxRotationZ)
        {
            spine.transform.localEulerAngles = new Vector3(0, 0, spineMaxRotationZ);
        }
        else if (Utility.GetProperEulerVal(spine.transform.localEulerAngles.z) + zVal < spineMinRotationZ)
        {
            spine.transform.localEulerAngles = new Vector3(0, 0, spineMinRotationZ);
        }
        else
        {
            spine.transform.localEulerAngles += new Vector3(0, 0, zVal);
        }


        /*        float headRotZ = Utility.GetProperEulerVal(head.transform.localEulerAngles.z);
                spine.transform.localEulerAngles += new Vector3(0, 0, zVal);*/
    }

    public void ActivateAnimatorSword()
    {
        sword.SetActive(false);
        animatorOnlySword.SetActive(true);
    }

    public void DeactivateAnimatorSword()
    {
        sword.SetActive(true);
        animatorOnlySword.SetActive(false);
    }

    public void SnapFingers()
    {
        if (DarkVisionSpell.instance == null)
        {
            Instantiate(spells[1], transform.position, transform.rotation, null);
        }
        else
        {
            DarkVisionSpell.instance.StartCoroutine("DisableSpell");
        }
        GetComponent<AudioSource>().PlayOneShot(spells[1].GetComponent<Spell>().castSound);
    }

    public void UseMagicAbility(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && currentMagic > 0 && currentMagic >= spell.GetComponent<Spell>().spellCost)
        {
            GetComponent<Animator>().SetBool("CastingSpell", true);
            if (activeSpell.GetComponent<DarkVisionSpell>() != null )
            {
                GetComponent<Animator>().SetTrigger("DarkVision");
            }
            else
            {
                currentMagic -= (int)activeSpell.GetComponent<Spell>().spellCost;
                GetComponent<Animator>().SetTrigger("HoldSpell");
                GetComponent<AudioSource>().PlayOneShot(spells[0].GetComponent<Spell>().castSound);
                Instantiate(activeSpell, transform.position, transform.rotation, null);
            }
            UIController.controller.UpdateUI();
        }
    }
    public void FireArrow(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started && context.phase != InputActionPhase.Canceled)
        {
            return;
        }

        if (Cursor.visible == true)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (bow.activeSelf)
        {
            bow.GetComponent<BowController>().AttackButtonClicked(context);
        }
        else if (sword.activeSelf)
        {
            sword.GetComponent<SwordController>().AttackButtonClicked(context);
        }
    }
    public void Crouch(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            GetComponent<Animator>().SetBool("Crouching", true);
        }
        if (context.phase == InputActionPhase.Canceled)
        {
            GetComponent<Animator>().SetBool("Crouching", false);
        }
    }
    public void UseAction(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started)
        {
            return;
        }


        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
        {
            if (Vector3.Distance(transform.position, hit.point) > 2.5f)
            {
                return;
            }

            if(hit.collider.GetComponent<Pickup>() != null)
            {
                hit.collider.GetComponent<Pickup>().GetItem();
                UIController.controller.UpdateUI();
            }
            else if(hit.collider.GetComponent<GateSwitch>() != null)
            {
                hit.collider.GetComponent<GateSwitch>().SwitchActivated();
            }
        }
    }
    public void Scroll(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (context.ReadValue<float>() > 0)
            {
                bow.GetComponent<BowController>().SetActiveArrowType(bow.GetComponent<BowController>().bombArrow);
            }
            else
            {
                bow.GetComponent<BowController>().SetActiveArrowType(bow.GetComponent<BowController>().normalArrow);
            }
        }
    }
    public void Move(InputAction.CallbackContext context)
    {
        movementDirection = context.ReadValue<Vector2>();
    }
    public void Lean(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started && context.phase != InputActionPhase.Canceled)
        {
            return;
        }

        float value = context.ReadValue<float>();

        if(value > 0)
        {
            GetComponent<Animator>().SetBool("LeanRight", true);
            GetComponent<Animator>().SetBool("LeanLeft", false);
        }
        else if(value < 0)
        {
            GetComponent<Animator>().SetBool("LeanLeft", true);
            GetComponent<Animator>().SetBool("LeanRight", false);
        }
        else
        {
            GetComponent<Animator>().SetBool("LeanLeft", false);
            GetComponent<Animator>().SetBool("LeanRight", false);
        }
    }
    public void Look(InputAction.CallbackContext context)
    {
        if(Cursor.visible != true)
        {
            lookingDirection = context.ReadValue<Vector2>();
            AimCamera();
        }

    }
    public void SwitchWeapons(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started)
        {
            return;
        }
        if (GetComponent<Animator>().GetBool("Melee"))
        {
            GetComponent<Animator>().SetBool("Melee", false);
        }
        else
        {
            GetComponent<Animator>().SetBool("Melee", true);
        }
    }
    public void SwitchToFirstSlot(InputAction.CallbackContext context)
    {
        activeSpell = spells[0];
        UIController.controller.spellIcons[0].SetActive(true);
        UIController.controller.spellIcons[1].SetActive(false);
        if (context.phase == InputActionPhase.Started && isGrounded)
            GetComponent<PlayerSoundController>().SpellSwitchClick();
    }
    public void SwitchToSecondSlot(InputAction.CallbackContext context)
    {
        activeSpell = spells[1];
        UIController.controller.spellIcons[1].SetActive(true);
        UIController.controller.spellIcons[0].SetActive(false);
        if (context.phase == InputActionPhase.Started && isGrounded)
            GetComponent<PlayerSoundController>().SpellSwitchClick();
    }
    public void Jump(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started && isGrounded)
        {
            isGrounded = false;
            GetComponent<Rigidbody>().AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
        }
    }
    public void DrinkPotion(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && numPotions > 0)
        {
            potion.SetActive(true);
            GetComponent<Animator>().SetTrigger("DrinkingPotion");
            GetComponent<Animator>().SetBool("UsingItem", true);
            numPotions -= 1;
        }
    }
    public void CancelAction(InputAction.CallbackContext context)
    {
        if(context.phase != InputActionPhase.Started)
        {
            return;
        }

        if (bow.activeSelf)
        {
            if (isGrounded && GetComponent<Animator>().GetBool("ReadyBow"))
            {
                cam.DOFieldOfView(60, .1f);
                GetComponent<Animator>().SetBool("ReadyBow", false);
            }
        }
        else
        {
            GetComponent<Animator>().SetBool("Parry", true);
        }
    }
    public void EscapeEditorWindow(InputAction.CallbackContext context)
    {
        lookingDirection = Vector2.zero;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void Sprint(InputAction.CallbackContext context)
    {
        if(context.ReadValue<float>() <= 0)
        {
            moveSpeed = walkSpeed;
            GetComponent<Animator>().SetFloat("WalkSpeed", 0.8f);
        }
        else
        {
            if (context.started)
            {
                staminaRegenTimer = 0f;
                moveSpeed = runSpeed; 
                GetComponent<Animator>().SetFloat("WalkSpeed", 1.25f);
            }
        }

    }
}