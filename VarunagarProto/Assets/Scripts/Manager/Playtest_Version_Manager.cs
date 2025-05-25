using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class RewardPack
{
    public List<int> values = new();
}
public class Playtest_Version_Manager : MonoBehaviour
{
    private void Awake()
    {
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


    public int Combat;
    [SerializeField] public int CaurisDeBase;
    [SerializeField] public List<RewardPack> CaurisSpé;
    public GlobalPlayerData BigData;

    public void Récompenses()
    {
        for (int i = 0; i < 4; i++)
        {
            BigData.AddCauris(CaurisSpé[Combat].values[i], i);
        }
        BigData.baseCaurisCount += CaurisDeBase;

    }
    public void LoadNextCombatScene()
    {
        SceneManager.LoadScene($"Combat {Combat}");
    }
}
