using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Manager : MonoBehaviour
{
    [Header("External References")]
    [SerializeField] private PlayerCards _playerCardsComponent;

    [Header("Battle UI")]
    [SerializeField] private HealthBar _health;

    [SerializeField] private GameObject _cardUIPrefab;
    [SerializeField] private Image _deckObject;
    [SerializeField] private TextMeshProUGUI _deckText;
    [SerializeField] private HorizontalLayoutGroup _handLayoutGroup;

    private Inventory_Card_Value_Pair[] _deckList;
    private int _currentCard = 0;
    private int _deckMaxSize = 0;

    private void Awake()
    {
        _deckText = _deckObject.GetComponentInChildren<TextMeshProUGUI>(true);
    }

    //BATTLE FUNCTIONS
    public void StartBattle(Inventory_Card_Value_Pair[] deckList, int deckSize, EntityHealth playerHealth)
    {
        //setup healthbar
        _health.SetupBar(playerHealth.GetMaxHealth());
        _health.transform.parent.gameObject.SetActive(true);

        //setup deck
        _deckList = deckList;
        _deckMaxSize = deckSize;
        _deckText.text = _deckMaxSize.ToString();

        //empty hand
        foreach (Transform card in _handLayoutGroup.transform)
        {
            Destroy(card.gameObject);
        }

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
        _health.transform.parent.gameObject.SetActive(false);

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
        UpdateDeckValue(newHand.Count);

        //update hand
        foreach(int cardType in newHand)
        {
            GameObject card = Instantiate(_cardUIPrefab, new Vector3(0,0,0), Quaternion.identity ,_handLayoutGroup.transform);
            UI_Card uiComponent = card.GetComponent<UI_Card>();
            uiComponent.SetCardValue(_deckList[cardType].card.GetCardPower());
            uiComponent.SetImage(_deckList[cardType].card.GetSprite());

            uiComponent.Deselect();
        }

        OnChangeSelection(0);
    }

    private void UpdateDeckValue(int newHand)
    {
        int.TryParse(_deckText.text, out int oldDeckValue);
        int newDeckValue;

        if (oldDeckValue <= 0)
            newDeckValue = _deckMaxSize - newHand;
        else
            newDeckValue = Mathf.Max(oldDeckValue - newHand, 0);
        _deckText.text = newDeckValue.ToString();
    }

    private void OnDiscardHand()
    {
        //discard previous hand
        foreach (Transform card in _handLayoutGroup.transform)
        {
            Destroy(card.gameObject);
        }

        UpdateDeckValue(0);
    }

    public void OnPlayerDamaged(int damage)
    {
        _health.TakeDamage(damage);
    }

    public void UpdateHUDValue(bool whichValue, int amount)
    {
        if (whichValue)
            _health.HealHealth(amount);
        else
            _health.IncreaseShield(amount);
    }
}
