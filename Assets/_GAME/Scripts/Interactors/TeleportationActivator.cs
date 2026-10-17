using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class TeleportationActivator : MonoBehaviour
{
    [SerializeField] private XRRayInteractor teleportInteractor;
    [SerializeField] private XRRayInteractor rayInteractor;
    [SerializeField] private InputActionProperty teleportActivatorAction;

    private void Start()
    {
        teleportInteractor.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (teleportActivatorAction.action.WasPressedThisFrame() && !IsOverUIGameObject())
            teleportInteractor.gameObject.SetActive(true);
        else if (teleportActivatorAction.action.WasReleasedThisFrame())
            teleportInteractor.gameObject.SetActive(false);
    }

    private bool IsOverUIGameObject()
    {
        return rayInteractor && rayInteractor.IsOverUIGameObject();
    }

    private void OnEnable()
    {
        rayInteractor.uiHoverEntered.AddListener(DisableTeleportRay);
    }

    private void OnDisable()
    {
        rayInteractor.uiHoverEntered.RemoveListener(DisableTeleportRay);
    }

    private void DisableTeleportRay(UIHoverEventArgs arg0)
    {
        teleportInteractor.gameObject.SetActive(false);
    }
}