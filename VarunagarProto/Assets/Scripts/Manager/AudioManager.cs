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
        DontDestroyOnLoad(gameObject);
    }
    //CombatClips : 0: Damage ,1: shield , 2: d�g�ts sur shield, 3: shield bris� , 4: Chock , 5 : Chock prock,  6 : N�crose, 7 : n�crose damage, 8 : Rage, 9 : Rage Damage, 10: Buff, 11 : D�buff, 12 : Dodge, 13 : QTE Roll, 14 : QTE flop, 15 : QTE Success, 16 : Heal, 17 : Crit
    //GameClips : 0 : Click, 1 : Click Cpt, 2 : play, 3 : Nouvelle Vague, 4 : Mort joueur, 5 : Mort ennemi, 6 : Ult Charged
    public AudioClip[] combatClips = new AudioClip[20];
    public AudioClip[] gameClips = new AudioClip[10];
    public GameObject AudioSources;
    public IEnumerator PlayClip(AudioClip clip)
    {
        AudioSource source = AudioSources.AddComponent<AudioSource>();
        source.PlayOneShot(clip,0.1f);
        yield return new WaitForSeconds(clip.length);
        Destroy(source);
    }

    public IEnumerator PlayCombatClip(int index, float delay = 0.2f)
    {
        AudioSource source = AudioSources.AddComponent<AudioSource>();
        if (combatClips[index] != null)
        {
            yield return new WaitForSeconds(delay);
            AudioClip clip = combatClips[index]; 
            source.PlayOneShot(clip, 0.1f);
            yield return new WaitForSeconds(clip.length);
        }
        Destroy(source);
    }
    public IEnumerator PlayGameClip(int index, float delay = 0.2f)
    {
        AudioSource source = AudioSources.AddComponent<AudioSource>();

        if (gameClips[index] != null)
        {
            yield return new WaitForSeconds(delay);
            AudioClip clip = gameClips[index];
            source.PlayOneShot(clip, 0.1f);
            yield return new WaitForSeconds(clip.length);
        }
        Destroy(source);
    }
}
