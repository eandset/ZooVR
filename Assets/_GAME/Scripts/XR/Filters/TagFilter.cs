using UnityEngine;using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[CreateAssetMenu(fileName = "TagFilter", menuName = "Filters/Tag")]
public class TagFilter : ScriptableObject, IXRSelectFilter, IXRHoverFilter
{
    [SerializeField] private string tag;

    public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
    {
        return IsCompare(interactable);
    }

    public bool Process(IXRHoverInteractor interactor, IXRHoverInteractable interactable)
    {
        return IsCompare(interactable);
    }

    private bool IsCompare(IXRInteractable interactable)
    {
        return interactable.transform.CompareTag(tag);
    }

    public bool canProcess => true;
}
