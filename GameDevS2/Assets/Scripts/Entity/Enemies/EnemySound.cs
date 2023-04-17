using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySound : EntitySound
{
    private enum R_SOUNDS
    {
        NONE,
        IDLE,
        WALKING
    }

    private R_SOUNDS _currentRepeatedSound = R_SOUNDS.NONE;

    public void PlayIdleSounds()
    {
        if(_currentRepeatedSound != R_SOUNDS.IDLE)
        {
            _currentRepeatedSound = R_SOUNDS.IDLE;
            StopRepeatedSounds();
            PlayIdle();
        }
    }

    public void PlayMovementSounds()
    {
        if (_currentRepeatedSound != R_SOUNDS.WALKING)
        {
            _currentRepeatedSound = R_SOUNDS.WALKING;
            StopRepeatedSounds();
            PlayMove();
        }
    }

    public void PlayAttackSound()
    {
        PlayAttack();
    }
}
