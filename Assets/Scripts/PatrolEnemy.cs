using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PatrolEnemy : MonoBehaviour, IDamageable
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float patrolDistance = 3f;
    [SerializeField] private int contactDamage = 1;
    [SerializeField] private int maxHp = 6;
    [SerializeField] private Color aliveColor = new(0.95f, 0.45f, 0.55f);

    private Vector3 startPosition;
    private float direction = 1f;
    private int hp;
    private SpriteRenderer spriteRenderer;

    public int ContactDamage => contactDamage;

    private void Awake()
    {
        hp = maxHp;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = aliveColor;
        }
    }

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        transform.Translate(Vector3.right * (speed * direction * Time.deltaTime));
        if (Mathf.Abs(transform.position.x - startPosition.x) >= patrolDistance)
        {
            direction *= -1f;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = direction < 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController2D>();
        if (player != null)
        {
            player.ApplyDamageFromEnemy(this);
        }
    }

    public void ReceiveDamage(int amount)
    {
        hp -= amount;
        if (hp <= 0)
        {
            Destroy(gameObject);
            return;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.Lerp(Color.white, aliveColor, Mathf.Clamp01((float)hp / maxHp));
        }
    }
}
