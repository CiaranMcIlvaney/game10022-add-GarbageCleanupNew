using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [Header("Inventory")]
    
    // Maximum number of items the player can carry
    [SerializeField] private int maxItems = 5;
    
    // List stores all the garbage GameObjects the player has picked up
    private readonly List<GameObject> items = new();

    // Keeps track of which item is currently selected
    private int selectedIndex = 0;

    // Returns true if backpack is full
    public bool IsFull => items.Count >= maxItems;

    // Returns how many items are currently in the backpack
    public int Count => items.Count;

    // Returns the currnetly selected item, if the inventory is empty then return null
    public GameObject CurrentItem => (items.Count == 0) ? null : items[Mathf.Clamp(selectedIndex, 0, items.Count - 1)];

    // Lets UI read what items are in the inventory
    public IReadOnlyList<GameObject> Items => items;

    // Lets UI know which slot is selected
    public int SelectedIndex => selectedIndex;

    [Header("Audio")]

    [SerializeField] private AudioSource pickupSound;


    void Update()
    {
        // Get scroll wheel input
        float scroll = Input.mouseScrollDelta.y;

        // Only allow switching if we actually have items
        if (items.Count > 0 && scroll != 0f)
        {
            // Scroll up is move selection backward
            if (scroll > 0f)
            {
                selectedIndex--;
            }
            // Scroll down is move selection forward
            else
            {
                selectedIndex++;
            }

            // Wrap around logic, if goes below 0 then go to last item
            if (selectedIndex < 0)
            {
                selectedIndex = items.Count - 1;
            }
            // If we go past the last item, go back to first
            if (selectedIndex >= items.Count)
            {
                selectedIndex = 0;
            }

            // Debug log showing what is currently selected
            Debug.Log($"[Inventory] Selected: {GetItemName(CurrentItem)} ({selectedIndex + 1}/{items.Count})");
        }
    }

    // Called when the player picks something up
    public bool TryAdd(GameObject item)
    {
        // If inventory is full, stop here
        if (IsFull)
        {
            Debug.Log("[Inventory] Backpack full (max 5). Can't pick up.");
            return false;
        }

        // Disable all the colliders on the object so it doesnt interfere with player movement
        foreach (var colliders in item.GetComponentsInChildren<Collider>())
        {
            colliders.enabled = false;
        }

        // Parent the item to the player object so it stays attatched logically
        item.transform.SetParent(transform);

        // Hide object from scene (Might need to be changed later? IDK)
        item.SetActive(false);

        // Add it to the inventory list
        items.Add(item);

        pickupSound.Play();

        // If this is the first item picked up then automatically select it
        if (items.Count == 1)
        {
            selectedIndex = 0;
        }

        // Debug feedback
        Debug.Log($"[Inventory] Picked up: {GetItemName(item)} (Now {items.Count}/{maxItems})");
        Debug.Log($"[Inventory] In hand: {GetItemName(CurrentItem)}");

        return true;
    }

    // Called when depositing into a bin
    public GameObject RemoveCurrent()
    {
        // If inventory is empty then return null
        if (items.Count == 0)
        {
            return null;
        }

        // Get the currently selected item
        GameObject item = CurrentItem;

        // Remove it from the list
        items.RemoveAt(selectedIndex);

        // Adjust selected list so it stays valid
        if (items.Count == 0)
        {
            selectedIndex = 0;
        }
        else if (selectedIndex >= items.Count)
        {
            selectedIndex = items.Count - 1;
        }

        // Debug feedback
        Debug.Log($"[Inventory] Removed: {GetItemName(item)} (Remaining {items.Count}/{maxItems})");
        Debug.Log($"[Inventory] Now selected: {GetItemName(CurrentItem)}");

        return item;
    }

    // Returns the garbage enum type of the current item
    public Garbage? GetCurrentGarbageType()
    {
        var item = CurrentItem;

        if (item == null)
        {
            return null;
        }

        // Try same object, parent, then children (even if they are inactive)
        var gc = item.GetComponent<GarbageController>();

        if (gc == null)
        {
            gc = item.GetComponentInParent<GarbageController>();
        }

        if (gc == null)
        {
            gc = item.GetComponentInChildren<GarbageController>(true);
        }

        return gc.data.garbageType;
    }

    // Returns the name of the GameObject (For debugging)
    private string GetItemName(GameObject item)
    {
        if (item == null)
        {
            return "(None)";
        }
        var gc = item.GetComponent<GarbageController>();

        if (gc == null)
        {
            gc = item.GetComponentInChildren<GarbageController>(true);
        }

        return (gc != null) ? $"{item.name} [{gc.data.garbageType}]" : item.name;
    }
}
