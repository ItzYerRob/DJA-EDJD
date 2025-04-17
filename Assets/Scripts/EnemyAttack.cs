using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    [Header("References")]
    private Transform player;
    
    [Header("Attack Settings")]
    public float attackRange = 5f;
    public float attackCooldown = 2f;
    public float damage = 10f;
    
    [Header("Parry Settings")]
    public float parryWindowTime = 0.5f; 
    private bool isAttacking = false;
    private bool isParryWindowOpen = false;
    private bool attackCancelled = false;

    [Header("UI Parry Indicator")]
    public GameObject parryIndicatorPrefab;
    private GameObject parryIndicatorInstance;

    private float distToPlayer;

    [Header("Parry Effects")]
    public GameObject parryParticlesPrefab;
    public AudioClip parrySound;
    private AudioSource audioSource;

    private Rigidbody rb;

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (player == null) return;
        distToPlayer = Vector3.Distance(transform.position, player.position);
        //Check if player is within attack range and no attack is already in progress.
        if (!isAttacking && distToPlayer <= attackRange)
        {
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        attackCancelled = false;

        //Open the parry window and change color to yellow
        isParryWindowOpen = true;

        //Subscribe to the global parry event.
        PlayerEventManager.OnParry += HandleParry;

        if (parryIndicatorPrefab != null && parryIndicatorInstance == null)
        {
            parryIndicatorInstance = Instantiate(parryIndicatorPrefab, transform.position + Vector3.up * 2f, Quaternion.identity);
            parryIndicatorInstance.transform.SetParent(transform); //So it follows the enemy
            parryIndicatorInstance.transform.LookAt(Camera.main.transform);
        }

        //Wait for the duration of the parry window.
        float timer = 0f;
        while (timer < parryWindowTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        //Close the parry window.
        isParryWindowOpen = false;
        PlayerEventManager.OnParry -= HandleParry;

        if (parryIndicatorInstance != null)
        {
            Destroy(parryIndicatorInstance);
        }

        if (attackCancelled)
        {
            // Debug.Log("Attack parried!");
        }
        else
        {
            if(distToPlayer <= attackRange) {
                //Attack lands, apply damage and turn red
                CharacterStats stats = player.GetComponent<CharacterStats>();
                if (stats != null)
                {
                    stats.TakeDamage(damage, damage, 35f);
                }
            }
        }

        //Wait a short time before resetting color
        yield return new WaitForSeconds(0.5f);

        //Wait for the attack cooldown before allowing the next attack.
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    //This method is called when the global parry event is triggered.
    private void HandleParry()
    {
        if (isParryWindowOpen)
        {
            attackCancelled = true;
        }

        //Spawn particles at enemy position
        if (parryParticlesPrefab != null) {  Instantiate(parryParticlesPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);  }
        //Play sound
        if (parrySound != null) { audioSource.PlayOneShot(parrySound); }

        rb.AddForce(Vector3.back * 20f, ForceMode.Impulse);
        
    }

    public void Init(Transform targetPlayer)
    {
        player = targetPlayer;
    }

    private void OnDestroy()
    {
        PlayerEventManager.OnParry -= HandleParry;
    }

}