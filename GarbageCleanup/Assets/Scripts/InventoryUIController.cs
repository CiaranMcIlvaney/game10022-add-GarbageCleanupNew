using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
{
    [System.Serializable]
    public class SlotUI
    {
        public Image highlight;  // The visual highlight for the selected slot
        public Image icon; // The image that displays the items icon
    }

    [Header("References")]

    // Reference to the players InventoryController script
    [SerializeField] private InventoryController inventory;

    // list of all hotbar slots in the UI
    [SerializeField] private List<SlotUI> slots = new();

    // Remembers the last known inventory state (used to refresh UI when something changes)
    private int lastCount = -1;
    private int lastSelected = -1;

    void Start()
    {
        Refresh();
    }

    // Checks if the inventory has changed
    void Update()
    {
        // Current state of the inventory
        int count = inventory.Items.Count;
        int selected = inventory.SelectedIndex;

        // If the number of items or the selected slot changed than update the UI to show that change
        if (count != lastCount || selected != lastSelected)
        {
            Refresh();
        }
    }

    // Function to update all hotbar UI elements to match the current state of the players inventory
    public void Refresh()
    {
        // Store the latest inventory state so we can compare later
        lastCount = inventory.Items.Count;
        lastSelected = inventory.SelectedIndex;

        // Loop through every UI slot in the hotbar
        for (int i = 0; i < slots.Count; i++)
        {
            // If the slot has a highlit image assigned
            if (slots[i].highlight != null)
            {
                // Enable the highlight only if the inventory has items, and this slot index matches the selected slot
                slots[i].highlight.enabled = (inventory.Items.Count > 0 && i == lastSelected);
            }

            // Check if this slot corresponds to a real item in the inventory
            if (i < inventory.Items.Count && inventory.Items[i] != null)
            {
                // Turn the icon on
                slots[i].icon.enabled = true;

                // Get the sprite associated with that item and display it in the UI slot
                slots[i].icon.sprite = GetItemIcon(inventory.Items[i]);
            }
            else
            {
                // If no item exists in this slot then hide the icon and clear the sprite
                slots[i].icon.enabled = false;
                slots[i].icon.sprite = null;
            }
        }
    }

    // This retrieves the icon sprite from a garbage item
    private Sprite GetItemIcon(GameObject item)
    {
        // Look for a GarbageController component on the object or any of its children
        var gc = item.GetComponentInChildren<GarbageController>(true);

        // Return the icon stored in the items GarbageData
        return gc.data.icon;
    }
}
