using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinController : MonoBehaviour
{
    [Header("Waste Type")]

    // What type of garbage this bin accepts
    public Garbage acceptedType = Garbage.Waste;

    [Header("Animation")]
    public Animator lidAnimator;

    // Called when the player interacts with the bin
    public void TryDeposit(InventoryController inventory)
    {
        // Ask inventory what type is currently selected
        Garbage? currentType = inventory.GetCurrentGarbageType();

        // If null then the player has nothing selected / the inventory is empty
        if (currentType == null)
        {
            Debug.Log("[Bin] No item in hand to deposit.");
            return;
        }

        // Remove the item from inventory 
        GameObject removed = inventory.RemoveCurrent();

        if (removed != null)
        {
            Destroy(removed);
        }

        // Count it as removed from the map (correct OR wrong)
        TrashProgress.OnTrashRemoved();

        // Correct bin
        if (currentType.Value == acceptedType)
        {
            Debug.Log($"SUCCESS! Deposited {currentType.Value} into {acceptedType} bin.");
            ScoreManager.Instance.AddCorrect(currentType.Value);
        }

        // Wrong bin
        else
        {
            Debug.Log($"FAIL! You tried to deposit {currentType.Value} into {acceptedType} bin.");
            ScoreManager.Instance.AddWrong(currentType.Value);
        }
    }

    public void MagnetToGround(LayerMask ground)
    {
        RaycastHit hit;

        // Move the bin up so it can raycast to the ground properly
        transform.position += new Vector3(0, 10, 0);

        // Detect where the ground is and place the bin on it
        // Also make sure the rotation and position above the ground is correct
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity, ground))
        {
            transform.position = hit.point;
            transform.rotation = Quaternion.identity;
            transform.position += new Vector3(0, 0.6f, 0);
        }
    }

    private void OnTriggerEnter(Collider player)
    {
        if (player.gameObject.name == "Player")
        {
            // Open the lid
            lidAnimator.Play("LidOpen");
        }
    }
    private void OnTriggerExit(Collider player)
    {
        if (player.gameObject.name == "Player")
        {
            // Close the lid
            lidAnimator.Play("LidClose");
        }
    }
}
