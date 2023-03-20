using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDS2_Cards
{
    public enum CARD_TYPE
    {
        ATTACK,
        DEFENSE,
        HEALTH
    }

    public enum CARD_AFFINITY
    {
        BASIC, //standard attack
        FIRE, //burn
        ICE, //freeze?
        LIGHTNING //stun
                  //etc - mostly examples!! may not be implemented due to scope
    }
}