using System;
using UnityEngine;

public class SoundTriggerVolume : SoundTrigger
{
    private void OnTriggerEnter(Collider other)
    {
        TriggerSound(true);
    }

    private void OnTriggerExit(Collider other)
    {
        TriggerSound(false);
    }
}