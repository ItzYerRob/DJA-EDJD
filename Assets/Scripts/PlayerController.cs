using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform groundCheck;
    public LayerMask groundMask;

    private Rigidbody rb;
    private bool isGrounded, isWalking;
    public float groundDamping = 1f;

    public float dashForce = 15f; 

    public float dashCooldown = 2f, dodgeCooldown = 1f;
    private bool canDash = true, canDodge = true;

    private CharacterStats charStats;
    private BodyController bodyController;


    public float pushRange = 2f;
    public float knockbackDealt = 1f;
    public float pushRadius = 1f;
    public LayerMask enemyLayer;
    private FactionHandler factionHandler;

    public Animator attackAnimator;

    private bool isAttacking = false;
    private bool queueNextAttack = false;

    public AudioSource audioSource;
    public AudioClip attackSound, attackWhoosh;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        charStats = GetComponent<CharacterStats>();
        bodyController = GetComponent<BodyController>();
        factionHandler = GetComponent<FactionHandler>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Update()
    {
        isGrounded = bodyController.isGrounded;
        isWalking = bodyController.isWalking;
        CombatHandler();
    }

    private void FixedUpdate()
    {
        
    }

    void CombatHandler() {
        //M1- Attack
        if (Input.GetButtonDown("Fire1"))
        {
            if (isAttacking)
            {
                queueNextAttack = true; //Queue next attack if already attacking
            }
            else
            {
                if (attackAnimator != null)
                {
                    PlayerEventManager.TriggerAttack();
                    StartCoroutine(AttackCombo());
                }
            }
        }

        //M2- Block/Parry
        if (Input.GetButtonDown("Fire2"))
        {
            if(charStats.currentStamina<10) return;
            charStats.TakeStaminaDamage(10);
            
            //Trigger the global parry event.
            PlayerEventManager.TriggerParry();
            Debug.Log("Parry attempted!");
        }

        //M3- Dash Attack
        if (Input.GetMouseButtonDown(2)) 
        {
            if (!canDash) return;
            if (charStats.currentStamina < 50) return;

            charStats.TakeStaminaDamage(50);
            Dash();
            StartCoroutine(DashCooldownRoutine());
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (!canDodge) return;
            if (charStats.currentStamina < 40) return;
            charStats.TakeStaminaDamage(40);

            Vector3 inputDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
            inputDir = Camera.main.transform.TransformDirection(inputDir);
            inputDir.y = 0f;
            inputDir.Normalize();

            Dodge(inputDir);
            StartCoroutine(DodgeCooldownRoutine());
        }


        //G- Push
        if (Input.GetKeyDown(KeyCode.G)) {
            Attack(0f,0f,0f,10f,51.5f,10f);
        }
    }

    void Dash()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;

        //Get the camera's forward direction and flatten y value
        Vector3 flatForward = mainCamera.transform.forward;
        flatForward.y = 0f;
        flatForward.Normalize();

        //Get the vertical offset of the mouse relative to screen center
        float screenHeight = Screen.height;
        float mouseY = Input.mousePosition.y;
        float yOffset = (mouseY - screenHeight / 2f) / (screenHeight / 2f); //normalized range [-1, 1]

        //Scale the upward momentum
        float upwardForce = yOffset * 0.5f;

        //Create the dash direction
        Vector3 dashDirection = flatForward + Vector3.up * upwardForce;
        dashDirection.Normalize();

        //Apply dash force
        rb.linearVelocity = dashDirection * dashForce;

        //Trigger the dash event
        PlayerEventManager.TriggerDash();
        Debug.Log("Dashed with horizontal forward and vertical mouse offset!");
    }

    void Dodge(Vector3 inputDirection)
    {
        if (inputDirection == Vector3.zero)
        {
            //Fallback to backwards dodge if no input
            inputDirection = -transform.forward;
        }

        inputDirection.Normalize();

        //Add upward offset
        Vector3 dodgeDirection = inputDirection + Vector3.up * 0.5f;
        dodgeDirection.Normalize();

        rb.linearVelocity = dodgeDirection * dashForce/1.5f; 

        PlayerEventManager.TriggerDodge();
        Debug.Log("Dodged towards input direction with upward lift!");
    }


    void Attack(float piercingDmg, float bluntDmg, float armorPen, float pushForce, float verticalForce, float staminaCost)
    {
        if(charStats.currentStamina < staminaCost) return;
        charStats.TakeStaminaDamage(staminaCost);

        audioSource.PlayOneShot(attackWhoosh);
        
        Vector3 origin = transform.position + Vector3.up * 1f; //Raise the ray origin slightly
        Vector3 direction = transform.forward;

        RaycastHit[] hits = Physics.SphereCastAll(origin, pushRadius, direction, pushRange, enemyLayer);

        foreach (RaycastHit hit in hits)
        {
            GameObject target = hit.collider.gameObject;

            if (target == gameObject) continue; 

            FactionHandler targetFaction = target.GetComponent<FactionHandler>();
            Rigidbody targetRb = target.GetComponent<Rigidbody>();
            CharacterStats targetStats = target.GetComponent<CharacterStats>();

            if (targetFaction != null)
            {
                if (factionHandler.IsEnemy(targetFaction.CurrentFaction))
                {
                    if(targetRb != null) {
                        //Push direction with vertical component
                        Vector3 pushDir = (target.transform.position - transform.position).normalized;
                        pushDir.y += verticalForce; //Vertical knockback component, doesn't seem to currently work properly for some reason?

                        targetRb.AddForce(pushDir.normalized * pushForce, ForceMode.Impulse);

                        Debug.Log("Pushed enemy with vertical force: " + target.name);
                    }

                    if (targetStats != null) {
                        if(piercingDmg > 0 || bluntDmg > 0) {
                            targetStats.TakeDamage(piercingDmg, bluntDmg, armorPen);
                        }
                    }

                    audioSource.PlayOneShot(attackSound);
                }
            }
        }
    }

    IEnumerator AttackCombo()
    {
        isAttacking = true;
        string[] comboAnims = { "2Rights", "Uppercut", "Inboxing" };

        int comboIndex = 0;

        while (comboIndex < comboAnims.Length)
        {
            queueNextAttack = false;

            attackAnimator.Play(comboAnims[comboIndex]);
            PlayerEventManager.TriggerAttack();
            Attack(50f,50f,1f,30f,0f,20f);

            float animTime = attackAnimator.GetCurrentAnimatorStateInfo(0).length * 0.9f;
            float timer = 0f;

            while (timer < animTime)
            {
                if (queueNextAttack) break;
                timer += Time.deltaTime;
                yield return null;
            }

            if (queueNextAttack)
            {
                comboIndex++;
            }
            else
            {
                break; //No queued attack, end combo
            }
        }

        isAttacking = false;
    }

    IEnumerator DashCooldownRoutine()
    {
        canDash = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    IEnumerator DodgeCooldownRoutine()
    {
        canDodge = false;
        yield return new WaitForSeconds(dodgeCooldown);
        canDodge = true;
    }



}
