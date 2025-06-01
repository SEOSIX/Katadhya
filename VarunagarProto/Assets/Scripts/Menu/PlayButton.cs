using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    // Nom de la scène à charger
    [SerializeField] private string sceneName = "testGarde";

    // Méthode appelée par le bouton
    public void PlayGame()
    {
        SceneManager.LoadScene("testGarde");
    }
}
