using System;
using System.Linq;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Zenject;

public class InteractableSocketDropDetector : IInitializable, IDisposable
{
    public event Action OnDroppedOutside;
    public event Action OnDroppedInside;

    private readonly XRGrabInteractable _grabInteractable;

    public InteractableSocketDropDetector(XRGrabInteractable grabInteractable)
    {
        _grabInteractable = grabInteractable;
    }

    public void Initialize()
    {
        _grabInteractable.selectExited.AddListener(OnSelectExited);
    }

    public void Dispose()
    {
        _grabInteractable.selectExited.RemoveListener(OnSelectExited);
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        if (IsSocketInteract(args))
            OnDroppedInside?.Invoke();
        else
            OnDroppedOutside?.Invoke();
    }

    private bool IsSocketInteract(SelectExitEventArgs args)
    {
        if (args.interactorObject is XRSocketInteractor)
        {
            return true;
        }

        if (_grabInteractable.isSelected)
        {
            if (_grabInteractable.interactorsSelecting.OfType<XRSocketInteractor>().Any())
            {
                return true;
            }
        }

        return IsInAnySocket();
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