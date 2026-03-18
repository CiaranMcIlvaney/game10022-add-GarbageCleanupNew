using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // ALPHA ONLY
    // Keep the timer at 3 minutes for the alpha test
    [Header("Game Time")]
    private float maxTime = 180f;
    [field: SerializeField] private float currentTimeRemaining;
    public TextMeshProUGUI timerText;

    [Header("Lighting")]
    public Light skyLight;

    // Start is called before the first frame update
    void Start()
    {
        currentTimeRemaining = maxTime;
        skyLight.transform.Rotate(200, -900, 180);
    }

    // Update is called once per frame
    void Update()
    {
        // Decrease time
        currentTimeRemaining -= Time.deltaTime;

        // End game if timer runs out
        if (currentTimeRemaining <= 0)
        {
            SceneManager.LoadScene("GameEnd");
        }

        // Float to proper time formatting
        int minutes = Mathf.FloorToInt(currentTimeRemaining / 60);
        int seconds = Mathf.FloorToInt(currentTimeRemaining - minutes * 60);
        string gameTimeString = string.Format("{0:0}:{1:00}", minutes, seconds);

        // Display time left
        timerText.text = $"Time: {gameTimeString}";

        // Change light rotation based on the current time of day
        skyLight.transform.Rotate(Vector3.left, 1 * Time.deltaTime);
    }
}
