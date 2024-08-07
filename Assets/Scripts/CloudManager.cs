using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour
{
    public GameObject[] cloudPrefabs; // Array of cloud prefabs
    public int cloudCount = 5; // Number of clouds to spawn
    public float spawnMinX = -10.0f; // Minimum X range for spawning clouds
    public float spawnMaxX = 10.0f; // Maximum X range for spawning clouds
    public float spawnMinY = -5.0f; // Minimum Y range for spawning clouds
    public float spawnMaxY = 5.0f; // Maximum Y range for spawning clouds
    public float minSpeed = 1.0f; // Minimum speed of clouds
    public float maxSpeed = 3.0f; // Maximum speed of clouds

    void Start()
    {
        for (int i = 0; i < cloudCount; i++)
        {
            SpawnCloud();
        }
    }

    void SpawnCloud()
    {
        // Determine a random position for the cloud within the specified range
        Vector3 spawnPosition = new Vector3(Random.Range(spawnMinX, spawnMaxX), Random.Range(spawnMinY, spawnMaxY), 0);

        // Randomly select a cloud prefab from the array
        GameObject cloudPrefab = cloudPrefabs[Random.Range(0, cloudPrefabs.Length)];

        // Instantiate the cloud at the random position
        GameObject cloud = Instantiate(cloudPrefab, spawnPosition, Quaternion.identity);

        // Assign a random speed to the cloud
        CloudMover cloudMover = cloud.GetComponent<CloudMover>();
        cloudMover.speed = Random.Range(minSpeed, maxSpeed);
    }

    public void OnCloudReset()
    {
        // Spawn a new cloud when a cloud is reset
        SpawnCloud();
    }
}
