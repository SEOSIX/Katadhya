using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CombatMusicManager : MonoBehaviour
{
    public AudioClip Combat1;
    public AudioClip Combat2;
    public static CombatMusicManager SINGLETON {get; private set; }

    private void Awake()
    {
        if (SINGLETON != null)
        {
            Destroy(gameObject);
            return;
        }
        SINGLETON = this;
        StartCoroutine(MusicLoop());
    }

    private IEnumerator MusicLoop()
    {
        this.GetComponent<AudioSource>().clip = Combat1;
        this.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(Combat1.length-6f);
        this.GetComponent<AudioSource>().clip = Combat2;
        this.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(Combat2.length-7f);
        StartCoroutine(MusicLoop());

    }

    public void CutMusic()
    {
        this.GetComponent<AudioSource>().volume = 0f;
    }
}
