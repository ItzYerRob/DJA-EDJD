using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    }

    [Header("Power Up Settings")]
    [SerializeField] private List<PowerUp> availablePowerUps = new List<PowerUp>();
    [SerializeField] private int numberOfCardsToShow = 4;
    
    [Header("UI References")]
    [SerializeField] private GameObject cardSelectionPanel;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardsContainer;
    [SerializeField] private Text selectionResultText;
    
    private List<PowerUp> currentPowerUpSelection = new List<PowerUp>();
    private PowerUp worstPowerUp;
    
    [SerializeField] private RoomManager roomManager;
    
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
        for (int i = 0; i < currentPowerUpSelection.Count; i++)
        {
            GameObject card = Instantiate(cardPrefab, cardsContainer);
            CardUI cardUI = card.GetComponent<CardUI>();

            card.transform.position += new Vector3(80*i,0,0);
            
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
        Debug.Log("Aplicando Power Up: " + powerUp.name);
    }
    
    private void ShowResult()
    {
        if (selectionResultText != null)
        {
            selectionResultText.gameObject.SetActive(true);
            selectionResultText.text = "U GOT: " + worstPowerUp.name + "\n" + worstPowerUp.description;
        }
    }
    
    private IEnumerator CloseSelectionPanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        cardSelectionPanel.SetActive(false);
    }
}