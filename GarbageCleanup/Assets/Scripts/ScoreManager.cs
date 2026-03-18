using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    // Single reference so other scripts can access ScoreManger easily
    public static ScoreManager Instance;

    // Current player score
    public int Score { get; private set; } = 0;

    // Text that displays when trash it deposited
    public string feedback = ("WASD to move, left mouse to pick up/deposite items");

    // Dictionary that tracks how many correct deposits the player has made for each garbage type
    public Dictionary<Garbage, int> Correct = new();

    // Dictionary that tracks how many inccorect deposits the player has made for each garbage type
    public Dictionary<Garbage, int> Wrong = new();

    [Header("Points (Correct Deposit)")]

    // Points given when the player places the correct garbage in the correct bin
    [SerializeField] private int wasteCorrect = 5;
    [SerializeField] private int recyclableCorrect = 10;
    [SerializeField] private int textileCorrect = 10;
    [SerializeField] private int electronicCorrect = 25;

    // Points removed when the player places garbage in the wrong bin
    [Header("Penalty (Wrong Deposit)")]
    [SerializeField] private int wasteWrong = 2;
    [SerializeField] private int recyclableWrong = 5;
    [SerializeField] private int textileWrong = 5;
    [SerializeField] private int electronicWrong = 15;
    [SerializeField] AudioSource Positive;
    [SerializeField] AudioSource Negative;

    private void Start()
    {
        feedback = ("WASD to move, left mouse to pick up/deposite items");
    }
    void Awake()
    {
        // If another ScoreManager already exists then delete this one
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Store reference so other scripts can access ScoreManager
        Instance = this;
        
        // Dont destroy scoremanager game object when game ends
        DontDestroyOnLoad(gameObject);

        // Initialize the dictionaires so each garbage type starts at 0
        foreach (Garbage g in System.Enum.GetValues(typeof(Garbage)))
        {
            Correct[g] = 0;
            Wrong[g] = 0;
        }
    }

    public void AddCorrect(Garbage type)
    {
        // Increase correct count for this garbage type
        Correct[type]++;

        // Add score based on the garbage type
        Score += GetCorrectPoints(type);

        // Print updated score + stats in console
        DebugTotals();

        //Change text
        feedback = ("Success!!");
        // Play Sound
        Positive.Play();
    }

    public void AddWrong(Garbage type)
    {
        // Increase wrong count for this garbage type
        Wrong[type]++;

        // Subtract score based on the garbage type
        Score -= GetWrongPenalty(type);

        //Change text
        feedback = ("Failure! :(");

        // Prevent score from going below zero
        if (Score < 0)
        {
            Score = 0;
        }

        // Print stats to console 
        DebugTotals();

        // Play Sound
        Negative.Play();
    }

    private int GetCorrectPoints(Garbage type)
    {
        // Switch statment checks which garbage type recieved and returns the right score value
        switch (type)
        {
            case Garbage.Waste: return wasteCorrect;
            case Garbage.Recyclable: return recyclableCorrect;
            case Garbage.Textile: return textileCorrect;
            case Garbage.Electronic: return electronicCorrect;
            default: return 0;
        }
    }

    private int GetWrongPenalty(Garbage type)
    {
        // Same logic as correct poitns but returns penalty values instead
        switch (type)
        {
            case Garbage.Waste: return wasteWrong;
            case Garbage.Recyclable: return recyclableWrong;
            case Garbage.Textile: return textileCorrect;
            case Garbage.Electronic: return electronicWrong;
            default: return 0;
        }
    }

    private void DebugTotals()
    {
        // Print total score
        Debug.Log($"[SCORE] {Score}");

        // Print how many correct deposits have been made
        Debug.Log($"[Correct] Waste:{Correct[Garbage.Waste]} Plastic:{Correct[Garbage.Recyclable]} Paper:{Correct[Garbage.Textile]} Electronic:{Correct[Garbage.Electronic]}");
        
        // Print how many wrong deposits have been made 
        Debug.Log($"[Wrong]   Waste:{Wrong[Garbage.Waste]} Plastic:{Wrong[Garbage.Recyclable]} Paper:{Wrong[Garbage.Textile]} Electronic:{Wrong[Garbage.Electronic]}");
    }

    public void ResetScoreData()
    {
        Score = 0;
        feedback = ("WASD to move, left mouse to pick up/deposite items");

        foreach (Garbage g in System.Enum.GetValues(typeof(Garbage)))
        {
            Correct[g] = 0;
            Wrong[g] = 0;
        }
    }
}
