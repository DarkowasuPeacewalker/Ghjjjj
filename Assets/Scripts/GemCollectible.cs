using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GemCollectible : MonoBehaviour
{
    private bool collected;

    private void OnValidate()
    {
        Collider2D trigger = GetComponent<Collider2D>();
        if (trigger != null)
        {
            trigger.isTrigger = true;
        }
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterGemTotal();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected)
        {
            return;
        }

        if (!other.TryGetComponent(out PlayerController2D _))
        {
            return;
        }

        collected = true;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterGem();
        }
        Destroy(gameObject);
    }
}
