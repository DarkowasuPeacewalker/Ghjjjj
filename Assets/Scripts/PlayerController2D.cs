using System.Collections;
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
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private GameObject hitEffectPrefab;

    [Header("Attack stats")]
    [SerializeField] private float weakRange = 1.2f;
    [SerializeField] private int weakDamage = 1;
    [SerializeField] private float weakCooldown = 0.2f;
    [SerializeField] private float strongRange = 1.8f;
    [SerializeField] private int strongDamage = 3;
    [SerializeField] private float strongCooldown = 0.9f;
    [SerializeField] private int rangedDamage = 2;
    [SerializeField] private float rangedCooldown = 0.6f;

    private Rigidbody2D rb;
    private int jumpsLeft;
    private int dashCharges;
    private bool controlsEnabled = true;
    private float invulnerabilityTimer;
    private float facing = 1f;

    private float weakTimer;
    private float strongTimer;
    private float rangedTimer;

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

        TickCooldowns();
        HandleMovement();
        HandleAttacks();

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
        Vector2 knockback = new(enemy.transform.position.x > transform.position.x ? -6f : 6f, 6f);
        rb.velocity = knockback;

        GameManager.Instance.DamagePlayer(enemy.ContactDamage, $"Враг нанес {enemy.ContactDamage} урона!");
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

    private void HandleMovement()
    {
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
            float direction = Mathf.Approximately(move, 0f) ? facing : Mathf.Sign(move);
            rb.velocity = new Vector2(direction * dashForce, rb.velocity.y);
            dashCharges--;
        }

        if (!Mathf.Approximately(move, 0f))
        {
            facing = Mathf.Sign(move);
            transform.localScale = new Vector3(facing, 1f, 1f);
        }
    }

    private void HandleAttacks()
    {
        if (Input.GetMouseButtonDown(0) && weakTimer <= 0f)
        {
            weakTimer = weakCooldown;
            DoMeleeAttack(weakRange, weakDamage, new Color(0.6f, 1f, 0.7f));
            GameManager.Instance.SetMessage("Слабая атака!");
        }

        if (Input.GetMouseButtonDown(1) && strongTimer <= 0f)
        {
            strongTimer = strongCooldown;
            DoMeleeAttack(strongRange, strongDamage, new Color(1f, 0.6f, 0.25f));
            GameManager.Instance.SetMessage("Сильная атака!");
        }

        if (Input.GetKeyDown(KeyCode.Q) && rangedTimer <= 0f)
        {
            rangedTimer = rangedCooldown;
            FireProjectile();
            GameManager.Instance.SetMessage("Дальняя атака!");
        }
    }

    private void DoMeleeAttack(float range, int damage, Color color)
    {
        Vector2 origin = (attackPoint != null ? (Vector2)attackPoint.position : (Vector2)transform.position) + Vector2.right * facing * (range * 0.5f);
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, range, enemyMask);

        foreach (Collider2D hit in hits)
        {
            IDamageable target = hit.GetComponent<IDamageable>();
            if (target != null)
            {
                target.ReceiveDamage(damage);
            }
        }

        SpawnHitEffect(origin, range, color);
    }

    private void FireProjectile()
    {
        if (projectilePrefab == null)
        {
            return;
        }

        Vector3 spawn = attackPoint != null ? attackPoint.position : transform.position;
        Projectile projectile = Instantiate(projectilePrefab, spawn + Vector3.right * facing * 0.5f, Quaternion.identity);
        projectile.Launch(new Vector2(facing, 0f), rangedDamage);
    }

    private void SpawnHitEffect(Vector2 position, float size, Color color)
    {
        if (hitEffectPrefab == null)
        {
            return;
        }

        GameObject effect = Instantiate(hitEffectPrefab, position, Quaternion.identity);
        effect.transform.localScale = new Vector3(size, size, 1f);

        SpriteRenderer sprite = effect.GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            sprite.color = color;
        }

        StartCoroutine(DestroyAfter(effect, 0.12f));
    }

    private IEnumerator DestroyAfter(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (target != null)
        {
            Destroy(target);
        }
    }

    private void TickCooldowns()
    {
        weakTimer -= Time.deltaTime;
        strongTimer -= Time.deltaTime;
        rangedTimer -= Time.deltaTime;
    }

    private bool IsGrounded()
    {
        if (groundCheck == null)
        {
            return false;
        }

        return Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundMask) != null;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(attackPoint.position + Vector3.right * weakRange * 0.5f, weakRange);
        Gizmos.color = new Color(1f, 0.6f, 0.2f);
        Gizmos.DrawWireSphere(attackPoint.position + Vector3.right * strongRange * 0.5f, strongRange);
    }
}
