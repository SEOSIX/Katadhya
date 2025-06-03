using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExplorationManager : MonoBehaviour
{
    public static ExplorationManager SINGLETON;

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

    [Header("Scenes")]
    public string CombatScene;
    public string AutelQTEScene;
    public string AutelStatScene;
    public string HealingScene;

    public string EntranceScene;
    public string ExitScene;

    [Header("Buttons")]
    public GameObject CombatSceneButton;
    public GameObject QTESceneButton;
    public GameObject StatSceneButton;
    public GameObject HealingSceneButton;

    [Header("Values")]
    public int CombatIndex = 0;

    public void LoadNextCombatScene()
    {
        SceneManager.LoadScene(CombatScene);
        GameManager.SINGLETON.currentCombat = LD.CombatList[CombatIndex];
        CombatIndex += 1;
    }

    public void LoadChoicesAfterCombat()
    {
        Combat combat = GameManager.SINGLETON.currentCombat;
        if (combat.RoomOptions.Length == 0 || combat.WaveList.Count == 0)
        {
            Debug.Log("pas de sorties");
            return;
        }
        foreach (string option in combat.RoomOptions)
        {
            if (option == CombatScene) CombatSceneButton.SetActive(true);
            if (option == AutelQTEScene) QTESceneButton.SetActive(true);
            if (option == AutelStatScene) StatSceneButton.SetActive(true);
            if (option == HealingScene) HealingSceneButton.SetActive(true);
        }
    }
    public void StartLoadScene(string name)
    {
        StartCoroutine(LoadScene(name));
    }
    public IEnumerator LoadScene(string SceneName)
    {
        FadeManager.Instance.FadeOut();
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(SceneName);
    }
}
[System.Serializable]
public class Combat
{
    public List<EnemyPack> WaveList = new List<EnemyPack>();
    public int NextCombatIndex;
    public int CaurisDor;
    public int CaurisSpe1;
    public int CaurisSpe2;
    public int CaurisSpe3;
    public int CaurisSpe4;

    public string[] RoomOptions = new string[2];
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObject/LevelDesign", order = 6)]
public class LevelDesign : ScriptableObject
{
    public Combat[] CombatList;
}
