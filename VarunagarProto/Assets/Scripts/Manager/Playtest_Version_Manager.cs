using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class RewardPack
{
    public List<int> values = new();
}
public class Playtest_Version_Manager : MonoBehaviour
{
    public static Playtest_Version_Manager SINGLETON { get; private set; }

    private void Awake()
    {
        BigData.Combat = 0;
        if (SINGLETON != null)
        {
            Destroy(gameObject);
            return;
        }
        SINGLETON = this;
        DontDestroyOnLoad(this);

    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (Time.timeScale == 5)
            {
                Time.timeScale = 1;
            }
            else
            {
                Debug.Log("SPEED UP");
                Time.timeScale = 5;
            }
        }
    }


    [SerializeField] public int CaurisDeBase;
    [SerializeField] public List<RewardPack> CaurisSpe;
    public GlobalPlayerData BigData;

    public void Recompenses(int index)
    {
        GameManager.SINGLETON.isCombatEnabled = false;
        for (int i = 0; i < 4; i++)
        {
            BigData.AddCauris(CaurisSpe[index].values[i], i);
            Debug.Log($"R�comp {index}");
        }
        BigData.baseCaurisCount += CaurisDeBase;

    }
    public void LoadNextCombatScene()
    {
        BigData.Combat += 1;
        Debug.Log($"Debut du combat : {BigData.Combat}");
        SceneManager.LoadScene($"Combat{BigData.Combat}");
    }
}
