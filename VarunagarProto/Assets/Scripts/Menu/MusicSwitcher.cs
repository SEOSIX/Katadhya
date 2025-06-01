using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSwitcher : MonoBehaviour
{
    [SerializeField] private AudioSource menuMusicSource;
    [SerializeField] private AudioSource introMusicSource;
    [SerializeField] private AudioSource creditsMusicSource;

    void Start()
    {
        PlayMenuMusic(); 
    }

    public void PlayCreditsMusic()
    {
        if (menuMusicSource.isPlaying)
            menuMusicSource.Stop();
        if (introMusicSource.isPlaying)
            introMusicSource.Stop();

        creditsMusicSource.Play();
    }

    public void PlayMenuMusic()
    {
        if (creditsMusicSource.isPlaying)
            creditsMusicSource.Stop();

        menuMusicSource.Play();
    }
}
