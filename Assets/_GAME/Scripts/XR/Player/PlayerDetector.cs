using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class PlayerDetector : MonoBehaviour
{
    public UnityEvent<GameObject> OnPlayerEntered;
    public UnityEvent<GameObject> OnPlayerExited;

    public bool IsPlayerStay { get; private set; }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerEntered?.Invoke(gameObject);
            IsPlayerStay = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerExited?.Invoke(gameObject);
            IsPlayerStay = false;
        }
    }
}