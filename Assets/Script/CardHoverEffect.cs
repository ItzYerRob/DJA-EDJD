using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardHoverEffect : MonoBehaviour
{
    private Vector3 originalPosition;
    private float originalRotation;
    private float elevationAmount;
    private Color glowColor;
    private Image borderImage;
    
    public void Initialize(Vector3 position, float elevation, float rotation, Color glow)
    {
        originalPosition = position;
        originalRotation = rotation;
        elevationAmount = elevation;
        glowColor = glow;
        
        // Obter a referência à borda
        borderImage = GetComponentInChildren<CardUI>()?.cardBorder;
        
        // Adicionar eventos de hover
        EventTrigger trigger = gameObject.AddComponent<EventTrigger>();
        
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) => { OnPointerEnter(); });
        trigger.triggers.Add(entryEnter);
        
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => { OnPointerExit(); });
        trigger.triggers.Add(entryExit);
    }
    
    private void OnPointerEnter()
    {
        // Elevar a carta
        RectTransform rt = GetComponent<RectTransform>();
        rt.anchoredPosition = originalPosition + new Vector3(0, elevationAmount, 0);
        
        // Certificar-se de que a rotação é mantida
        rt.rotation = Quaternion.Euler(0, 0, originalRotation);
        
        // Ativar o brilho na borda
        if (borderImage != null)
        {
            borderImage.color = glowColor;
        }
    }
    
    private void OnPointerExit()
    {
        // Retornar à posição original
        RectTransform rt = GetComponent<RectTransform>();
        rt.anchoredPosition = originalPosition;
        
        // Certificar-se de que a rotação é mantida
        rt.rotation = Quaternion.Euler(0, 0, originalRotation);
        
        // Desativar o brilho
        if (borderImage != null)
        {
            borderImage.color = new Color(1f, 1f, 1f, 0f);
        }
    }
}