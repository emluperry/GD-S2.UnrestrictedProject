using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Collectable : MonoBehaviour
{
    [Header("Collectable Details")]
    //collectable type: ie coins, card - just card for now
    [SerializeField] public string collectableName = "None"; //name of the card to be collected!
    //amount: ie 10 coins, 2 copies of one card
    [SerializeField] public int amount = 1;


    public Action<Collectable> onCollected;

    private void OnTriggerEnter(Collider other)
    {
        if(TryGetComponent(out Collider collectableCollider))
        {
            collectableCollider.enabled = false;
        }

        onCollected?.Invoke(this);

        Destroy(gameObject);
    }
}
