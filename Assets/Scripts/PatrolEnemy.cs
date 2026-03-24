using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PatrolEnemy : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float patrolDistance = 3f;
    [SerializeField] private int damage = 1;

    private Vector3 startPosition;
    private float direction = 1f;

    public int Damage => damage;

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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController2D>();
        if (player != null)
        {
            player.ApplyDamageFromEnemy(this);
        }
    }
}
