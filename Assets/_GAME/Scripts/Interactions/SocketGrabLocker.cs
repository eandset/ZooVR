using System;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Zenject;

public class SocketGrabLocker : IInitializable, IDisposable
{
    private readonly XRGrabInteractable _grabInteractable;
    private readonly XRSocketInteractor _socketToDisable;

    public SocketGrabLocker(XRGrabInteractable grabInteractable, XRSocketInteractor socketToDisable)
    {
        _grabInteractable = grabInteractable;
        _socketToDisable = socketToDisable;
    }

    public void Initialize()
    {
        _grabInteractable.selectEntered.AddListener(OnSelectEntered);
        _grabInteractable.selectExited.AddListener(OnSelectExited);
    }

    public void Dispose()
    {
        _grabInteractable.selectEntered.RemoveListener(OnSelectEntered);
        _grabInteractable.selectExited.RemoveListener(OnSelectExited);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (args.interactorObject is XRDirectInteractor or XRRayInteractor)
        {
            _socketToDisable.enabled = false;
        }
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        _socketToDisable.enabled = true;
    }
}