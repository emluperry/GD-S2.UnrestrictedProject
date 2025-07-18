using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_DeckEditor : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _UICardPrefab;
    [SerializeField] private GameObject _RowPrefab;

    [Header("UI References")]
    [SerializeField] private Transform _DeckList;
    [SerializeField] private Transform _InventoryList;
    [SerializeField] private TextMeshProUGUI _deckLimitsText;

    private List<UI_Inventory_Pair> _deckButtons;
    private List<UI_Inventory_Pair> _inventoryButtons;

    [Header("UI Limits")]
    [SerializeField] private int _maxPerRow = 3;

    //deck limits
    private int _maxDeckSize = 20;
    private int _currentDeckSize = 0;

    public Action<string, bool> onUpdateDeck; //true = add to deck, false = remove from deck

    private void OnDestroy()
    {
        DisableButtonEvents();
    }

    public void Initialise(int maxDeckSize)
    {
        _maxDeckSize = maxDeckSize;
    }

    public void SetupButtons(Inventory_Card_Value_Pair[] _currentInventory, Inventory_Card_Value_Pair[] _currentDeck)
    {
        _inventoryButtons = new List<UI_Inventory_Pair>();

        int currentInRow = 0;
        GameObject currentLayoutGroup = null;
        foreach (Inventory_Card_Value_Pair pair in _currentInventory)
        {
            if(currentInRow == 0)
            {
                //setup hlayout group
                currentLayoutGroup = Instantiate(_RowPrefab, _InventoryList);
            }
            //create ui card
            CreateNewCard(_inventoryButtons, currentLayoutGroup.transform, pair.card, pair.amount);

            currentInRow++;
            if (currentInRow >= _maxPerRow)
            {
                currentInRow = 0;
            }
        }

        _deckButtons = new List<UI_Inventory_Pair>();
        currentInRow = 0;
        currentLayoutGroup = null;
        foreach (Inventory_Card_Value_Pair pair in _currentDeck)
        {
            if (pair.amount <= 0)
            {
                continue;
            }

            if (currentInRow == 0)
            {
                //setup hlayout group
                currentLayoutGroup = Instantiate(_RowPrefab, _DeckList);
            }
            //create ui card
            CreateNewCard(_deckButtons, currentLayoutGroup.transform, pair.card, pair.amount);
            _currentDeckSize += pair.amount;

            currentInRow++;
            if (currentInRow >= _maxPerRow)
            {
                currentInRow = 0;
            }
        }

        UpdateDeckText();

        SetupButtonEvents();
    }

    private void CreateNewCard(List<UI_Inventory_Pair> list, Transform newParent, Scriptable_Card cardType, int amount)
    {
        GameObject newCard = Instantiate(_UICardPrefab, newParent);
        //setup values
        UI_Inventory_Pair uiPair = new UI_Inventory_Pair();
        uiPair.card = cardType;
        uiPair.cardObject = newCard;
        uiPair.SetupCard();
        uiPair.UpdateAmount(amount);

        list.Add(uiPair);
    }

    private void DestroyCard(UI_Inventory_Pair pair, List<UI_Inventory_Pair> parentList)
    {
        pair.StopListeningForEvents();
        pair.onButtonClicked -= OnInventoryButtonClicked;

        Destroy(pair.cardObject);
        parentList.Remove(pair);
    }

    private void RearrangeCards(Transform objectList)
    {
        if(objectList.childCount <= 1)
        {
            return;
        }

        //search for empty space
        for(int i = 0; i < objectList.childCount - 1; i++)
        {
            Transform currentRow = objectList.transform.GetChild(i);
            while(currentRow.childCount <= _maxPerRow)
            {
                Transform nextRow = objectList.transform.GetChild(i + 1);
                nextRow.GetChild(0).SetParent(currentRow, false);

                if(nextRow.childCount <= 0)
                {
                    Destroy(nextRow.gameObject);
                }
            }
        }
    }

    private void UpdateDeckText()
    {
        _deckLimitsText.text = _currentDeckSize.ToString() + " / " + _maxDeckSize.ToString();
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

    private bool CurrentSelectionIsValid(UI_Inventory_Pair deckPair)
    {
        int amount = deckPair.GetAmount();

        if (amount <= 0) //don't continue if there are no more cards of this type in the deck - failsafe
            return false;

        //check type as the following only applies to attacking cards
        if(deckPair.card.GetCardType() == GDS2_Cards.CARD_TYPE.ATTACK)
        {
            if (amount - 1 >= 1)
                return true;

            foreach (UI_Inventory_Pair pair in _deckButtons)
            {
                if (pair == deckPair)
                    continue;

                if (pair.card.GetCardType() != GDS2_Cards.CARD_TYPE.ATTACK)
                    continue;
                else
                {
                    return true;
                }
            }

            return false;
        }

        return true;
    }

    private void OnDeckButtonClicked(string cardName)
    {
        //decrease deck val, increase inv val
        foreach(UI_Inventory_Pair pair in _deckButtons)
        {
            if(pair.card.GetName() == cardName)
            {
                //VALIDATE SELECTION
                if (!CurrentSelectionIsValid(pair))
                    return;

                int amount = pair.GetAmount() -1;

                pair.UpdateAmount(amount);

                //if new amount is 0 or less, destroy from deck button list
                if(amount <= 0)
                {
                    DestroyCard(pair, _deckButtons);
                    RearrangeCards(_DeckList);
                }
                break;
            }
        }
        _currentDeckSize--;
        UpdateDeckText();

        foreach (UI_Inventory_Pair pair in _inventoryButtons)
        {
            if (pair.card.GetName() == cardName)
            {
                pair.UpdateAmount(pair.GetAmount() + 1);
                break;
            }
        }

        onUpdateDeck?.Invoke(cardName, false);
    }

    private void OnInventoryButtonClicked(string cardName)
    {
        if(_currentDeckSize >= _maxDeckSize)
        {
            return;
        }

        //decrease inv val, increase deck val
        Scriptable_Card card = null;
        foreach (UI_Inventory_Pair pair in _inventoryButtons)
        {
            if (pair.card.GetName() == cardName)
            {
                int amount = pair.GetAmount();

                if(amount <= 0) //don't continue if there are no more cards of this type in the inventory
                {
                    return;
                }

                pair.UpdateAmount(amount - 1);
                card = pair.card;
                break;
            }
        }

        bool deckCardFound = false;
        foreach (UI_Inventory_Pair pair in _deckButtons)
        {
            if (pair.card.GetName() == cardName)
            {
                pair.UpdateAmount(pair.GetAmount() + 1);
                deckCardFound = true;
                break;
            }
        }

        if(!deckCardFound)
        {
            bool emptySpaceFound = false;
            //create new card in empty space
            foreach(Transform entry in _DeckList)
            {
                if(entry.childCount < _maxPerRow)
                {
                    //add here and break
                    CreateNewCard(_deckButtons, entry, card, 1);
                    emptySpaceFound = true;
                    break;
                }
            }

            //else create new row for card
            if(!emptySpaceFound)
            {
                GameObject currentLayoutGroup = Instantiate(_RowPrefab, _DeckList);
                CreateNewCard(_deckButtons, currentLayoutGroup.transform, card, 1);
            }
        }
        _currentDeckSize++;
        UpdateDeckText();

        onUpdateDeck?.Invoke(cardName, true);
    }
}
