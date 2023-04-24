using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_DeckEditor : MonoBehaviour
{
    [SerializeField] private GameObject _UICardPrefab;

    [SerializeField] private Transform _DeckList;
    [SerializeField] private Transform _InventoryList;

    [SerializeField] private UI_Inventory_Pair[] _deckButtons;
    [SerializeField] private UI_Inventory_Pair[] _inventoryButtons;

    [SerializeField] private int _maxPerRow;

    public Action onRequestInventory;
    public Action<string, bool> onUpdateDeck; //true = add to deck, false = remove from deck

    private void Awake()
    {
        onRequestInventory?.Invoke();

        SetupButtonEvents();
    }

    private void OnDestroy()
    {
        DisableButtonEvents();
    }

    public void SetupButtons(Inventory_Card_Value_Pair[] _currentInventory, Inventory_Card_Value_Pair[] _currentDeck)
    {
        int currentRow = 0;
        int currentInRow = 0;
        foreach (Inventory_Card_Value_Pair pair in _currentInventory)
        {
            //create ui card
            GameObject newCard = Instantiate(_UICardPrefab, _InventoryList);
            //setup values
            UI_Inventory_Pair uiPair = new UI_Inventory_Pair();
            uiPair.card = pair.card;
            uiPair.cardObject = newCard;
            uiPair.SetupCard();

            currentInRow++;
            if (currentInRow >= _maxPerRow)
                currentRow++;
        }

        foreach (UI_Inventory_Pair button in _deckButtons)
        {
            button.SetupCard();
        }

        foreach (UI_Inventory_Pair button in _inventoryButtons)
        {
            button.SetupCard();
        }
    }

    private void SetupButtonEvents()
    {
        foreach (UI_Inventory_Pair button in _deckButtons)
        {
            button.StartListeningForEvents();
            button.onButtonClicked += OnDeckButtonClicked;
        }

        foreach (UI_Inventory_Pair button in _inventoryButtons)
        {
            button.StartListeningForEvents();
            button.onButtonClicked += OnInventoryButtonClicked;
        }
    }

    private void DisableButtonEvents()
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
