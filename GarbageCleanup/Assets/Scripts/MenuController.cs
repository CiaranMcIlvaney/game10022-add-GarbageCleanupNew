using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void OnExitGame()
    {
        Application.Quit();
    }

    public void OnPlayAgain()
    {
        // CHANGE FOR PLAYTEST
        //SceneManager.LoadScene("IsaacSceneGameLogic");
        
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScoreData();
        }
        

        SceneManager.LoadScene("CiaranScene");
    }
}
