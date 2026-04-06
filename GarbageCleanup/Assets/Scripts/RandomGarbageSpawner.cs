using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RandomGarbageSpawner : MonoBehaviour
{
    // Prefabs that can spawn on the map
    [Header("Garbage Prefabs By Type")]

    [SerializeField] private List<GameObject> wastePrefabs = new();
    [SerializeField] private List<GameObject> recyclablePrefabs = new();
    [SerializeField] private List<GameObject> textilePrefabs = new();
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

        // Debug numbers for checking spawn randomizer 
        int wasteSpawned = 0;
        int recyclableSpawned = 0;
        int textileSpawned = 0;
        int electronicSpawned = 0;

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

            if (prefab == null)
            {
                continue;
            }

            // Try to get the GarbageController from the prefab
            GarbageController gc = prefab.GetComponent<GarbageController>();

            // If the prefab has garbage data attached to it
            if (gc != null)
            {
                // Check what type of garbage this prefab is
                switch (gc.data.garbageType)
                {
                    case Garbage.Waste:
                        // Increase the count of waste items spawned 
                        wasteSpawned++;
                        break;

                    case Garbage.Recyclable:
                        // Increase count of recyclable items spawned 
                        recyclableSpawned++;
                        break;

                    case Garbage.Textile:
                        // Increase count of textile items spawned
                        textileSpawned++; 
                        break;

                    case Garbage.Electronic:
                        // Increase count of electronics items spawned 
                        electronicSpawned++;
                        break;
                }
            }

            // Randomize Y rotation on objects
            Quaternion randomYRotation = Quaternion.Euler(0f, Random.Range(0f, 300f), 0);
            Quaternion finalRotation = randomYRotation * prefab.transform.rotation;

            // Spawn it at the position
            Instantiate(prefab, spawnPos, finalRotation, transform);

            // Save this position so we can check spacing next time
            usedPositions.Add(spawnPos);
            spawned++;
        }

        // Tell TrashProgress class how many items were spawned in
        TrashProgress.SetTotal(spawned);

        // How many of each spawned
        Debug.Log($"Spawned Results -> Waste: {wasteSpawned}, Recyclable: {recyclableSpawned}, Textile: {textileSpawned}, Electronic: {electronicSpawned}");
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
        // Stores the total chance of all valid catergories
        float totalChance = 0f;

        // Only add chance if that category actually has prefabs assignned
        if (wastePrefabs.Count > 0)
        {
            totalChance += wasteChance;
        }

        if (recyclablePrefabs.Count > 0)
        {
            totalChance += recyclableChance;
        }

        if (textilePrefabs.Count > 0)
        {
            totalChance += textileChance;
        }

        if (electronicPrefabs.Count > 0)
        {
            totalChance += electronicChance;
        }

        // Pick random number between 0 and the total chance 
        float roll = Random.Range(0f, totalChance);

        // Check if the roll falls into the waste category
        if (wastePrefabs.Count > 0)
        {
            if (roll < wasteChance)
            {
                // Return a random waste prefab
                return wastePrefabs[Random.Range(0, wastePrefabs.Count)];
            }

            // If not selected subtract waste chance and move to next category
            roll -= wasteChance;
        }

        // Check recyclable category
        if (recyclablePrefabs.Count > 0)
        {
            if (roll < recyclableChance)
            {
                // Return a random recyclable prefab
                return recyclablePrefabs[Random.Range(0, recyclablePrefabs.Count)];
            }

            // Move to next category
            roll -= recyclableChance;
        }

        // Check textile category
        if (textilePrefabs.Count > 0)
        {
            if (roll < textileChance)
            {
                // Return a random textile prefab
                return textilePrefabs[Random.Range(0, textilePrefabs.Count)];
            }

            // Move to next category
            roll -= textileChance;
        }

        // Check electronic category
        if (electronicPrefabs.Count > 0)
        {
            if (roll < electronicChance)
            {
                // Return a random electronic prefab
                return electronicPrefabs[Random.Range(0, electronicPrefabs.Count)];
            }
        }

        // Fallback incase something goes wrong (it shouldnt)
        return null;
    }
}
