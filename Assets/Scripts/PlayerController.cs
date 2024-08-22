using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    public Animator animator;
    public Sprite[] playerSprites;
    public RuntimeAnimatorController[] animControllers;
    public Sprite[] victorySprites;
    bool isFacingRight = true;

    [Header("Movement")]
    public float moveSpeed = 7.0f;
    private float _horizontalMovement;
    private float originalMoveSpeed;

    [Header("Jumping")]
    public float jumpPower = 10.0f;

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;
    public bool isGrounded;

    [Header("Gravity")]
    public float baseGravity = 2.0f;
    public float maxFallSpeed = 18.0f;
    public float fallSpeedMultiplier = 2.0f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip jumpSound;
    public AudioClip freezeBuffSound;
    public AudioClip bigBuffSound;
    public AudioClip tinyBuffSound;
    public AudioClip speedBuffSound;

    private Vector3 originalScale;

    public bool isRoundStarting = false;
    private bool canJump = true;
    private bool controlsEnabled = true;

    void Awake()
    {
        if (_rb == null)
        {
            _rb = GetComponent<Rigidbody2D>();
            if (_rb == null)
            {
                Debug.LogError("Rigidbody2D component not found on " + gameObject.name);
            }
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Animator component not found on " + gameObject.name);
            }
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }

    void Start()
    {
        originalMoveSpeed = moveSpeed;
        originalScale = transform.localScale;
        InitializePlayer();
    }

    void Update()
    {
        if (!controlsEnabled) return;
        GroundCheck();
        Gravity();
        Flip();

        if (isRoundStarting)
        {
            _horizontalMovement = 0;
            return;
        }

        _rb.velocity = new Vector2(_horizontalMovement * moveSpeed, _rb.velocity.y);

        animator.SetFloat("yVelocity", _rb.velocity.y);
        animator.SetFloat("magnitude", _rb.velocity.magnitude);
        animator.SetBool("isGrounded", isGrounded);
    }

    private void Gravity()
    {
        if (_rb.velocity.y < 0)
        {
            _rb.gravityScale = baseGravity * fallSpeedMultiplier;
            _rb.velocity = new Vector2(_rb.velocity.x, Mathf.Max(_rb.velocity.y, -maxFallSpeed));
        }
        else
        {
            _rb.gravityScale = baseGravity;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        _horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded && context.performed && canJump)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, jumpPower);
            animator.SetTrigger("jump");

            if (jumpSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(jumpSound);
            }
        }
    }

    private void GroundCheck()
    {
        isGrounded = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer);
    }

    private void Flip()
    {
        if (isFacingRight && _horizontalMovement < 0 || !isFacingRight && _horizontalMovement > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    private Vector3 GetCurrentScale()
    {
        Vector3 currentScale = transform.localScale;
        currentScale.x = isFacingRight ? Mathf.Abs(currentScale.x) : -Mathf.Abs(currentScale.x);
        return currentScale;
    }

    public void ResetBuffs()
    {
        moveSpeed = originalMoveSpeed;
        transform.localScale = originalScale;
        canJump = true;

        Vector3 ls = transform.localScale;
        ls.x = isFacingRight ? Mathf.Abs(ls.x) : -Mathf.Abs(ls.x);
        transform.localScale = ls;
    }

    public IEnumerator IncreaseSpeed(float duration)
    {
        if (speedBuffSound != null)
        {
            audioSource.PlayOneShot(speedBuffSound);
        }
        moveSpeed *= 1.5f;
        yield return new WaitForSeconds(duration);
        moveSpeed = originalMoveSpeed;
    }

    public IEnumerator IncreaseSize(float duration)
    {
        if (bigBuffSound != null)
        {
            audioSource.PlayOneShot(bigBuffSound);
        }
        Vector3 originalFacingScale = GetCurrentScale();

        transform.localScale = originalFacingScale * 1.5f;
        yield return new WaitForSeconds(duration);

        transform.localScale = originalFacingScale;

        Vector3 ls = transform.localScale;
        ls.x = isFacingRight ? Mathf.Abs(ls.x) : -Mathf.Abs(ls.x);
        transform.localScale = ls;
    }

    public IEnumerator DecreaseSize(float duration)
    {
        if (tinyBuffSound != null)
        {
            audioSource.PlayOneShot(tinyBuffSound);
        }
        Vector3 originalFacingScale = GetCurrentScale();

        transform.localScale = originalFacingScale * 0.5f;
        yield return new WaitForSeconds(duration);

        transform.localScale = originalFacingScale;

        Vector3 ls = transform.localScale;
        ls.x = isFacingRight ? Mathf.Abs(ls.x) : -Mathf.Abs(ls.x);
        transform.localScale = ls;
    }

    public IEnumerator Freeze(float duration)
    {
        if (freezeBuffSound != null)
        {
            audioSource.PlayOneShot(freezeBuffSound);
        }

        float originalMoveSpeed = moveSpeed; // Store the original movement speed
        moveSpeed = 0; // Set the move speed to 0 to freeze the player
        _horizontalMovement = 0; // Stop any movement input
        canJump = false;

        // Change the sprite color to blue
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color originalColor = spriteRenderer.color; // Store the original color
        spriteRenderer.color = Color.blue; // Change color to blue

        yield return new WaitForSeconds(duration);

        // Restore the original color
        spriteRenderer.color = originalColor;

        moveSpeed = originalMoveSpeed; // Restore the original move speed
        canJump = true;
    }

    public void InitializePlayer()
    {
        string key = gameObject.name == "Player1" ? "Player1CharacterIndex" : "Player2CharacterIndex";
        int playerIndex = PlayerPrefs.GetInt(key, 0);

        SetCharacterAnimation(playerIndex);
        SetCharacterSprite(playerIndex);
    }

    private void SetCharacterAnimation(int characterIndex)
    {
        if (characterIndex >= 0 && characterIndex < animControllers.Length)
        {
            animator.runtimeAnimatorController = animControllers[characterIndex];
        }
        else
        {
            Debug.LogError("Character index out of range for animControllers.");
        }
    }

    private void SetCharacterSprite(int characterIndex)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && characterIndex >= 0 && characterIndex < playerSprites.Length)
        {
            spriteRenderer.sprite = playerSprites[characterIndex];
        }
        else
        {
            Debug.LogError("Character index out of range for playerSprites.");
        }
    }

    public void DisableControls()
    {
        controlsEnabled = false;
        _rb.velocity = Vector2.zero; // Stop the player's movement
    }

    public void EnableControls()
    {
        controlsEnabled = true;
    }

    public void DisplayVictoryPose()
    {
        // Disable player controls
        DisableControls();

        // Stop any running animations
        animator.enabled = false;

        // Determine the player's index
        string key = gameObject.name == "Player1" ? "Player1CharacterIndex" : "Player2CharacterIndex";
        int playerIndex = PlayerPrefs.GetInt(key, 0);

        // Set the victory sprite
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && playerIndex >= 0 && playerIndex < victorySprites.Length)
        {
            spriteRenderer.sprite = victorySprites[playerIndex];
        }
        else
        {
            Debug.LogError("Character index out of range for victorySprites.");
        }
    }
}
