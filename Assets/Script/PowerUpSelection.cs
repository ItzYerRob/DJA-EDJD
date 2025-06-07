using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PowerUpSelection : MonoBehaviour
{
    [System.Serializable]
    public class PowerUp
    {
        public string name;
        public string description;
        public Sprite cardImage;
        public Sprite powerUpIcon;
        [Range(1, 10)]
        public int powerValue;

        public void Apply(CharacterStats stats)
        {
            switch (name)
            {
                case "PHealthMStamina":
                    stats.maxHealth += 10;
                    stats.currentHealth = Mathf.Min(stats.currentHealth + 10, stats.maxHealth);

                    stats.maxStamina = Mathf.Max(0, stats.maxStamina - 10);
                    stats.currentStamina = Mathf.Min(stats.currentStamina, stats.maxStamina);
                    break;

                case "PStaminaMHealth":
                    stats.maxStamina += 10;
                    stats.currentStamina = Mathf.Min(stats.currentStamina + 10, stats.maxStamina);

                    stats.maxHealth = Mathf.Max(0, stats.maxHealth - 5);
                    stats.currentHealth = Mathf.Min(stats.currentHealth, stats.maxHealth);
                    break;

                case "PArmorMRegen":
                    stats.maxArmor += 3;
                    stats.currentArmor = Mathf.Min(stats.currentArmor + 3, stats.maxArmor);

                    stats.healthRegen = Mathf.Max(0f, stats.healthRegen - 0.5f);
                    stats.staminaRegen = Mathf.Max(0f, stats.staminaRegen - 0.5f);
                    break;

                case "PSpeedBoostMHealth":
                    stats.moveSpeed += 2f;

                    stats.maxHealth = Mathf.Max(0, stats.maxHealth - 5);
                    stats.currentHealth = Mathf.Min(stats.currentHealth, stats.maxHealth);
                    break;

                case "PJumpBoostMHealth":
                    stats.maxJumpCount += 1;

                    stats.maxHealth = Mathf.Max(0, stats.maxHealth - 10);
                    stats.currentHealth = Mathf.Min(stats.currentHealth, stats.maxHealth);
                    break;

                case "PRegenBoostMArmor":
                    stats.healthRegen += 0.5f;
                    stats.staminaRegen += 0.5f;

                    stats.maxArmor = Mathf.Max(0, stats.maxArmor - 2);
                    stats.currentArmor = Mathf.Max(0, Mathf.Min(stats.currentArmor - 2, stats.maxArmor));
                    break;

                default:
                    Debug.LogWarning($"PowerUp.Apply: Unknown power-up '{name}'");
                    break;
            }
        }

    }

    [Header("Power Up Settings")]
    [SerializeField] private List<PowerUp> availablePowerUps = new List<PowerUp>();
    [SerializeField] private int numberOfCardsToShow = 10;
    
    [Header("UI References")]
    [SerializeField] private GameObject cardSelectionPanel;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardsContainer;
    [SerializeField] private Text selectionResultText;
    
    [Header("Card Layout")]
    [SerializeField] private float horizontalSpacing = 120f;
    [SerializeField] private float semicircleHeight = 50f; 
    [SerializeField] private float maxRotationAngle = 15f; 
    [SerializeField] private float hoverElevation = 30f;
    [SerializeField] private Color hoverGlowColor = new Color(1f, 0.8f, 0.2f, 1f);
    
    private List<PowerUp> currentPowerUpSelection = new List<PowerUp>();
    private PowerUp worstPowerUp;
    
    [SerializeField] private RoomManager roomManager;

    public CharacterStats playerStats;
    
    private void Start()
    {
        if (cardSelectionPanel != null)
            cardSelectionPanel.SetActive(false);

        if (roomManager != null)
            roomManager.OnRoomCleared += ShowPowerUpSelection;
    }
    
    private void OnDestroy()
    {
        if (roomManager != null)
            roomManager.OnRoomCleared -= ShowPowerUpSelection;
    }
    
    public void ShowPowerUpSelection()
    {
        foreach (Transform child in cardsContainer)
        {
            Destroy(child.gameObject);
        }
        
        currentPowerUpSelection.Clear();
        
        SelectRandomPowerUps();
        
        FindWorstPowerUp();
        
        CreateCardUI();
        
        cardSelectionPanel.SetActive(true);
        
        if (selectionResultText != null)
            selectionResultText.gameObject.SetActive(false);
    }
    
    private void SelectRandomPowerUps()
    {
        List<PowerUp> tempPowerUps = new List<PowerUp>(availablePowerUps);
        
        int count = Mathf.Min(numberOfCardsToShow, tempPowerUps.Count);
        
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, tempPowerUps.Count);
            currentPowerUpSelection.Add(tempPowerUps[randomIndex]);
            tempPowerUps.RemoveAt(randomIndex);
        }
    }
    
    private void FindWorstPowerUp()
    {
        worstPowerUp = currentPowerUpSelection[0];
        
        foreach (var powerUp in currentPowerUpSelection)
        {
            if (powerUp.powerValue < worstPowerUp.powerValue)
            {
                worstPowerUp = powerUp;
            }
        }
    }
    
    private void CreateCardUI()
    {
        int totalCards = currentPowerUpSelection.Count;
        
        // Calcular a largura total das cartas
        float totalWidth = (totalCards - 1) * horizontalSpacing;
        float startX = -totalWidth * 0.5f; // Começar do lado esquerdo centralizado
        
        for (int i = 0; i < currentPowerUpSelection.Count; i++)
        {
            GameObject card = Instantiate(cardPrefab, cardsContainer);
            CardUI cardUI = card.GetComponent<CardUI>();
            
            // Posição horizontal linear
            float xPosition = startX + i * horizontalSpacing;
            
            // Calcular a posição Y para formar um semicírculo sutil
            // Usar uma função quadrática para criar a curva
            float normalizedPosition = (float)i / (totalCards - 1); // 0 a 1
            float curvePosition = (normalizedPosition - 0.5f) * 2f; // -1 a 1
            float yOffset = semicircleHeight * (1f - curvePosition * curvePosition); // Parábola invertida
            
            Vector3 position = new Vector3(xPosition, yOffset, 0);
            card.GetComponent<RectTransform>().anchoredPosition = position;
            
            // Rotação sutil baseada na posição
            float rotationZ = curvePosition * maxRotationAngle;
            card.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, rotationZ);
            
            // Adicionar efeitos de hover
            CardHoverEffect hoverEffect = card.AddComponent<CardHoverEffect>();
            hoverEffect.Initialize(position, hoverElevation, rotationZ, hoverGlowColor);
            
            if (cardUI != null)
            {
                cardUI.SetupCard(currentPowerUpSelection[i], false);
                
                int index = i;
                Button cardButton = card.GetComponent<Button>();
                if (cardButton != null)
                {
                    cardButton.onClick.AddListener(() => OnCardSelected(index));
                }
            }
        }
    }
    
    private void OnCardSelected(int cardIndex)
    {
        foreach (Transform child in cardsContainer)
        {
            Button button = child.GetComponent<Button>();
            if (button != null)
                button.interactable = false;
        }
        
        RevealAllCards();
        
        ApplyPowerUp(worstPowerUp);
        
        ShowResult();
        
        StartCoroutine(CloseSelectionPanelAfterDelay(3.0f));
    }
    
    private void RevealAllCards()
    {
        CardUI[] cardUIs = cardsContainer.GetComponentsInChildren<CardUI>();
        for (int i = 0; i < cardUIs.Length; i++)
        {
            cardUIs[i].RevealCard();
        }
    }
    
    private void ApplyPowerUp(PowerUp powerUp)
    {
        powerUp.Apply(playerStats);

        Debug.Log($"Applied PowerUp: {powerUp.name}");
    }
    
    private void ShowResult()
    {
        if (selectionResultText != null)
        {
            selectionResultText.gameObject.SetActive(true);
            selectionResultText.text = "Você recebeu: " + worstPowerUp.name + "\n" + worstPowerUp.description;
        }
    }
    
    private IEnumerator CloseSelectionPanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        cardSelectionPanel.SetActive(false);
    }
}