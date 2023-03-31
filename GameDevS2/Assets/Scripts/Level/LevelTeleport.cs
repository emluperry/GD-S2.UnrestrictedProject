using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTeleport : MonoBehaviour
{
    private Collider _collider;

    public Transform playerStartTransform;

    public Action onExitTouched;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(_collider)
            _collider.enabled = false;

        //play animation?

        //load player into connected teleport location
        onExitTouched?.Invoke();
    }
}
