using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BallController : MonoBehaviour
{
    public float initialSpeed = 5f;
    public float maxSpeed = 10f;
    private Rigidbody2D rb;
    public PhysicsMaterial2D noBounceMaterial;
    private PhysicsMaterial2D originalMaterial;
    private bool isGrounded = false;

    public PlayerController player1;
    public PlayerController player2;
    public Image readyImage;   // New Image component for "Ready"
    public Image startImage;   // New Image component for "Start"
    public Image countdownImage;
    public Sprite[] countdownSprites;
    public float countdownDuration = 3f;

    private PlayerController lastRoundWinner;
    private PlayerController lastPlayerTouched;
    public ScoreManager scoreManager;
    public GameObject guideObject;
    public float guideYPosition = 10f;

    public float gravityIncreaseRate = 0.5f;
    private float initialGravityScale;
    public BuffManager buffManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalMaterial = GetComponent<Collider2D>().sharedMaterial;
        initialGravityScale = rb.gravityScale;

        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;

        if (guideObject != null)
        {
            guideObject.SetActive(false);
        }

        StartCoroutine(StartFirstRound());
    }

    IEnumerator StartFirstRound()
    {
        buffManager.StopSpawningBuffs();

        transform.position = new Vector3(0.19f, 4f, 0);

        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;

        // Show the "Ready" image
        readyImage.enabled = true;
        startImage.enabled = false;
        countdownImage.enabled = false;
        yield return new WaitForSeconds(1f);
        readyImage.enabled = false;

        // Countdown "3", "2", "1"
        countdownImage.enabled = true;
        for (int i = (int)countdownDuration; i > 0; i--)
        {
            countdownImage.sprite = countdownSprites[i - 1]; // "3", "2", "1"
            countdownImage.enabled = true;
            yield return new WaitForSeconds(1f);
        }

        countdownImage.enabled = false;

        // Show the "Start" image
        startImage.enabled = true;
        yield return new WaitForSeconds(1f);
        startImage.enabled = false;

        LaunchBall();

        buffManager.StartSpawningBuffs();
    }

    void LaunchBall()
    {
        Vector2 launchDirection = new Vector2(Random.Range(-1f, 1f), 1).normalized;
        rb.velocity = launchDirection * initialSpeed;
        rb.gravityScale = initialGravityScale;
    }

    IEnumerator StartNewRound()
    {
        player1.ResetBuffs();
        player2.ResetBuffs();

        buffManager.StopSpawningBuffs();

        Vector3 spawnPosition = lastRoundWinner.transform.position + Vector3.up * 7f;
        transform.position = spawnPosition;

        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;

        // Show the "Ready" image
        readyImage.enabled = true;
        startImage.enabled = false;
        countdownImage.enabled = false;
        yield return new WaitForSeconds(1f);
        readyImage.enabled = false;

        // Countdown "3", "2", "1"
        countdownImage.enabled = true;
        for (int i = (int)countdownDuration; i > 0; i--)
        {
            countdownImage.sprite = countdownSprites[i - 1]; // "3", "2", "1"
            countdownImage.enabled = true;
            yield return new WaitForSeconds(1f);
        }

        countdownImage.enabled = false;

        // Show the "Start" image
        startImage.enabled = true;
        yield return new WaitForSeconds(1f);
        startImage.enabled = false;

        rb.gravityScale = initialGravityScale;
        buffManager.StartSpawningBuffs();
        isGrounded = false;
    }

    void ResetPlayerPositions()
    {
        // Set player positions to their initial positions (if needed)
        player1.transform.position = new Vector3(-7f, player1.transform.position.y, player1.transform.position.z);
        player2.transform.position = new Vector3(7.19f, player2.transform.position.y, player2.transform.position.z);
    }

    void Update()
    {
        float yThreshold = 9.0f;

        // Update the position of the guide object to have the same X coordinate as the ball
        if (guideObject != null)
        {
            Vector3 guidePosition = guideObject.transform.position;
            guidePosition.x = transform.position.x; // Match X coordinate
            guidePosition.y = guideYPosition; // Fixed Y coordinate
            guideObject.transform.position = guidePosition;

            // Check if the ball is above the Y threshold
            bool isAboveThreshold = transform.position.y > yThreshold;

            // Show or hide the guide object based on the ball's Y position
            guideObject.SetActive(isAboveThreshold);
        }

        // Rotate the ball based on its velocity
        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // Apply gradual increase in gravity to simulate falling faster
        if (rb.gravityScale > 0)
        {
            rb.gravityScale += gravityIncreaseRate * Time.deltaTime; // Increase gravity scale over time
        }

        // Limit the ball's speed
        LimitBallSpeed();
    }

    void LimitBallSpeed()
    {
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (!isGrounded)
            {
                // Stop bouncing by changing the material to a no bounce material
                GetComponent<Collider2D>().sharedMaterial = noBounceMaterial;
                rb.velocity = Vector2.zero; // Stop any residual velocity
                isGrounded = true; // Set isGrounded to true when grounded

                // Determine the winner of the round
                if (transform.position.x < 0)
                {
                    scoreManager.IncreaseScore(player2);
                    lastRoundWinner = player2;
                }
                else
                {
                    scoreManager.IncreaseScore(player1);
                    lastRoundWinner = player1;
                }

                // Reset the ball material
                GetComponent<Collider2D>().sharedMaterial = originalMaterial;

                // Disable gravity and stop the ball to start a new round
                rb.gravityScale = 0;
                rb.velocity = Vector2.zero;

                // Reset player positions
                ResetPlayerPositions();

                // Start a new round with countdown
                StartCoroutine(StartNewRound());
            }
        }
        else if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Wall"))
        {
            isGrounded = false; // Reset isGrounded to false when colliding with player or wall

            if (collision.gameObject.CompareTag("Player"))
            {
                lastPlayerTouched = collision.gameObject.GetComponent<PlayerController>(); // Store the player who last touched the ball
            }

            // Reset gravity scale to initial value on collision with player or wall
            rb.gravityScale = initialGravityScale;
        }
    }

    public PlayerController GetLastPlayerTouched()
    {
        return lastPlayerTouched;
    }
}
