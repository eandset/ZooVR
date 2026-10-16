using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Collider))]
public class ResetInteractionTrigger : MonoBehaviour
{
    [SerializeField] private XRGrabInteractable grabInteractable;

    private bool _isPlayerInside;

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrabStarted);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabStarted);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInside = false;
        }
    }

    private void OnGrabStarted(SelectEnterEventArgs args)
    {
        if (_isPlayerInside)
        {
            if (grabInteractable is IXRSelectInteractable selectInteractable)
            {
                grabInteractable.interactionManager.CancelInteractableSelection(selectInteractable);
            }
            
            if (grabInteractable.TryGetComponent<IFailHandler>(out var failHandler))
            {
                failHandler.HandleFail();
            }
        }
    }
}