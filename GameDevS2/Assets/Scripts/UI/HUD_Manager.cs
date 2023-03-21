using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Manager : MonoBehaviour
{
    private enum HUD_STATE
    {
        NONE, //standard state - no ui other than button prompts?
        IDLE, //player stands still for too long, ui such as health(?) and current amount of money shows up
        BATTLE //full ui, health, defense, cards
    }

    private HUD_STATE _state = HUD_STATE.NONE;

    [Header("External References")]
    [SerializeField] private PlayerCards _playerCardsComponent;

    [Header("Battle UI")]
    [SerializeField] private Image _health;
    [SerializeField] private Image _defense;

    [SerializeField] private GameObject _cardUIPrefab;
    [SerializeField] private Image _deckObject;
    [SerializeField] private TextMeshProUGUI _deckText;
    [SerializeField] private HorizontalLayoutGroup _handLayoutGroup;

    private Inventory_Card_Value_Pair[] _deckList;
    private int _currentCard = 0;


    //BATTLE FUNCTIONS
    public void StartBattle(Inventory_Card_Value_Pair[] deckList)
    {
        Debug.Log("Setup hud manager for battle.");
        _state = HUD_STATE.BATTLE;

        //unhide battle elements - could group these in future revision for cleaner code?
        _health.gameObject.SetActive(true);
        _defense.gameObject.SetActive(true);
        _deckObject.gameObject.SetActive(true);
        _deckText = _deckObject.GetComponentInChildren<TextMeshProUGUI>();
        _handLayoutGroup.gameObject.SetActive(true);

        _deckList = deckList;
        //get the size of the deck
        int currentMaxCards = 0;
        foreach (Inventory_Card_Value_Pair pair in _deckList)
        {
            currentMaxCards += pair.amount;
        }
        _deckText.text = currentMaxCards.ToString();

        //listen for events in player cards
        if (_playerCardsComponent != null)
        {
            _playerCardsComponent.onCardUsed += OnCardUsed;
            _playerCardsComponent.onHandDraw += OnHandDraw;
            _playerCardsComponent.onDiscardHand += OnDiscardHand;
            _playerCardsComponent.onSelectedChanged += OnChangeSelection;
        }
    }

    public void EndBattle()
    {
        _state = HUD_STATE.NONE;

        _health.gameObject.SetActive(false);
        _defense.gameObject.SetActive(false);
        _deckObject.gameObject.SetActive(false);
        _handLayoutGroup.gameObject.SetActive(false);

        _deckList = null;

        //unlisten to events in player cards
        if (_playerCardsComponent != null)
        {
            _playerCardsComponent.onCardUsed -= OnCardUsed;
            _playerCardsComponent.onHandDraw -= OnHandDraw;
            _playerCardsComponent.onDiscardHand -= OnDiscardHand;
            _playerCardsComponent.onSelectedChanged -= OnChangeSelection;
        }
    }

    private void OnCardUsed(int cardUsedIndex)
    {
        //play any animation

        //destroy card
        Destroy(_handLayoutGroup.transform.GetChild(cardUsedIndex).gameObject);
    }

    private void OnChangeSelection(int newIndex)
    {
        if(_handLayoutGroup.transform.childCount > 0)
        {
            _handLayoutGroup.transform.GetChild(_currentCard).GetComponent<UI_Card>().Deselect();

            _handLayoutGroup.transform.GetChild(newIndex).GetComponent<UI_Card>().SetSelected();
            _currentCard = newIndex;
        }
    }

    private void OnHandDraw(List<int> newHand)
    {
        //update deck value
        int.TryParse(_deckText.text, out int oldDeckValue);
        int newDeckValue = Mathf.Max(oldDeckValue - newHand.Count, 0);
        _deckText.text = newDeckValue.ToString();

        //update hand
        foreach(int cardType in newHand)
        {
            GameObject card = Instantiate(_cardUIPrefab, new Vector3(0,0,0), Quaternion.identity ,_handLayoutGroup.transform);
            UI_Card uiComponent = card.GetComponent<UI_Card>();
            uiComponent.SetCardValue(_deckList[cardType].card.GetCardPower());
            //uiComponent.SetImage(_deckList[cardType].card.GetCardImage()); -- uncomment when this actually exists!!!

            uiComponent.Deselect();
        }

        OnChangeSelection(0);
    }

    private void OnDiscardHand()
    {
        //discard previous hand
        foreach (Transform card in _handLayoutGroup.transform)
        {
            Destroy(card.gameObject);
        }
    }
}
