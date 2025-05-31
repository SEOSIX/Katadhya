using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;

public class LoadingScene : MonoBehaviour
{
    
    public static LoadingScene SINGLETON { get; private set; }
    public GlobalGameData globalGameData;
    public Transform BackgroundParent;

    void Awake()
    {
        if (SINGLETON != null)
        {
            Destroy(SINGLETON.gameObject);
            return;
        }
        SINGLETON = this;
        GameObject Background = globalGameData.CombatBackgroundPrefabs[Random.Range(0,globalGameData.CombatBackgroundPrefabs.Length-1)];
        globalGameData.LoadBackground(BackgroundParent, Background);
    }

    public void LoadNextSceneAsync()
    {
        FadeManager.Instance.FadeOut(() =>
        {
            StartCoroutine(LoadNextScene());
        });
    }

    private IEnumerator LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        yield return new WaitForSeconds(0f);
        SceneManager.LoadScene(nextSceneIndex);
    }
    public void LoadScene(string SceneName)
    {
        FadeManager.Instance.FadeOut(() =>
        {
            StartCoroutine(LoadSceneByName(SceneName));
        });
    }
    private IEnumerator LoadSceneByName(string Name)
    {

        yield return new WaitForSeconds(0f);
        SceneManager.LoadScene(Name);
    }
}
