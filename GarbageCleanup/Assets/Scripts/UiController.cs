using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiController : MonoBehaviour
{
    [Header("Main UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI cleanedText;

    [Header("Correct Deposits")]
    [SerializeField] private TextMeshProUGUI wasteText;
    [SerializeField] private TextMeshProUGUI recyclableText;
    [SerializeField] private TextMeshProUGUI textileText;
    [SerializeField] private TextMeshProUGUI electronicText;

    [Header("TEMPORARY PLAYTEST TUTORIAL")]
    [SerializeField] private TextMeshProUGUI tempText;
    bool textToggle; 
    void Update()
    {
        // Score
        scoreText.text = $"{ScoreManager.Instance.Score}";

        // Cleaned percentage of map
        int percent = TrashProgress.GetPercent();
        cleanedText.text = $"{percent}%";

        // Correct counts of items placed in the correct bins
        wasteText.text = $"{ScoreManager.Instance.Correct[Garbage.Waste]}";
        recyclableText.text = $"{ScoreManager.Instance.Correct[Garbage.Recyclable]}";
        textileText.text = $"{ScoreManager.Instance.Correct[Garbage.Textile]}";
        electronicText.text = $"{ScoreManager.Instance.Correct[Garbage.Electronic]}";

        // Playtest Tutorial

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            textToggle = !textToggle;
        }
        if (textToggle)
        {
            tempText.text = $"WASD to Move    \r\nClick Trash to Collect\r\nClick Bin to Deposit\r\nScroll to Switch Selected Trash\r\nSpace to Jump";
        }
        if (!textToggle)
        {
            tempText.text = $""; 
        }
    }
}
