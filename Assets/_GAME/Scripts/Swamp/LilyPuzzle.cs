using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class LilyPuzzle : MonoBehaviour
{
    [SerializeField] private List<PlayerDetector> dangerTriggers;

    [Header("Teleportation")]
    [SerializeField] private TeleportationProvider teleportationProvider;
    [SerializeField] private Transform playerSpawnPoint;

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void SubscribeEvents()
    {
        foreach (var failTrigger in dangerTriggers)
            failTrigger.OnPlayerTriggered.AddListener(OnPlayerFailed);
    }

    private void UnsubscribeEvents()
    {
        foreach (var failTrigger in dangerTriggers)
            failTrigger.OnPlayerTriggered.RemoveListener(OnPlayerFailed);
    }

    public void OnPlayerFailed(XRGrabInteractable source)
    {
        OnPlayerFailed(source.gameObject);
    }

    public void OnPlayerFailed(GameObject source)
    {
        TeleportPlayerToStart();

        if (source && source.TryGetComponent<IFailHandler>(out var failHandler))
            failHandler.HandleFail();
    }

    private void TeleportPlayerToStart()
    {
        var request = new TeleportRequest
        {
            destinationPosition = playerSpawnPoint.position,
            destinationRotation = playerSpawnPoint.rotation,
            matchOrientation = MatchOrientation.TargetUpAndForward
        };

        teleportationProvider.QueueTeleportRequest(request);
    }
}