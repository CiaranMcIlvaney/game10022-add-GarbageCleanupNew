using System.Collections;
using TMPro;
using UnityEngine;

public class FactPopupUI : MonoBehaviour
{
    // Lets scripts access this 
    public static FactPopupUI Instance;

    [Header("Popup References")]
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI bodyText;

    [Header("Popup Timing")]
    [SerializeField] private float showTime = 4f;

    // Used to keep track of the current popup timer coroutine
    private Coroutine currentRoutine;

    void Awake()
    {
        // Set this script as the global instance so other scripts can call it
        Instance = this;

        // Make sure popup starts hidden when the game loads
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }
    }

    // This is specifically for WRONG bin messages
    public void ShowWrongBinPopup(string itemName, Garbage correctType, string explanation)
    {
        // Title text is Wrong Bin
        string finalTitle = "Wrong Bin";

        // Makes the full sentence using the item + explaination
        string finalBody = $"{itemName} should go in the {correctType} bin because {explanation}";

        // Call the general popup function
        ShowPopup(finalTitle, finalBody);
    }

    public void ShowPopup(string title, string body)
    {
        // If a popup is already showing stop its timer so it doesnt conflict 
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        // Update the text on screen
        titleText.text = title;
        bodyText.text = body;

        // Turn the popup on
        popupPanel.SetActive(true);

        // Start the countdown to hide it after a few seconds
        currentRoutine = StartCoroutine(HideAfterDelay());
    }

    // Waits for showTime amount then rehides the popup
    private IEnumerator HideAfterDelay()
    {
        // Wait for the set time
        yield return new WaitForSeconds(showTime);

        // Turn popup off
        popupPanel.SetActive(false);

        // Clear coroutine reference
        currentRoutine = null;
    }
}