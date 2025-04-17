using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [SerializeField] private Image cardBackImage;
    [SerializeField] private Image powerUpIconImage;
    [SerializeField] private Text powerUpNameText;
    [SerializeField] private Text powerUpDescriptionText;
    
    private PowerUpSelection.PowerUp powerUpData;
    
    public void SetupCard(PowerUpSelection.PowerUp powerUp, bool revealed)
    {
        powerUpData = powerUp;
        
        if (powerUpIconImage != null)
            powerUpIconImage.sprite = powerUp.powerUpIcon;
            
        if (powerUpNameText != null)
            powerUpNameText.text = powerUp.name;
            
        if (powerUpDescriptionText != null)
            powerUpDescriptionText.text = powerUp.description;
        
        SetCardRevealed(revealed);
    }
    
    public void RevealCard()
    {
        SetCardRevealed(true);
    }
    
    private void SetCardRevealed(bool revealed)
    {
        if (cardBackImage != null)
            cardBackImage.gameObject.SetActive(!revealed);
            
        if (powerUpIconImage != null)
            powerUpIconImage.gameObject.SetActive(revealed);
            
        if (powerUpNameText != null)
            powerUpNameText.gameObject.SetActive(revealed);
            
        if (powerUpDescriptionText != null)
            powerUpDescriptionText.gameObject.SetActive(revealed);
    }
}