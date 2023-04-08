using System;
using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    public Action<bool> onSoundTriggered;

    public void TriggerSound(bool shouldPlay)
    {
        onSoundTriggered?.Invoke(shouldPlay);
    }
}