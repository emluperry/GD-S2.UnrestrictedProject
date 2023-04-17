using System;
using UnityEngine;

[Serializable]
public class UI_Inventory_Pair
{
    public Scriptable_Card card;
    public GameObject cardObject;
    private UI_OnClickButton button;

    public Action<string> onButtonClicked;

    public void SetupCard()
    {
        UI_Card uiCard = cardObject.GetComponent<UI_Card>();
        uiCard.SetCardValue(card.GetCardPower());
        uiCard.SetImage(card.GetSprite());
    }

    public void StartListeningForEvents()
    {
        button.onButtonClicked += OnClicked;
    }

    public void StopListeningForEvents()
    {
        button.onButtonClicked -= OnClicked;
    }

    private void OnClicked()
    {
        onButtonClicked?.Invoke(card.GetName());
    }

    public void UpdateAmount(int newAmount)
    {
        button.UpdateText(newAmount.ToString());
    }
}