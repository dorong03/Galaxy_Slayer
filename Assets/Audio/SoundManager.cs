using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private int poolSize = 25;
    private List<AudioSource> audioPool;

    [Header("Audio Clips")] 
    public AudioClip mapExpansion1;
    public AudioClip monsterHit2;
    public AudioClip monsterWarning;
    public AudioClip monsterExplosion;
    public AudioClip playerAttack1;
    public AudioClip playerLand1;
    public AudioClip playerLandAttack;
    public AudioClip titleStart2;
    public AudioClip gameOverRetry;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CreateAudioPool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void CreateAudioPool()
    {
        audioPool = new List<AudioSource>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = new GameObject("AudioSource_" + i);
            obj.transform.parent = this.transform;
            AudioSource source = obj.AddComponent<AudioSource>();
            audioPool.Add(source);
        }
    }

    private AudioSource GetAvailableSource()
    {
        foreach (AudioSource source in audioPool)
        {
            if (!source.isPlaying)
                return source;
        }
        return audioPool[0];
    }

    public void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (clip == null) return;

        AudioSource source = GetAvailableSource();
        source.volume = volume;
        source.pitch = pitch;
        source.PlayOneShot(clip);
    }
}