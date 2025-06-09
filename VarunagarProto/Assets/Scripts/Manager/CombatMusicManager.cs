using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatMusicManager : MonoBehaviour
{
    public AudioClip Combat1;
    public AudioClip Combat2;

    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(MusicLoop());
    }

    // Update is called once per frame
    private IEnumerator MusicLoop()
    {
        this.GetComponent<AudioSource>().clip = Combat1;
        this.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(Combat1.length-7f);
        this.GetComponent<AudioSource>().clip = Combat2;
        this.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(Combat2.length-8f);
        StartCoroutine(MusicLoop());

    }
}
