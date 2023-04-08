using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Sound_Trigger_Pair
{
    public AudioClip soundClip;
    public SoundTrigger trigger;

    public Action<bool, AudioClip> onSoundTriggered;

    public void StartListeningForEvents()
    {
        if(trigger)
            trigger.onSoundTriggered += TriggerSound;
    }

    public void StopListeningForEvents()
    {
        if(trigger)
            trigger.onSoundTriggered -= TriggerSound;
    }

    private void TriggerSound(bool shouldPlay)
    {
        onSoundTriggered?.Invoke(shouldPlay, soundClip);
    }
}

public class Music_Manager : MonoBehaviour
{
    [SerializeField] private Sound_Trigger_Pair[] _musicTriggers;
    private Stack<AudioClip> _musicStack = new Stack<AudioClip>();

    //components
    private AudioSource _source;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();

        foreach(Sound_Trigger_Pair pair in _musicTriggers)
        {
            pair.StartListeningForEvents();
            pair.onSoundTriggered += PlayMusicClip;
        }

        //play default sound
        if (_source.clip != null)
        {
            _musicStack.Push(_source.clip);
        }
        else if (_musicTriggers.Length > 0)
        {
            PlayClip(_musicTriggers[0].soundClip);
        }
    }

    private void OnDestroy()
    {
        foreach (Sound_Trigger_Pair pair in _musicTriggers)
        {
            pair.StopListeningForEvents();
            pair.onSoundTriggered -= PlayMusicClip;
        }
    }

    private void PlayMusicClip(bool shouldPlay, AudioClip music)
    {
        _source.Stop();

        if (shouldPlay)
        {
            //play next clip and add to stack
            PlayClip(music);
        }
        else
        {
            //play last clip in stack
            if (_musicStack.Count > 0)
            {
                _musicStack.Pop();
            }

            if (_musicStack.Count > 0)
            {
                _source.clip = _musicStack.Peek();
                _source.Play();
            }
        }
    }

    private void PlayClip(AudioClip music)
    {
        _source.clip = music;
        _source.Play();

        _musicStack.Push(music);
    }
}
