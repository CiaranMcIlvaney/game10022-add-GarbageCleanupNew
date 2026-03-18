using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashProgress
{
    // How many trash items actually spawned in the level
    public static int TotalSpawned { get; private set; } = 0;

    // How many trash items are still left in the world
    public static int Remaining { get; private set; } = 0;

    // Called once right after spawning is finished
    public static void SetTotal(int total)
    {
        TotalSpawned = Mathf.Max(0, total);
        Remaining = TotalSpawned;

        Debug.Log($"[TrashProgress] Total set to {TotalSpawned}. Remaining set to {Remaining}.");
    }

    // Call this whenever a trash item gets deposited (correct OR wrong)
    public static void OnTrashRemoved()
    {
        Remaining = Mathf.Max(0, Remaining - 1);

        Debug.Log($"[TrashProgress] Remaining: {Remaining}/{TotalSpawned} ({GetPercent()}%)");
    }

    // Percentage of area cleaned
    public static int GetPercent()
    {
        if (TotalSpawned <= 0)
        {
            return 0;
        }

        int cleaned = TotalSpawned - Remaining;
        
        return (cleaned * 100) / TotalSpawned; 
    }
}
