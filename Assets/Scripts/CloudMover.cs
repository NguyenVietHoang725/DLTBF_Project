using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMover : MonoBehaviour
{
    public float speed; // Speed at which the cloud moves
    public float resetPositionX = -10.0f; // Position to reset the cloud to when it goes off screen
    public float startPositionX = 10.0f; // Starting position of the cloud

    private CloudManager cloudManager;

    void Start()
    {
        cloudManager = FindObjectOfType<CloudManager>(); // Find the CloudManager in the scene
    }

    void Update()
    {
        // Move the cloud from left to right
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        // Check if the cloud has gone off screen
        if (transform.position.x > startPositionX)
        {
            // Notify CloudManager and destroy this cloud
            cloudManager.OnCloudReset();
            Destroy(gameObject);
        }
    }
}
