using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Collider))]
public class DisableInteractableTrigger : MonoBehaviour
{
    [SerializeField] private XRBaseInteractable interactable;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ForceDropAndDisable();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactable.enabled = true;
        }
    }
    
    private void ForceDropAndDisable()
    {
        if (!interactable) return;
        
        if (interactable.isSelected && interactable is IXRSelectInteractable selectInteractable)
        {
            interactable.interactionManager.CancelInteractableSelection(selectInteractable);
        }
        
        interactable.enabled = false;
    }

}
