using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExplorationManager : MonoBehaviour
{
    public static ExplorationManager SINGLETON;

    private void Start()
    {
        if (this.enabled == false) this.enabled = true;
    }
    private void Awake()
    {
        if (SINGLETON != null && SINGLETON != this)
        {
            Destroy(gameObject);
            return;
        }

        SINGLETON = this;
        DontDestroyOnLoad(gameObject);
    }

    public LevelDesign LD;
    public GlobalPlayerData BigData;

    [Header("Scenes")]
    public string CombatScene;
    public string AutelQTEScene;
    public string AutelStatScene;
    public string HealingScene;

    public string EntranceScene;
    public string ExitScene;
    public bool FirstRoom = true;



    [Header("Values")]
    public int CombatIndex = 0;

    public void StartLoadNextCombatScene()
    {
        StartCoroutine(LoadNextCombatScene());
    }
    public IEnumerator LoadNextCombatScene()
    {
        SceneManager.LoadScene(CombatScene);
        CombatIndex += 1;
        while (SceneManager.GetActiveScene().name != CombatScene)
        {
            yield return null;
        }
        ChoicesHolder V = null;
        if (ChoicesHolder.SINGLETON != null) V = ChoicesHolder.SINGLETON;
        GameManager.SINGLETON.currentCombat = LD.CombatList[CombatIndex];
        Debug.Log(V);
        Debug.Log(V.combatUI);
        V.combatUI.SetActive(true);
    }


    public void LoadChoicesAfterCombat()
    {
        ChoicesHolder V = null;
        if (ChoicesHolder.SINGLETON != null) V = ChoicesHolder.SINGLETON;

        Combat combat = GameManager.SINGLETON.currentCombat;
        if (combat.RoomOptions.Length == 0 || combat.WaveList.Count == 0)
        {
            Debug.LogWarning("pas de sorties");
            return;
        }
        foreach (string option in combat.RoomOptions)
        {
            if (option == CombatScene) V.CombatSceneButton.SetActive(true);
            if (option == AutelQTEScene) V.QTESceneButton.SetActive(true);
            if (option == AutelStatScene) V.StatSceneButton.SetActive(true);
            if (option == HealingScene) V.HealingSceneButton.SetActive(true);
        }
    }

    public void LoadChoicesFromList(string[] Options)
    {
        ChoicesHolder V = null;
        if (ChoicesHolder.SINGLETON != null) V = ChoicesHolder.SINGLETON;

        foreach (string option in Options)
        {
            if (option == CombatScene) V.CombatSceneButton.SetActive(true);
            if (option == AutelQTEScene) V.QTESceneButton.SetActive(true);
            if (option == AutelStatScene) V.StatSceneButton.SetActive(true);
            if (option == HealingScene) V.HealingSceneButton.SetActive(true);
        }
    }

    public void LoadNextCombatChoice()
    {
        ChoicesHolder V = null;
        if (ChoicesHolder.SINGLETON != null) V = ChoicesHolder.SINGLETON;
        V.CombatSceneButton.SetActive(true);
    }
    public void StartLoadScene(string name)
    {
        StartCoroutine(LoadScene(name));
    }
    public IEnumerator LoadScene(string SceneName)
    {
        //FadeManager.Instance.FadeOut();
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(SceneName);
    }

    public void Recompenses()
    {
        GameManager.SINGLETON.isCombatEnabled = false;
        Combat C = LD.CombatList[CombatIndex];
        List<int> CauriSpe = new List<int>() 
            {C.CaurisSpe1,
                C.CaurisSpe2,
                C.CaurisSpe3,
                C.CaurisSpe4};
        for (int i = 0; i < 4; i++)
        {
            BigData.AddCauris(CauriSpe[i], i);
        }
        BigData.AddGlobalCauris(C.CaurisDor);
    }


}



