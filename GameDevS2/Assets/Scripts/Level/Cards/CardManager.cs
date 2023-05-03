using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [SerializeField] private Inventory_Card_Value_Pair[] _cardInventory;
    //key: card name
    private Dictionary<string, Inventory_Card_Value_Pair> _cardDictionary; //convert unity values into this during runtime - cannot set dict. values in inspector

    [SerializeField] private int _maxDeckSize = 20;
    [SerializeField] private Inventory_Card_Value_Pair[] _currentDeck; //use default values as the default deck
    private Dictionary<string, Inventory_Card_Value_Pair> _deckDictionary;

    private void Awake()
    {
        _cardDictionary = new Dictionary<string, Inventory_Card_Value_Pair>();

        foreach(Inventory_Card_Value_Pair pair in _cardInventory)
        {
            _cardDictionary.Add(pair.card.GetName(), pair);
        }

        _deckDictionary = new Dictionary<string, Inventory_Card_Value_Pair>();

        foreach (Inventory_Card_Value_Pair pair in _currentDeck)
        {
            _deckDictionary.Add(pair.card.GetName(), pair);
        }
    }

    public void SetupPauseCards(UI_Screen screen, bool shouldSetup)
    {
        UI_DeckEditor deckEditor = screen.gameObject.GetComponentInChildren<UI_DeckEditor>(true);
        if (!deckEditor)
            return;

        if (shouldSetup)
        {
            deckEditor.Initialise(_maxDeckSize);
            deckEditor.SetupButtons(_cardInventory, _currentDeck);
            deckEditor.onUpdateDeck += UpdateDeck;
        }
        else
            deckEditor.onUpdateDeck -= UpdateDeck;
    }

    private void UpdateDeck(string cardName, bool addToDeck) //true = add to deck, false = remove from deck
    {
        if (addToDeck)
            AddToDeck(cardName);
        else
            RemoveFromDeck(cardName);
    }

    public void AddCollectable(string cardName, int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            AddToInventory(cardName);
        }
    }

    private void AddToInventory(string cardName)
    {
        if (_cardDictionary.ContainsKey(cardName))
        {
            Inventory_Card_Value_Pair current = _cardDictionary[cardName];
            current.amount += 1;
            _cardDictionary[cardName] = current;
        }
    }

    private void AddToDeck(string cardName)
    {
        if(_cardDictionary.ContainsKey(cardName))
        {
            Inventory_Card_Value_Pair current = _cardDictionary[cardName];
            current.amount -= 1;
            _cardDictionary[cardName] = current;
        }

        if (_deckDictionary.ContainsKey(cardName))
        {
            Inventory_Card_Value_Pair current = _deckDictionary[cardName];
            current.amount += 1;
            _deckDictionary[cardName] = current;
        }
    }

    private void RemoveFromDeck(string cardName)
    {
        AddToInventory(cardName);

        if (_deckDictionary.ContainsKey(cardName))
        {
            Inventory_Card_Value_Pair current = _deckDictionary[cardName];
            current.amount -= 1;
            _deckDictionary[cardName] = current;
        }
    }

    public void SetDecklist(PlayerCards cardsComponent)
    {
        List<Inventory_Card_Value_Pair> currentDeck = new List<Inventory_Card_Value_Pair>();

        foreach (KeyValuePair<string, Inventory_Card_Value_Pair> pair in _deckDictionary)
        {
            if(pair.Value.amount > 0)
                currentDeck.Add(pair.Value);
        }

        if(currentDeck.Count <= 0) //if nothing has been set in the deck, failsafe:
        {
            Inventory_Card_Value_Pair failsafe = new Inventory_Card_Value_Pair();
            failsafe.card = _cardDictionary["Claws"].card;
            failsafe.amount = 1;
            currentDeck.Add(failsafe);
        }

        cardsComponent.SetDeckList(currentDeck.ToArray());
    }
}
