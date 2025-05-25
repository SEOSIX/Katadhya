using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    
    public static LoadingScene SINGLETON { get; private set; }

    void Awake()
    {
        if (SINGLETON != null)
        {
            Destroy(SINGLETON.gameObject);
            return;
        }
        SINGLETON = this;
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
