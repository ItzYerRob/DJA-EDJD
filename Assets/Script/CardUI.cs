using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [SerializeField] private Image cardBackImage;
    [SerializeField] private Image powerUpIconImage;
    [SerializeField] private Text powerUpNameText;
    [SerializeField] private Text powerUpDescriptionText;
    [SerializeField] public Image cardBorder;
    
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
        
        // Adicionar uma borda para o efeito de brilho se ela não existir
        if (cardBorder == null)
        {
            GameObject borderObj = new GameObject("CardBorder");
            borderObj.transform.SetParent(transform);
            
            RectTransform rt = borderObj.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(-5, -5);
            rt.offsetMax = new Vector2(5, 5);
            rt.SetAsFirstSibling(); // Colocar atrás de tudo
            
            cardBorder = borderObj.AddComponent<Image>();
            cardBorder.sprite = cardBackImage.sprite; // Usar o mesmo sprite como base
            cardBorder.color = new Color(1f, 1f, 1f, 0f); // Inicialmente invisível
        }
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