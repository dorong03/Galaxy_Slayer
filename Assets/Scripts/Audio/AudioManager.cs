using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] _audioClip;
    public AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayClip(int number)
    {
        _audioSource.clip = _audioClip[number];
        _audioSource.Play();
        Debug.Log(number);
    }
}
