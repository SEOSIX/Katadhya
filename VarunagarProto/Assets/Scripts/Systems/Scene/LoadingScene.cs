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
    public IEnumerator LoadScene(string SceneName)
    {
        yield return new WaitForSeconds(0f);
        SceneManager.LoadScene(SceneName);
    }
}
