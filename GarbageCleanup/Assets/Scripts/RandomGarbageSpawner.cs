using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RandomGarbageSpawner : MonoBehaviour
{
    // Prefabs that can spawn on the map
    [Header("Garbage Prefabs By Type")]

    [SerializeField] private List<GameObject> wastePrefabs = new();
    [SerializeField] private List<GameObject> recyclablePrefabs = new();
    [SerializeField] private List<GameObject> textilePrfabs = new();
    [SerializeField] private List<GameObject> electronicPrefabs = new();

    [Header("Spawn Chances")]

    [Range(0f, 1f)][SerializeField] private float wasteChance = 0.35f;
    [Range(0f, 1f)][SerializeField] private float recyclableChance = 0.35f;
    [Range(0f, 1f)][SerializeField] private float textileChance = 0.25f;
    [Range(0f, 1f)][SerializeField] private float electronicChance = 0.05f;


    [Header("Item Spawn Count")]

    // How many objects can actually spawn
    [SerializeField] private int spawnCount = 50;

    [Header("Spawn Area")]

    // Center point of the spawn area in the world space
    [SerializeField] private Vector3 areaCenter = Vector3.zero;

    // How wide and long the spawn area is
    [SerializeField] private Vector2 areaSize = new Vector2(200f, 200f);

    [Header("Ground Settings")]

    // Tells the raycast what layer counts as "ground"
    [SerializeField] private LayerMask groundMask;

    // How high above the map the raycast start
    [SerializeField] private float rayStartHeight = 200f;

    // Small vertical offset so garbage doesnt clip into the ground
    [SerializeField] private float yOffset = 0.02f;

    [Header("Spacing")]

    // Minimum distance between spawned objects
    [SerializeField] private float minDistanceBetween = 1.5f;

    // Stores every position already used
    [SerializeField] private readonly List<Vector3> usedPositions = new();

    void Start()
    {
        SpawnAll();
    }

    // Spawn Function
    public void SpawnAll()
    {
        // Remove previously spawned garbage if there is any
        ClearSpawned();

        // Cleared the stored positions from last spawn
        usedPositions.Clear();

        int spawned = 0; // How many objects successfully spawned 
        int attempts = 0; // How many times tried to find position
        
        // Prevents infinite loops if spacing is too strict
        int maxAttempts = spawnCount * 50;

        // Keep trying until enough garbage spawned
        while (spawned < spawnCount && attempts < maxAttempts)
        {
            attempts++;

            // Pick random X/Z
            float x = Random.Range(-areaSize.x * 0.5f, areaSize.x * 0.5f);
            float z = Random.Range(-areaSize.y * 0.5f, areaSize.y * 0.5f);

            // Gives a random position within the rectangle 
            Vector3 candidateXZ = areaCenter + new Vector3(x, 0f, z);

            // Raycast Down
            Vector3 rayStart = candidateXZ + Vector3.up * rayStartHeight;

            // Cast a ray downward to find the terrain surface
            if (!Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, rayStartHeight * 2f, groundMask))
            {
                continue; // If we did not hit ground then try again
            }
                
            // Use the hit point as the spawn position
            Vector3 spawnPos = hit.point + Vector3.up * yOffset;

            // Check spacing
            bool tooClose = false;

            // Compare this position with all previously used positions
            for (int i = 0; i < usedPositions.Count; i++)
            {
                if (Vector3.Distance(usedPositions[i], spawnPos) < minDistanceBetween)
                {
                    tooClose = true;
                    break;
                }
            }
            // If too close to another object then try again
            if (tooClose)
            {
                continue;
            } 

            // Choose a random garbage object from the list
            GameObject prefab = PickedBiasedPrefab();

            // Spawn it at the position
            Instantiate(prefab, spawnPos, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), transform);

            // Save this position so we can check spacing next time
            usedPositions.Add(spawnPos);
            spawned++;
        }

        // Tell TrashProgress class how many items were spawned in
        TrashProgress.SetTotal(spawned);
    }

    // Clear Function
    public void ClearSpawned()
    {
        // Destroy all children under this spawner object making it easy to respawn garbage cleanly
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    private GameObject PickedBiasedPrefab()
    {
        float roll = Random.value;

        // Waste randomizer
        if (roll < wasteChance && wastePrefabs.Count > 0)
        {
            return wastePrefabs[Random.Range(0, wastePrefabs.Count)];
        }
            
        roll -= wasteChance;

        // Plastic randomizer
        if (roll < recyclableChance && recyclablePrefabs.Count > 0)
        {
            return recyclablePrefabs[Random.Range(0, recyclablePrefabs.Count)];
        }
           
        roll -= recyclableChance;

        // Paper randomizer
        if (roll < textileChance && textilePrfabs.Count > 0)
        {
            return textilePrfabs[Random.Range(0, textilePrfabs.Count)];
        }
            
        roll -= textileChance;

        // Electronics randomizer
        if (electronicPrefabs.Count > 0)
        {
            return electronicPrefabs[Random.Range(0, electronicPrefabs.Count)];
        }

        // Safety fallback
        return null;
    }
}
