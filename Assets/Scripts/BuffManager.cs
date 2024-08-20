using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    public GameObject[] buffPrefabs; // Array of buff prefabs
    public float minX = -7f; // Minimum X position for spawning buffs
    public float maxX = 7f; // Maximum X position for spawning buffs
    public float spawnY = 5f; // Fixed Y position for spawning buffs
    public float minSpawnInterval = 5f; // Minimum interval between spawns
    public float maxSpawnInterval = 15f; // Maximum interval between spawns
    public float buffDuration = 5f; // Duration each buff stays active

    private GameObject currentBuff; // Track the currently active buff
    private bool isRoundActive = true; // Track if a round is active
    private bool isGameOver = false;

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

            // Only spawn buffs if the round is active
            if (isRoundActive && !isGameOver)
            {
                // Destroy the current buff if it exists
                if (currentBuff != null)
                {
                    Destroy(currentBuff);
                }

                // Select a random buff prefab
                GameObject buffPrefab = buffPrefabs[Random.Range(0, buffPrefabs.Length)];

                // Generate a random X position within the specified range
                float randomX = Random.Range(minX, maxX);
                Vector3 spawnPosition = new Vector3(randomX, spawnY, 0);

                // Spawn the buff at the random position
                currentBuff = Instantiate(buffPrefab, spawnPosition, Quaternion.identity);

                // Destroy the buff after the specified duration
                StartCoroutine(DestroyBuffAfterDuration(currentBuff));
            }
        }
    }

    IEnumerator DestroyBuffAfterDuration(GameObject buff)
    {
        yield return new WaitForSeconds(buffDuration);
        if (buff != null)
        {
            Destroy(buff);
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

    public void EndGame()
    {
        isGameOver = true;
        StopSpawningBuffs();
    }
}
