using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

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
