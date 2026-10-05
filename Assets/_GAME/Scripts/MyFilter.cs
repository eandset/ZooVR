using UnityEngine;using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[CreateAssetMenu(fileName = "MyFilter", menuName = "My Tools/My Filter")]
public class MyFilter : ScriptableObject, IXRSelectFilter, IXRHoverFilter
{
    public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
    {
        return interactable.transform.tag.Contains("Cube");
    }

    public bool Process(IXRHoverInteractor interactor, IXRHoverInteractable interactable)
    {
        return interactable.transform.tag.Contains("Cube");
    }

    public bool canProcess => true;
}
