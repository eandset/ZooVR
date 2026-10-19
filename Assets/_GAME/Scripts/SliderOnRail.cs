using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SliderOnRail : XRSimpleInteractable
{
    [SerializeField] bool moveX = true;
    [SerializeField] bool moveY;
    [SerializeField] bool moveZ;
    
    [SerializeField] Vector3 min = new(-1f, -1f, -1f);
    [SerializeField] Vector3 max = new(1f, 1f, 1f);

    IXRSelectInteractor hand;
    Vector3 grabOffset;

    // Мировая точка -> координаты родителя (или мировые, если родителя нет)
    Vector3 ToParentSpace(Vector3 worldPoint)
    {
        Transform parent = transform.parent;
        return parent != null ? parent.InverseTransformPoint(worldPoint) : worldPoint;
    }

    Vector3 GetHandPosition()
    {
        return ToParentSpace(hand.GetAttachTransform(this).position);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        hand = args.interactorObject;
        grabOffset = transform.localPosition - GetHandPosition();
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        hand = null;
    }

    void Update()
    {
        if (hand == null) return;

        Vector3 target = GetHandPosition() + grabOffset;
        Vector3 p = transform.localPosition;

        if (moveX) p.x = Mathf.Clamp(target.x, min.x, max.x);
        if (moveY) p.y = Mathf.Clamp(target.y, min.y, max.y);
        if (moveZ) p.z = Mathf.Clamp(target.z, min.z, max.z);

        transform.localPosition = p;
    }

    void OnValidate()
    {
        max = Vector3.Max(min, max);
    }
}