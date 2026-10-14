using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class TeleportPlayerToPointTrigger : MonoBehaviour
{
    [SerializeField] private TeleportationProvider teleportationProvider;
    [SerializeField] private Transform to;

    private void OnTriggerEnter(Collider other)
    {
        var request = new TeleportRequest
        {
            destinationPosition = to.position,
            destinationRotation = to.rotation,
            matchOrientation = MatchOrientation.TargetUpAndForward
        };

        teleportationProvider.QueueTeleportRequest(request);
    }
}
