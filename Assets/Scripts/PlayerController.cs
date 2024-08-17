using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    public Animator animator;
    public AudioSource audioSource;
    public AudioClip jumpSound;
    public AudioClip speedBuffSound;
    public AudioClip sizeBuffSound;
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

    private Vector3 originalScale;
    private bool isFrozen = false;

    // New variable to control whether the player is allowed to move
    public bool isRoundStarting = false;

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
                Debug.LogError("AudioSource component not found on " + gameObject.name);
            }
        }
    }

    void Start()
    {
        originalMoveSpeed = moveSpeed;
        originalScale = GetCurrentScale();
        audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

    void Update()
    {
        GroundCheck();
        Gravity();
        Flip();

        if (isRoundStarting)
        {
            _horizontalMovement = 0; // Prevent movement
            return;
        }

        _rb.velocity = new Vector2(_horizontalMovement * moveSpeed, _rb.velocity.y);

        // Clamp y velocity and magnitude to avoid sudden spikes
        float clampedYVelocity = Mathf.Clamp(_rb.velocity.y, -maxFallSpeed, jumpPower);
        float clampedMagnitude = Mathf.Clamp(_rb.velocity.magnitude, 0, moveSpeed);

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
        if (isGrounded && context.performed)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, jumpPower);
            animator.SetTrigger("jump");
            audioSource.PlayOneShot(jumpSound);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
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

        Vector3 ls = transform.localScale;
        ls.x = isFacingRight ? Mathf.Abs(ls.x) : -Mathf.Abs(ls.x);
        transform.localScale = ls;
    }

    public IEnumerator IncreaseSpeed(float duration)
    {
        audioSource.PlayOneShot(speedBuffSound);
        moveSpeed *= 1.5f;
        yield return new WaitForSeconds(duration);
        moveSpeed = originalMoveSpeed;
    }

    public IEnumerator IncreaseSize(float duration)
    {
        audioSource.PlayOneShot(sizeBuffSound);
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
        audioSource.PlayOneShot(sizeBuffSound);
        Vector3 originalFacingScale = GetCurrentScale();

        transform.localScale = originalFacingScale * 0.5f;
        yield return new WaitForSeconds(duration);

        transform.localScale = originalFacingScale;

        Vector3 ls = transform.localScale;
        ls.x = isFacingRight ? Mathf.Abs(ls.x) : -Mathf.Abs(ls.x);
        transform.localScale = ls;
    }

    public void Freeze(bool freeze)
    {
        isFrozen = freeze;

        if (isFrozen)
        {
            _horizontalMovement = 0;
            _rb.velocity = new Vector2(0, 0);
        }
    }
}
