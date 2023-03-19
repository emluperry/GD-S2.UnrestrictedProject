using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCards : MonoBehaviour
{
    [SerializeField] private Inventory_Card_Value_Pair[] _cards;
    private int[] _deckArray;
    private List<int> _currentHand;

    [Header("Deck Limits")]
    //used to limit deck size to 20 (currently not used).
    [SerializeField] private int _totalMaxCards = 20;
    //actual total deck size
    private int _currentMaxCards = 0;
    private int _currentDeckIndex = 0;
    [SerializeField] private int _maxHandDraw = 5;
    private int _currentHandSize = 0;
    private int _currentHandIndex = 0;

    //input
    private PlayerInput _input;
    
    //swapping
    private InputAction _swapInputAction;
    private int _swapInput;
    private Coroutine _swapCoroutine;

    [Header("Cooldown Values")]
    [SerializeField] private float _swapDelay = 0.5f;
    [SerializeField] private float _maxDrawDelay = 5f;

    private void Awake()
    {
        _input = GetComponent<PlayerInput>();

        _swapInputAction = _input.currentActionMap.FindAction("Swap");
    }

    #region INPUT

    private void Input_SwapPerformed(InputAction.CallbackContext ctx)
    {
        float input = ctx.ReadValue<float>();
        _swapInput = Mathf.RoundToInt(input);

        if (_swapInput != 0 && _swapCoroutine == null)
        {
            _swapCoroutine = StartCoroutine(c_OnCardSwapped());
        }
    }

    private void Input_SwapCancelled(InputAction.CallbackContext ctx)
    {
        _swapInput = 0;
    }

    #endregion

    public void StartBattle()
    {
        //input to swap active card
        _swapInputAction.performed += Input_SwapPerformed;
        _swapInputAction.canceled += Input_SwapCancelled;

        _currentHand = new List<int>();

        InitialiseDeck();

        Shuffle(_deckArray);

        DrawHand();
    }

    public void EndBattle()
    {
        _swapInputAction.performed -= Input_SwapPerformed;
        _swapInputAction.canceled -= Input_SwapCancelled;

        _currentHand.Clear();
    }

    private IEnumerator c_OnCardSwapped()
    {
        _currentHandIndex += _swapInput;

        if (_currentHandIndex >= _currentHandSize)
            _currentHandIndex = 0;
        else if (_currentHandIndex < 0)
            _currentHandIndex = _currentHandSize - 1;

        int currentCardType = _currentHand[_currentHandIndex];
        Scriptable_Card currentCard = _cards[currentCardType].card;
        Debug.Log("Current card: " + _currentHandIndex + " - " + currentCard.GetName());

        float currentTime = 0;
        while (currentTime < _swapDelay)
        {
            yield return new WaitForFixedUpdate();

            currentTime += Time.fixedDeltaTime;
        }
    }

    private void InitialiseDeck()
    {
        _currentDeckIndex = 0;
        //get the size of the deck
        _currentMaxCards = 0;
        foreach (Inventory_Card_Value_Pair pair in _cards)
        {
            _currentMaxCards += pair.amount;
        }

        //set the size of the array
        _deckArray = new int[_currentMaxCards];

        //add cards in order
        int currentIndex = 0;
        for (int i = 0; i < _cards.Length; i++)
        {
            for (int j = 0; j < _cards[i].amount; j++)
            {
                int current = currentIndex + j;
                _deckArray[current] = i;
            }
            currentIndex += _cards[i].amount;
        }
    }

    //fisher yates shuffle
    private void Shuffle(int[] cardArray)
    {
        int currentIndex = cardArray.Length;
        while(currentIndex > 1)
        {
            currentIndex--;
            int randomIndex = UnityEngine.Random.Range(0, currentIndex);

            (cardArray[randomIndex], cardArray[currentIndex]) = (cardArray[currentIndex], cardArray[randomIndex]);
        }
    }

    private void DrawHand()
    {
        if(_currentDeckIndex >= _currentMaxCards) { return; }

        //reset hand
        _currentHand.Clear();
        _currentHandSize = 0;

        //for a maximum of draw size times
        for(int i = 0; i < _maxHandDraw; i++)
        {
            _currentHand.Add(_deckArray[_currentDeckIndex]);

            _currentHandSize++;
            _currentDeckIndex++;

            //if there are no cards left in deck, break; the loop
            if (_currentDeckIndex >= _currentMaxCards) { break; }
        }
    }
}
