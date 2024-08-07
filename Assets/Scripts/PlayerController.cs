using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    public Animator animator;
    bool isFacingRight = true;

    [Header("Movement")]
    public float moveSpeed = 7.0f;
    private float _horizontalMovement;
    private float originalMoveSpeed; // Original move speed for reset

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
    }

    // Start is called before the first frame update
    void Start()
    {
        originalMoveSpeed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        Gravity();
        Flip();

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

    // Coroutine to temporarily increase the player's speed
    public IEnumerator IncreaseSpeed(float duration)
    {
        moveSpeed *= 1.5f; // Increase speed by 50%
        yield return new WaitForSeconds(duration);
        moveSpeed = originalMoveSpeed; // Reset to original speed
    }

    // Coroutine to temporarily increase the player's size
    public IEnumerator IncreaseSize(float duration)
    {
        Vector3 originalScale = GetCurrentScale();
        bool originalFacingRight = isFacingRight;

        transform.localScale = originalScale * 1.5f; // Increase size by 50%
        yield return new WaitForSeconds(duration);

        transform.localScale = originalScale; // Reset to original size

        isFacingRight = originalFacingRight; 
        Vector3 ls = transform.localScale;
        ls.x = isFacingRight ? Mathf.Abs(ls.x) : -Mathf.Abs(ls.x); 
        transform.localScale = ls;
    }
}
