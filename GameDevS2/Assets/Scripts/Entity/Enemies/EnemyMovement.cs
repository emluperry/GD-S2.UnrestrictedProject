using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : EntityMovement
{
    private Transform _playerTransform;

    public void SetPlayerTransform(Transform playerTransform) //set when spawned
    {
        _playerTransform = playerTransform;
    }

    public void StartBattle()
    {
        //determine current behaviour?
    }
}
