using UnityEngine;
using UnityEngine.Events;

public class PlayerTrigger : MonoBehaviour
{
    public UnityEvent<Collider2D> ContactEnter, ContactExit;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(nameof(Player)))
        {
            ContactEnter.Invoke(other);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(nameof(Player)))
        {
            ContactExit.Invoke(other);
        }
    }
}
