using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_WorldCanvasRotation : MonoBehaviour
{
    private Transform _targetCamera;

    public void SetupCameraReference(Transform camera)
    {
        _targetCamera = camera;
    }

    public void UpdateCanvasRotation()
    {
        transform.LookAt(_targetCamera);
    }
}
