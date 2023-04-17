using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_DeckEditor : MonoBehaviour
{
    [SerializeField] private UI_Inventory_Pair[] _deckButtons;
    [SerializeField] private UI_Inventory_Pair[] _inventoryButtons;

    private void Awake()
    {
        foreach(UI_Inventory_Pair button in _deckButtons)
        {
            button.onButtonClicked += UpdateValue;
        }

        foreach (UI_Inventory_Pair button in _inventoryButtons)
        {
            button.onButtonClicked += UpdateValue;
        }
    }
}
