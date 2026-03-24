using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private int maxJumps = 2;
    [SerializeField] private int maxDashCharges = 2;

    [Header("Ground")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.15f;
    [SerializeField] private LayerMask groundMask;

    [Header("Combat")]
    [SerializeField] private float invulnerabilityDuration = 0.8f;

    private Rigidbody2D rb;
    private int jumpsLeft;
    private int dashCharges;
    private bool controlsEnabled = true;
    private float invulnerabilityTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        jumpsLeft = maxJumps;
        dashCharges = maxDashCharges;
    }

    private void Update()
    {
        if (!controlsEnabled)
        {
            return;
        }

        bool grounded = IsGrounded();
        if (grounded)
        {
            jumpsLeft = maxJumps;
            dashCharges = maxDashCharges;
        }

        float move = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && jumpsLeft > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpsLeft--;
        }

        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && dashCharges > 0)
        {
            float direction = move;
            if (Mathf.Approximately(direction, 0f))
            {
                direction = transform.localScale.x >= 0 ? 1f : -1f;
            }

            rb.velocity = new Vector2(direction * dashForce, rb.velocity.y);
            dashCharges--;
        }

        if (!Mathf.Approximately(move, 0f))
        {
            transform.localScale = new Vector3(Mathf.Sign(move), 1f, 1f);
        }

        if (invulnerabilityTimer > 0f)
        {
            invulnerabilityTimer -= Time.deltaTime;
        }

        if (transform.position.y < -20f)
        {
            GameManager.Instance.RespawnPlayer();
        }
    }

    public void ApplyDamageFromEnemy(PatrolEnemy enemy)
    {
        if (invulnerabilityTimer > 0f)
        {
            return;
        }

        invulnerabilityTimer = invulnerabilityDuration;
        Vector2 knockback = new Vector2(enemy.transform.position.x > transform.position.x ? -6f : 6f, 6f);
        rb.velocity = knockback;

        GameManager.Instance.DamagePlayer(enemy.Damage, $"Враг нанес {enemy.Damage} урона!");
    }

    public void DisableControl()
    {
        controlsEnabled = false;
        rb.velocity = Vector2.zero;
    }

    public void ResetVelocity()
    {
        rb.velocity = Vector2.zero;
    }

    private bool IsGrounded()
    {
        if (groundCheck == null)
        {
            return false;
        }

        return Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundMask) != null;
    }
}
