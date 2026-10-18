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
        foreach (var failTrigger in dangerTriggers)
            failTrigger.OnPlayerEntered.AddListener(OnPlayerFailed);
    }

    private void OnDisable()
    {
        foreach (var failTrigger in dangerTriggers)
            failTrigger.OnPlayerEntered.RemoveListener(OnPlayerFailed);
    }

    private void OnPlayerFailed(GameObject source)
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