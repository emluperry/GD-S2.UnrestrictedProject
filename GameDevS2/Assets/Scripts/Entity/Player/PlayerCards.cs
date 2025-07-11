using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCards : MonoBehaviour, IInput, IPausable
{
    public bool isInCombat { get; private set; } = false;

    [SerializeField] private Inventory_Card_Value_Pair[] _cards;
    private int[] _deckArray;
    private List<int> _currentHand;

    [Header("Gameplay Limits")]
    //total deck size
    private int _currentMaxCards = 0;
    private int _currentDeckIndex = 0;
    //hand draw size
    [SerializeField] private int _maxHandDraw = 5;
    private int _currentHandSize = 0;
    private int _currentHandIndex = 0;

    //input: swapping
    private InputAction _swapInputAction;
    private int _swapInput;
    private Coroutine _swapCoroutine;

    //input: discard & draw
    private InputAction _drawInputAction;

    [Header("Cooldown Values")]
    //delay between swap inputs
    [SerializeField] private float _swapDelay = 0.5f;
    //delay between drawing a new hand - due to input or automatic
    [SerializeField] private float _maxDrawDelay = 2f;
    //delay between refreshing the empty deck to refill values
    [SerializeField] private float _maxDeckRefreshDelay = 5f;

    private Coroutine _handDrawCoroutine;

    //events for UI updates
    public Action<List<int>> onHandDraw;
    public Action onDiscardHand;
    //takes in index of used card
    public Action<int> onCardUsed;
    //takes in index of new card to select
    public Action<int> onSelectedChanged;

    private bool _isPaused = false;

    #region INPUT SETUP

    public void SetupInput(Dictionary<string, InputAction> inputs)
    {
        _swapInputAction = inputs["Swap"];
        _drawInputAction = inputs["Draw"];
    }

    public void EnableInput()
    {
        if (_swapInputAction != null)
        {
            _swapInputAction.performed += Input_SwapPerformed;
            _swapInputAction.canceled += Input_SwapCancelled;
        }

        if (_drawInputAction != null)
        {
            _drawInputAction.performed += Input_DrawPerformed;
        }
    }

    public void DisableInput()
    {
        if (_swapInputAction != null)
        {
            _swapInputAction.performed -= Input_SwapPerformed;
            _swapInputAction.canceled -= Input_SwapCancelled;
        }

        if (_drawInputAction != null)
        {
            _drawInputAction.performed -= Input_DrawPerformed;
        }
    }

    #endregion

    private void OnDestroy()
    {
        if(isInCombat)
            EndBattle();
    }

    #region INPUT

    private void Input_SwapPerformed(InputAction.CallbackContext ctx)
    {
        if (_isPaused)
            return;

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

    private void Input_DrawPerformed(InputAction.CallbackContext ctx)
    {
        if (_isPaused)
            return;

        bool input = ctx.ReadValueAsButton();

        if(input && _handDrawCoroutine == null)
        {
            DiscardHand();
            _handDrawCoroutine = StartCoroutine(c_DelayHandDraw());
        }
    }

    #endregion

    public void InitialiseBattle()
    {
        isInCombat = true;

        //enable all inputs
        EnableInput();

        _currentHand = new List<int>();

        InitialiseDeck();

        Shuffle(_deckArray);
    }

    public void StartBattle()
    {
        DrawHand();
    }

    public int GetDeckSize()
    {
        return _currentMaxCards;
    }

    public Inventory_Card_Value_Pair[] GetDeckList()
    {
        return _cards;
    }

    public void EndBattle()
    {
        isInCombat = false;

        DisableInput();

        _currentHand.Clear();

        if(_handDrawCoroutine != null)
        {
            StopCoroutine(_handDrawCoroutine);
            _handDrawCoroutine = null;
        }

        if(_swapCoroutine != null)
        {
            StopCoroutine(_swapCoroutine);
            _swapCoroutine = null;
        }
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

        onSelectedChanged?.Invoke(_currentHandIndex);

        float currentTime = 0;
        while (currentTime < _swapDelay)
        {
            yield return new WaitUntil(() => !_isPaused);
            yield return new WaitForFixedUpdate();

            currentTime += Time.fixedDeltaTime;
        }

        _swapCoroutine = null;
    }

    private IEnumerator c_DelayHandDraw()
    {
        float maxDelay = 0;
        bool shouldReshuffleDeck = _currentDeckIndex >= _deckArray.Length;
        if (shouldReshuffleDeck) //if deck is empty
            maxDelay = _maxDeckRefreshDelay;
        else
            maxDelay = _maxDrawDelay;

        float currentDrawDelay = 0;
        while (currentDrawDelay < maxDelay)
        {
            yield return new WaitUntil(() => !_isPaused);
            yield return new WaitForFixedUpdate();

            currentDrawDelay += Time.fixedDeltaTime;
        }

        //once delay is up, refresh deck values (if necessary)
        if (shouldReshuffleDeck)
        {
            Debug.Log("Refreshing deck");
            Shuffle(_deckArray);
            _currentDeckIndex = 0;
        }

        //draw hand
        DrawHand();

        _handDrawCoroutine = null;
    }

    public Scriptable_Card GetSelectedCard()
    {
        if (_currentHandSize <= 0 || _handDrawCoroutine != null) //defensive, may occur during hand refreshes
            return null;

        int cardType = _currentHand[_currentHandIndex];

        return _cards[cardType].card;
    }

    public Scriptable_Card UseSelectedCard()
    {
        if (_currentHandSize <= 0 || _handDrawCoroutine != null) //defensive, may occur during hand refreshes
            return null;

        //remove from hand list
        int cardType = _currentHand[_currentHandIndex];
        _currentHand.RemoveAt(_currentHandIndex);

        Scriptable_Card card = _cards[cardType].card;

        onCardUsed?.Invoke(_currentHandIndex);

        //decrease size of hand
        _currentHandSize--;

        //update current hand index to keep within boundaries
        if (_currentHandIndex >= _currentHandSize)
            _currentHandIndex = 0;
        else if (_currentHandIndex < 0)
            _currentHandIndex = _currentHandSize - 1;

        onSelectedChanged?.Invoke(_currentHandIndex);

        //draw new hand if hand is now empty - will refresh deck if deck is now empty
        if (_currentHandSize <= 0 && _handDrawCoroutine == null)
            _handDrawCoroutine = StartCoroutine(c_DelayHandDraw());

        //return the removed card
        return card;
    }

    public void SetDeckList(Inventory_Card_Value_Pair[] newDeckList)
    {
        if(newDeckList != null)
        {
            _cards = newDeckList;
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

        onHandDraw?.Invoke(_currentHand);
        Debug.Log("New hand size: " + _currentHandSize);
    }

    private void DiscardHand()
    {
        _currentHandSize = 0;
        _currentHandIndex = 0;
        _currentHand.Clear();

        onDiscardHand?.Invoke();
    }

    public void PauseGame(bool isPaused)
    {
        _isPaused = isPaused;
    }
}
