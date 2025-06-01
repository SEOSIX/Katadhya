using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    // Nom de la sc�ne � charger
    [SerializeField] private string sceneName = "testGarde";

    // M�thode appel�e par le bouton
    public void PlayGame()
    {
        SceneManager.LoadScene("testGarde");
    }
}
