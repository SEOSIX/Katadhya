using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class AudioManager : MonoBehaviour
{
    public static AudioManager SINGLETON { get; private set; }
    private void Awake()
    {
        if (SINGLETON != null)
        {
            Destroy(gameObject);
            return;
        }
        SINGLETON = this;
    }
    //CombatClips : 0: Damage ,1: Damage to shield, 2: Chock, 3: Chock prock, 4 : Nécrose, 5 : Rage, 6: Buff, 7 : Débuff, 8 : Heal, 9: Ult Chargé
    //GameClips : 0 : Click, 1 : Click Cpt, 2 : play, 3 : Nouvelle Vague, 
    public AudioClip[] combatClips = new AudioClip[10];
    public AudioClip[] gameClips = new AudioClip[4];
    public GameObject AudioSources;
    public IEnumerator PlayClip(AudioClip clip)
    {
        AudioSource source = AudioSources.AddComponent<AudioSource>();
        source.PlayOneShot(clip,0.1f);
        yield return new WaitForSeconds(clip.length);
        Destroy(source);
    }

    public IEnumerator PlayCombatClip(int index)
    {
        AudioSource source = AudioSources.AddComponent<AudioSource>();

        if (combatClips[index] != null)
        {
            yield return new WaitForSeconds(0.2f);
            AudioClip clip = combatClips[index]; 
            source.PlayOneShot(clip, 0.1f);
            yield return new WaitForSeconds(clip.length);
        }
        Destroy(source);
    }
    public IEnumerator PlayGameClip(int index)
    {
        AudioSource source = AudioSources.AddComponent<AudioSource>();

        if (gameClips[index] != null)
        {
            AudioClip clip = gameClips[index];
            source.PlayOneShot(clip, 0.1f);
            yield return new WaitForSeconds(clip.length);
        }
        Destroy(source);
    }
}
