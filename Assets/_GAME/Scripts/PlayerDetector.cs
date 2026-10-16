using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class PlayerDetector : MonoBehaviour
{
    public UnityEvent<GameObject> OnPlayerTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerTriggered?.Invoke(gameObject);
        }
    }
}