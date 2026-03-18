using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Accessibility;

public class GameEndUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI finalScoreText;

    private void Start()
    {
        finalScoreText.text = $"Final Score: {ScoreManager.Instance.Score}";
    }
}
