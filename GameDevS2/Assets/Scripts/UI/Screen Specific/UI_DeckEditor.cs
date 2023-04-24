using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_DeckEditor : MonoBehaviour
{
    [SerializeField] private UI_Inventory_Pair[] _deckButtons;
    [SerializeField] private UI_Inventory_Pair[] _inventoryButtons;

    private void Awake()
    {
        SetupButtons();
    }

    private void OnDestroy()
    {
        SetdownButtons();
    }

    private void SetupButtons()
    {
        foreach (UI_Inventory_Pair button in _deckButtons)
        {
            button.SetupCard();

            button.StartListeningForEvents();
            button.onButtonClicked += OnDeckButtonClicked;
        }

        foreach (UI_Inventory_Pair button in _inventoryButtons)
        {
            button.SetupCard();

            button.StartListeningForEvents();
            button.onButtonClicked += OnInventoryButtonClicked;
        }
    }

    private void SetdownButtons()
    {
        foreach (UI_Inventory_Pair button in _deckButtons)
        {
            button.StopListeningForEvents();
            button.onButtonClicked -= OnDeckButtonClicked;
        }

        foreach (UI_Inventory_Pair button in _inventoryButtons)
        {
            button.StopListeningForEvents();
            button.onButtonClicked -= OnInventoryButtonClicked;
        }
    }

    private void OnDeckButtonClicked(string cardName)
    {
        //decrease deck val, increase inv val
        foreach(UI_Inventory_Pair pair in _deckButtons)
        {
            if(pair.card.GetName() == cardName)
            {
                pair.UpdateAmount(pair.GetAmount() - 1);
                break;
            }
        }

        foreach (UI_Inventory_Pair pair in _inventoryButtons)
        {
            if (pair.card.GetName() == cardName)
            {
                pair.UpdateAmount(pair.GetAmount() + 1);
                break;
            }
        }
    }

    private void OnInventoryButtonClicked(string cardName)
    {
        //decrease inv val, increase deck val
        foreach (UI_Inventory_Pair pair in _deckButtons)
        {
            if (pair.card.GetName() == cardName)
            {
                pair.UpdateAmount(pair.GetAmount() + 1);
                break;
            }
        }

        foreach (UI_Inventory_Pair pair in _inventoryButtons)
        {
            if (pair.card.GetName() == cardName)
            {
                pair.UpdateAmount(pair.GetAmount() - 1);
                break;
            }
        }
    }
}
