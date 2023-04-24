using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GDS2_Cards;

[CreateAssetMenu(fileName = "NewCard", menuName = "ScriptableObjects/Create new card", order = 1)]
public class Scriptable_Card : ScriptableObject
{
    //visual
    [SerializeField] private string _cardName = "NewCard";
    [SerializeField] private Sprite _cardImage;

    //game impact
    [SerializeField] private int _cardPower = 1;
    [SerializeField] private CARD_TYPE _cardType = CARD_TYPE.ATTACK;
    [SerializeField] private Sprite _cardTypeImage;
    //[SerializeField] private CARD_AFFINITY _cardAffinity = CARD_AFFINITY.BASIC;
    //any extra effects? array of 'Effect' derived classes? could be something like.. inflicts a burn, stuns the enemy for a moment, etc

    public string GetName()
    {
        return _cardName;
    }

    public int GetCardPower()
    {
        return _cardPower;
    }

    public CARD_TYPE GetCardType()
    {
        return _cardType;
    }

    public Sprite GetCardTypeImage()
    {
        return _cardTypeImage;
    }

    public Sprite GetSprite()
    {
        return _cardImage;
    }
}
