using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[CreateAssetMenu(fileName = "KeyDoorFilter", menuName = "Filters/Key Door")]
public class KeyDoorFilter : ScriptableObject, IXRSelectFilter, IXRHoverFilter
{
    public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
    {
        return IsCompare(interactor, interactable);
    }

    public bool Process(IXRHoverInteractor interactor, IXRHoverInteractable interactable)
    {
        return IsCompare(interactor, interactable);
    }

    private bool IsCompare(IXRInteractor interactor,  IXRInteractable interactable)
    {
        if (interactor.transform.TryGetComponent<Door>(out var door))
            return door.Key == interactable.transform.gameObject;
        
        return false;
    }

    public bool canProcess => true;
}