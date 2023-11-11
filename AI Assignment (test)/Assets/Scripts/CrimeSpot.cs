using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CrimeSpot : MonoBehaviour
{
    // Reference to the road tilemap
    public Tilemap Roads;

    // Crime Spot Prefab
    public GameObject crimeSpot;

    // Time interval between crime spot spawns
    public float spawnInterval;
    private float spawnTime;

    // Reference to previous crime spot and its transform
    private GameObject previousCrimeSpot;
    private Transform previousCrimeSpotTransform;

    private void Start()
    {
        Debug.Log("Start method called");
        ReplaceCrimeSpot();
    }
    private void Update()
    {
        // Update spawn time
        spawnTime += Time.deltaTime;

        // Check if its time to spawn a new crime spot, or if R is pressed
        if (spawnTime >= spawnInterval || Input.GetKeyDown(KeyCode.R))
        {
            // Replace the current crime spot with a new one
            ReplaceCrimeSpot();
            spawnTime = 0.0f;
        }
    }

    // Spawns a new crime spot on a random road tile
    void SpawnCrimeSpot()
    {
        // Get the bounds of the road tilemap
        BoundsInt bounds = Roads.cellBounds;

        // generate a random position within the bounds
        Vector3Int randomPos = new Vector3Int(Random.Range(bounds.x, bounds.x + bounds.size.x),
                                              Random.Range(bounds.y, bounds.y + bounds.size.y), 0);

        // Get the tile at the random position
        TileBase tile = Roads.GetTile(randomPos);

        // Check if it is a valid tile
        if (tile != null)
        {
            // Get the world position of the tile
            Vector3 spawnPosition = Roads.GetCellCenterWorld(randomPos);

            // Instantiate the crime spot at the random position
            previousCrimeSpot = Instantiate(crimeSpot, spawnPosition, Quaternion.identity);
            previousCrimeSpotTransform = previousCrimeSpot.transform;
        }
    }

    // Destroys the previous crime spot and spawns a new one
    void ReplaceCrimeSpot()
    {
        if (previousCrimeSpot != null)
        {
            Destroy(previousCrimeSpot); // Destroy the previous crime location
        }

        SpawnCrimeSpot();
    }

    public Transform GetPreviousCrimeSpotTransform()
    {
        return previousCrimeSpotTransform;
    }
}

