using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    public GameObject[] buffPrefabs; // Array of buff prefabs
    public Vector3 spawnPosition; // Fixed position for spawning buffs
    public float minSpawnInterval = 5f; // Minimum interval between spawns
    public float maxSpawnInterval = 15f; // Maximum interval between spawns
    public float buffDuration = 5f; // Duration each buff stays active

    private GameObject currentBuff;
    private bool isRoundActive = true; // Track if a round is active

    void Start()
    {
        StartCoroutine(SpawnBuff());
    }

    IEnumerator SpawnBuff()
    {
        while (true)
        {
            // Wait for a random time interval before spawning the next buff
            float spawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(spawnInterval);

            // Only spawn buff if round is active
            if (isRoundActive)
            {
                // Destroy the current buff if it exists
                if (currentBuff != null)
                {
                    Destroy(currentBuff);
                }

                // Randomly select a buff prefab and spawn it at the fixed position
                GameObject buffPrefab = buffPrefabs[Random.Range(0, buffPrefabs.Length)];
                currentBuff = Instantiate(buffPrefab, spawnPosition, Quaternion.identity);

                // Destroy the buff after the specified duration
                Destroy(currentBuff, buffDuration);
            }
        }
    }

    // Method to disable spawning of buffs
    public void StopSpawningBuffs()
    {
        isRoundActive = false;

        // Destroy the current buff if it exists
        if (currentBuff != null)
        {
            Destroy(currentBuff);
        }
    }

    // Method to enable spawning of buffs
    public void StartSpawningBuffs()
    {
        isRoundActive = true;
    }
}