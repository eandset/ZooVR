using System;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Zenject;

public class PlayerPresenceGrabBlocker : IInitializable, IDisposable
{
    private readonly PlayerDetector _playerDetector;
    private readonly XRGrabInteractable _grabInteractable;
    private readonly IFailHandler _failHandler;

    public PlayerPresenceGrabBlocker(PlayerDetector playerDetector, XRGrabInteractable grabInteractable, IFailHandler failHandler)
    {
        _playerDetector = playerDetector;
        _grabInteractable = grabInteractable;
        _failHandler = failHandler;
    }

    public void Initialize()
    {
        _grabInteractable.selectEntered.AddListener(OnGrabStarted);
    }

    public void Dispose()
    {
        _grabInteractable.selectEntered.RemoveListener(OnGrabStarted);
    }

    private void OnGrabStarted(SelectEnterEventArgs args)
    {
        if (_playerDetector.IsPlayerStay)
        {
            CancelGrab(args);
            _failHandler?.HandleFail();
        }
    }

    private void CancelGrab(SelectEnterEventArgs args)
    {
        if (args.interactorObject is not (XRDirectInteractor or XRRayInteractor))
            return;
        
        if (_grabInteractable is IXRSelectInteractable selectInteractable)
            _grabInteractable.interactionManager.CancelInteractableSelection(selectInteractable);
    }
}