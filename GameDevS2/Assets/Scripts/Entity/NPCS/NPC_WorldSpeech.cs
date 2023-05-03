using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_WorldSpeech : MonoBehaviour
{
    [SerializeField] private UI_WorldCanvasRotation _canvasRotator;

    private PlayerMovement _playerMovementComp;

    private void OnTriggerEnter(Collider other)
    {
        if(_canvasRotator)
        {
            if(other.TryGetComponent(out _playerMovementComp))
            {
                _canvasRotator.SetupCameraReference(_playerMovementComp.GetCameraTransform());
                _playerMovementComp.onUpdateRotation += _canvasRotator.UpdateCanvasRotation;

                _canvasRotator.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_canvasRotator)
        {
            _playerMovementComp.onUpdateRotation -= _canvasRotator.UpdateCanvasRotation;
            _playerMovementComp = null;

            _canvasRotator.gameObject.SetActive(false);
        }
    }
}
