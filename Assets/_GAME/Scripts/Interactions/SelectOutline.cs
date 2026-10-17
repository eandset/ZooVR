using Plugins.QuickOutline.Scripts;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SelectOutline : MonoBehaviour
{
    [SerializeField] private Outline outline;
    [SerializeField] private XRGrabInteractable grabInteractable;

    private void Update()
    {
        outline.enabled = grabInteractable.isHovered;
    }
}
