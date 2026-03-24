using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GemCollectible : MonoBehaviour
{
    private bool collected;

    private void Start()
    {
        GameManager.Instance.RegisterGemTotal();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected)
        {
            return;
        }

        if (other.GetComponent<PlayerController2D>() == null)
        {
            return;
        }

        collected = true;
        GameManager.Instance.RegisterGem();
        Destroy(gameObject);
    }
}
