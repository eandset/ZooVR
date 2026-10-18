using System;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Zenject;

public class ReturnBlocker : IInitializable, IDisposable
{
    private readonly IReturnable _returnable;
    private readonly XRGrabInteractable _grabInteractable;
    private readonly XRSocketInteractor _socketInteractor;

    public ReturnBlocker(XRGrabInteractable grabInteractable, XRSocketInteractor socketInteractor, IReturnable returnable)
    {
        _grabInteractable = grabInteractable;
        _socketInteractor = socketInteractor;
        _returnable = returnable;
    }

    public void Initialize()
    {
        _returnable.OnReturnStarted += DisableComponents;
        _returnable.OnReturnCompleted += EnableComponents;
    }

    public void Dispose()
    {
        _returnable.OnReturnStarted -= DisableComponents;
        _returnable.OnReturnCompleted -= EnableComponents;
    }

    private void DisableComponents()
    {
        _grabInteractable.enabled = false;
        _socketInteractor.enabled = false;
    }

    private void EnableComponents()
    {
        _grabInteractable.enabled = true;
        _socketInteractor.enabled = true;
    }
}