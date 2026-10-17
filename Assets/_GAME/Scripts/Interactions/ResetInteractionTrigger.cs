using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ResetInteractionTrigger : MonoBehaviour
{
    [SerializeField] private XRGrabInteractable grabInteractable;
    [SerializeField] private PlayerDetector detector;

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrabStarted);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabStarted);
    }

    private void OnGrabStarted(SelectEnterEventArgs args)
    {
        if (detector.IsPlayerStay)
        {
            if (grabInteractable is IXRSelectInteractable selectInteractable)
            {
                if (args.interactorObject is XRDirectInteractor or XRRayInteractor)
                    grabInteractable.interactionManager.CancelInteractableSelection(selectInteractable);
            }

            if (grabInteractable.TryGetComponent<IFailHandler>(out var failHandler))
            {
                failHandler.HandleFail();
            }
        }
    }
}