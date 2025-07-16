using System.Collections;
using System.Collections.Generic;
using Infrastructure;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    private Queue<float> queue;
    private ICoroutineRunner _runner;

    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private AudioSource _audioSource;
    
    [Header("AudioClips")]
    [SerializeField]
    private AudioClip _moveClip;
    [SerializeField]
    private AudioClip _fallClip;
    
    [SerializeField]
    private AudioClip _attachClip;
    [SerializeField]
    private AudioClip _detachClip;
    
    [SerializeField]
    private AudioClip _rotateClip;
    [SerializeField]
    private AudioClip _pullInClip;
    [SerializeField]
    private AudioClip _pullOutClip;
    
    [SerializeField]
    private AudioClip _winClip;
    [SerializeField]
    private AudioClip _loseClip;


    public void PlayMove()
    {
        _audioSource.PlayOneShot(_moveClip);
    }
    
    public void PlayDetach()
    {
        _audioSource.PlayOneShot(_moveClip);
    }
    
    public void PlayAttach()
    {
        _audioSource.PlayOneShot(_moveClip);
    }
    
    public void PlayFall()
    {
        _audioSource.PlayOneShot(_moveClip);
    }
    
    public void PlayPullIn()
    {
        _audioSource.PlayOneShot(_pullInClip);
    }
    
    public void PlayPullOut()
    {
        _audioSource.PlayOneShot(_pullOutClip);
    }

    public void PlayRotate()
    {
        _audioSource.PlayOneShot(_rotateClip);
    }
    
    public void MakeJoinNoise()
    {
        if (queue.Count == 0)
        {
            queue.Enqueue(0);
                //_runner.StartCoroutine(PlayJoinNoise);
        }
        else
        {
            var delay = queue.Peek()+0.1f;
            queue.Enqueue(delay);
            
        }
    }

    /*
    IEnumerator PlayJoinNoise()
    {
        while (queue.TryPeek(out var delay))
        {
            
        }
    }
    */
}
