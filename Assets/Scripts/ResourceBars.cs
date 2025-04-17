using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider sliderHealth, sliderStamina;

    public SpriteRenderer healthBarRenderer, staminaBarRenderer;
    public float originalHealthWidth = 2.0f, originalStaminaWidth = 2.0f;

    public bool AreWeAPlayer = false;

    private void Start()
    {
        //If we aren't a player, scale the health bar sprite.
        if (!AreWeAPlayer)
        {
            if (healthBarRenderer == null) { healthBarRenderer = GetComponent<SpriteRenderer>(); }
            originalHealthWidth = healthBarRenderer.transform.localScale.x;
            healthBarRenderer.color = Color.green;
        }
    }

    public void SetMaxHealth(int maxhealth, CharacterStats characterStats)
    {
        if (AreWeAPlayer) { sliderHealth.maxValue = maxhealth; sliderHealth.value = maxhealth; }
        else
        {
            float newWidth = originalHealthWidth * ((float)maxhealth / (float)characterStats.GetMaxHealth());

            Vector3 newHBarScale = healthBarRenderer.transform.localScale;
            newHBarScale.x = newWidth;
            healthBarRenderer.transform.localScale = newHBarScale;
        }
    }

    public void SetHealth(int health, CharacterStats characterStats)
    {
        if (AreWeAPlayer) { sliderHealth.value = health; }
        else
        {
            float healthPercentage = (float)health / (float)characterStats.GetMaxHealth();
            float newWidth = originalHealthWidth * healthPercentage;

            //Change the color based on health percentage
            healthBarRenderer.color = Color.Lerp(Color.red, Color.green, healthPercentage);

            //Set the local scale of the health bar sprite to adjust its width
            Vector3 newHBarScale = healthBarRenderer.transform.localScale;
            newHBarScale.x = newWidth;
            healthBarRenderer.transform.localScale = newHBarScale;
        }
    }

    public void SetMaxStamina(int maxstamina, CharacterStats characterStats)
    {
        if (AreWeAPlayer) { sliderStamina.maxValue = maxstamina; sliderStamina.value = maxstamina; }
    }

    public void SetStamina(int stamina)
    {
        if (AreWeAPlayer) { sliderStamina.value = stamina; }
    }

}