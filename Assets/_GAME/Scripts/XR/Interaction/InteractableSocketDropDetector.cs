using System;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRGrabInteractable))]
public class InteractableSocketDropDetector : MonoBehaviour
{
    public event Action OnDroppedOutsideSocket;

    private XRGrabInteractable _grabInteractable;

    private void Awake()
    {
        _grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        _grabInteractable.selectExited.AddListener(OnSelectExited);
    }

    private void OnDisable()
    {
        _grabInteractable.selectExited.RemoveListener(OnSelectExited);
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        if (args.interactorObject is XRSocketInteractor)
        {
            return;
        }
        
        if (_grabInteractable.isSelected)
        {
            if (_grabInteractable.interactorsSelecting.OfType<XRSocketInteractor>().Any())
            {
                return;
            }
        }
        
        if (IsInAnySocket())
        {
            return;
        }
        
        OnDroppedOutsideSocket?.Invoke();
    }

    private bool IsInAnySocket()
    {
        foreach (var interactor in _grabInteractable.interactorsHovering)
        {
            if (interactor is not XRSocketInteractor socket)
                continue;

            if (_grabInteractable is not IXRSelectInteractable selectInteractable)
                continue;
            
            if (socket.CanSelect(selectInteractable))
            {
                return true;
            }
        }

        return false;
    }
}