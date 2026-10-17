using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class LockGrabWhenSocketOccupied : MonoBehaviour
{
    [SerializeField] private XRGrabInteractable grabInteractable;
    [SerializeField] private XRSocketInteractor socketInteractor;

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrabAttempt);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabAttempt);
    }

    private void OnGrabAttempt(SelectEnterEventArgs args)
    {
        if (args.interactorObject is not (XRDirectInteractor or XRRayInteractor))
            return;
        
        if (socketInteractor.hasSelection && grabInteractable is IXRSelectInteractable selectInteractable)
        {
            grabInteractable.interactionManager.CancelInteractableSelection(selectInteractable);
        }
    }
}