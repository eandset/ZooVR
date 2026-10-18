using System;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Zenject;

public class InteractableSocketDropDetector : IInitializable, IDisposable
{
    public event Action OnDroppedOutside;
    public event Action OnDroppedInside;

    private readonly XRGrabInteractable _grabInteractable;
    private readonly PlayerDetector _playerDetector;

    public InteractableSocketDropDetector(XRGrabInteractable grabInteractable, PlayerDetector playerDetector = null)
    {
        _grabInteractable = grabInteractable;
        _playerDetector = playerDetector;
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
        if (_playerDetector && _playerDetector.IsPlayerStay)
            return; 

        if (_grabInteractable.isSelected)
            return;

        if (IsInAnySocket())
            OnDroppedInside?.Invoke();
        else
            OnDroppedOutside?.Invoke();
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