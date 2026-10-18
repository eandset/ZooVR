using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class VRMonitorScreen : MonoBehaviour
{
    [SerializeField] private MonitorTextDisplay textDisplay;

    public UnityEvent OnScreenActivated;

    private XRSimpleInteractable interactable;
    private Collider monitorCollider;
    
    public MonitorTextDisplay TextDisplay => textDisplay;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        monitorCollider = GetComponent<Collider>();
    }

    private void OnEnable() => interactable.selectEntered.AddListener(HandleInteraction);
    private void OnDisable() => interactable.selectEntered.RemoveListener(HandleInteraction);

    private void HandleInteraction(SelectEnterEventArgs args) => OnScreenActivated?.Invoke();

    public void SetInteractive(bool isInteractive)
    {
        interactable.enabled = isInteractive;
        monitorCollider.enabled = isInteractive;
    }
}