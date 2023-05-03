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
        uiCard.SetCardTypeImage(card.GetCardTypeImage());

        button = cardObject.GetComponent<UI_OnClickButton>();
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

    public int GetAmount()
    {
        int amount = 0;
        bool isValid = int.TryParse(button.GetText(), out amount);

        if (isValid)
            return amount;
        else
            return 0;
    }
}