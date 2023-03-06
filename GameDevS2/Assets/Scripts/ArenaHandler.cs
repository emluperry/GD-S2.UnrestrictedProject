using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaHandler : MonoBehaviour
{
    private Collider m_Trigger;

    private void Awake()
    {
        m_Trigger = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //start battle
        //unparent from enemy
        gameObject.transform.SetParent(transform, true);

        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }

        m_Trigger.enabled = false;
    }
}
