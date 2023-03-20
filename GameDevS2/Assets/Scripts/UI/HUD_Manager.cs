using System.Collections;
using System.Collections.Generic;
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

    //external references
    [SerializeField] private PlayerCards _playerCardsComponent;

    //battle UI
    [SerializeField] private Image _health;
    [SerializeField] private Image _defense;

    [SerializeField] private GameObject _cardUIPrefab;
    [SerializeField] private Image _deckObject;
    [SerializeField] private HorizontalLayoutGroup _handLayoutGroup;


    //BATTLE FUNCTIONS
    public void StartBattle()
    {
        _state = HUD_STATE.BATTLE;

        //unhide battle elements - could group these in future revision for cleaner code?
        _health.gameObject.SetActive(true);
        _defense.gameObject.SetActive(true);
        _deckObject.gameObject.SetActive(true);
        _handLayoutGroup.gameObject.SetActive(true);

        //listen for events in player cards
        if(_playerCardsComponent != null)
        {
            _playerCardsComponent.onCardUsed += OnCardUsed;
            _playerCardsComponent.onHandDraw += OnHandDraw;
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

        //unlisten to events in player cards
        if (_playerCardsComponent != null)
        {
            _playerCardsComponent.onCardUsed -= OnCardUsed;
            _playerCardsComponent.onHandDraw -= OnHandDraw;
            _playerCardsComponent.onSelectedChanged -= OnChangeSelection;
        }
    }

    private void OnCardUsed(int cardUsedIndex)
    {

    }

    private void OnChangeSelection(int newIndex)
    {

    }

    private void OnHandDraw()
    {

    }
}
