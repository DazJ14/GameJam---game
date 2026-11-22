
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CoinPickup2D : MonoBehaviour
{
    [SerializeField] private CoinCounter counter;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;

        if (counter == null)
            counter = FindAnyObjectByType<CoinCounter>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (counter == null)
            return;

        if (other.attachedRigidbody != counter.PlayerBody)
            return;

        counter.AddCoin();
        Destroy(gameObject);
    }
}
