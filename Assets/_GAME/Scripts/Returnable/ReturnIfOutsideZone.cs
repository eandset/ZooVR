using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ReturnIfOutsideZone : MonoBehaviour
{
    [SerializeField] private Collider zone;
    [SerializeField] private bool lockGrabWhileReturning = true;
    
    private XRBaseInteractable _interactable;
    private IReturnable _returnable;

    private void Awake()
    {
        _returnable = GetComponent<ReturnableObject>();
        _interactable = GetComponent<XRBaseInteractable>();
    }

    private void Update()
    {
        if (!zone.bounds.Contains(transform.position))
            _returnable.ReturnToStart();
    }
    
    private void OnEnable()
    {
        _returnable.OnReturnStarted += OnReturnStarted;
        _returnable.OnReturnCompleted += Unlock;
    }

    private void OnDisable()
    {
        _returnable.OnReturnStarted -= OnReturnStarted;
        _returnable.OnReturnCompleted -= OnReturnCompleted;
        
        Unlock();
    }
    
    private void OnReturnStarted()
    {
        if (!lockGrabWhileReturning) return;
        _interactable.enabled = false;
    }

    private void OnReturnCompleted()
    {
        Unlock();
    }

    private void Unlock()
    {
        _interactable.enabled = true;
    }
}