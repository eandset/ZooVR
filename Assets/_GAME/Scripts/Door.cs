using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Door : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private XRGrabInteractable interactable;

    private void Awake()
    {
        rigidbody.isKinematic = true;
        interactable.enabled = false;
    }

    public void EnableInteract()
    {
        rigidbody.isKinematic = false;
        interactable.enabled = true;
    }
}
