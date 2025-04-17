using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//We're gonna use this as a class for all things with stats
public class CharacterStats : MonoBehaviour
{
    public float maxHealth = 100f, maxStamina = 100f, maxArmor = 0f;
    public float currentHealth, currentStamina, currentArmor;
    public float absorption = 1f;
    public float healthRegen = 0f, staminaRegen = 0f;

    public HealthBar resourceBar;

    public bool AreWeAPlayer = false;
    public bool HaveWeAHealthBar = false;
    
    public float gravityForce;

    public float moveSpeed, rotationSpeed, jumpForce;
    public int maxJumpCount;

    public bool isImmune = false;

    void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        currentArmor = maxArmor;

        if (resourceBar != null) { resourceBar.SetMaxHealth((int)maxHealth, this); } //Our health bar (UI) is player-only
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K) && AreWeAPlayer) { TakeDamage(20, 0, 100000f); } //Debug
        if (Input.GetKeyDown(KeyCode.L) && AreWeAPlayer) { TakeDamage(-20, 0, 100000f); } //Debug

        if (!AreWeAPlayer && currentHealth <= 0) { Destroy(gameObject, 0f); SingletonGameManager.Instance.EnemyDefeated(); }
        //There is currently no gameOver scene, so death is fake news
        //if (AreWeAPlayer && currentHealth <= 0) { SceneManager.LoadScene("GameOverMenu"); }

        if (currentHealth >= maxHealth) { currentHealth = maxHealth; }
        if (currentStamina >= maxStamina) { currentStamina = maxStamina; }
    }

    void FixedUpdate()
    {
        currentHealth += healthRegen/60;
        currentStamina += staminaRegen/60;
        if (resourceBar != null) { 
            resourceBar.SetHealth((int)currentHealth, this);
            resourceBar.SetStamina((int)currentStamina); }
    }

    public void TakeDamage(float piercingdamage, float bluntdamage, float armorpen)
    {
        if(isImmune) {return;}
        
        float damageMultiplier = 1.0f;

        //The whole armor thing is probably overthinked but meh, ill worry about it later

        //Full piercing damage + half of blunt damage if armorpen is greater than or equal to currentArmor
        if (armorpen >= currentArmor)
        {
            bluntdamage = bluntdamage * 0.5f;
            currentHealth -= piercingdamage + bluntdamage;
            Debug.Log(gameObject.name + " took " + piercingdamage + " piercing damage and " + bluntdamage + " blunt damage.");
        }
        else
        {
            //First, we get a damage mult from the armorpen divided by entity's current armor
            damageMultiplier = Mathf.Max(0.2f, armorpen / currentArmor) * 1.0f; //Careful with currentArmor if it equals 0, since dividing by zero will likely attribute infinity. Since its minimum is 0.2 however, i guess its theoretically not a problem?
            //Then we square root the piercing damage and multiply it by damage mult
            piercingdamage = Mathf.Pow(piercingdamage, 1f / 3f) * damageMultiplier;
            //And we multiply blunt damage by absorption and damage mult
            bluntdamage = (bluntdamage * absorption) * damageMultiplier;
            currentHealth -= piercingdamage + bluntdamage;
            Debug.Log(gameObject.name + " took " + piercingdamage + " piercing damage and " + bluntdamage + " blunt damage.");
        }

        if (HaveWeAHealthBar) { resourceBar.SetHealth((int)currentHealth, this); }
    }

    public void TakeStaminaDamage(float value)
    {
        currentStamina -= value;
        if (HaveWeAHealthBar) { resourceBar.SetStamina((int)currentStamina); }
    }

    public float GetMaxHealth()
    { return maxHealth; }

}
