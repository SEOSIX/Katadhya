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
    public int Combat;

    private void Awake()
    {
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
    [SerializeField] public List<RewardPack> CaurisSpé;
    public GlobalPlayerData BigData;

    public void Récompenses()
    {
        GameManager.SINGLETON.isCombatEnabled = false;
        for (int i = 0; i < 4; i++)
        {
            BigData.AddCauris(CaurisSpé[Combat].values[i], i);
        }
        BigData.baseCaurisCount += CaurisDeBase;

    }
    public void LoadNextCombatScene()
    {
        Combat += 1;
        SceneManager.LoadScene($"Combat{Combat}");
    }
}
