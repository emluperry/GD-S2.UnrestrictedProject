using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySound : MonoBehaviour
{
    //components
    protected AudioSource _source;
    protected EntityHealth _health;

    //common sounds
    [Header("Common sounds")]
    [SerializeField] protected AudioClip[] _idle;
    [SerializeField] protected AudioClip[] _footsteps;
    [SerializeField] protected AudioClip[] _jump;
    [SerializeField] protected AudioClip[] _land;
    [SerializeField] protected AudioClip[] _attack;
    [SerializeField] protected AudioClip[] _die;

    //sound delays
    [SerializeField] protected float _idleDelay = 4f;
    [SerializeField] protected float _footstepDelay = 0.2f;

    protected Coroutine _repeatedSoundCoroutine;

    protected virtual void Awake()
    {
        _source = GetComponent<AudioSource>();
        _health = GetComponent<EntityHealth>();
        _health.onDead += PlayDie;
    }

    protected virtual void OnDestroy()
    {
        _health.onDead -= PlayDie;
    }

    protected IEnumerator c_PlayRepeatedSound(AudioClip[] sounds, float delay)
    {
        if(sounds.Length <= 0)
        {
            _repeatedSoundCoroutine = null;
            yield break;
        }    

        float currentTime = 0;
        while(true)
        {
            yield return new WaitForFixedUpdate();

            currentTime += Time.fixedDeltaTime;

            if(currentTime >= delay)
            {
                yield return new WaitUntil(() => !_source.isPlaying);

                currentTime = 0;

                int soundIndex = PickRandomSound(sounds.Length);

                if (soundIndex > -1)
                {
                    _source.clip = sounds[soundIndex];
                    _source.Play();
                }
            }
        }
    }

    protected void PlaySound(AudioClip[] sounds)
    {
        int soundIndex = PickRandomSound(sounds.Length);

        if (soundIndex > -1)
            AudioSource.PlayClipAtPoint(sounds[soundIndex], transform.position);
    }

    protected int PickRandomSound(int clipLength)
    {
        if (clipLength <= 0)
            return -1;
        if (clipLength == 0)
            return 0;

        return Random.Range(0, clipLength);
    }

    #region ACTIONS

    protected void StopRepeatedSounds()
    {
        if(_repeatedSoundCoroutine != null)
        {
            StopCoroutine(_repeatedSoundCoroutine);
            _repeatedSoundCoroutine = null;
        }
        _source.Stop();
    }

    protected void PlayIdle()
    {
        if(_repeatedSoundCoroutine == null)
        {
            _repeatedSoundCoroutine = StartCoroutine(c_PlayRepeatedSound(_idle, _idleDelay));
        }
    }

    protected void PlayMove()
    {
        if (_repeatedSoundCoroutine == null)
        {
            _repeatedSoundCoroutine = StartCoroutine(c_PlayRepeatedSound(_footsteps, _footstepDelay));
        }
    }

    protected void PlayJump()
    {
        PlaySound(_jump);
    }

    protected void PlayLand()
    {
        PlaySound(_land);
    }

    protected void PlayAttack()
    {
        PlaySound(_attack);
    }

    protected void PlayDie(EntityHealth health)
    {
        PlaySound(_die);
    }

    #endregion
}
