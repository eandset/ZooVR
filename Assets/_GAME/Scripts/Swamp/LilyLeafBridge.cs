using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRGrabInteractable))]
public class LilyLeafBridge : MonoBehaviour
{
    public event Action OnResetTriggered;

    private XRGrabInteractable _grabInteractable;
    private Coroutine _checkSocketCoroutine;

    private void Awake()
    {
        _grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        _grabInteractable.selectExited.AddListener(OnSelectExited);
    }

    private void OnDisable()
    {
        _grabInteractable.selectExited.RemoveListener(OnSelectExited);
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        if (_checkSocketCoroutine != null)
        {
            StopCoroutine(_checkSocketCoroutine);
            _checkSocketCoroutine = null;
        }
        
        if (gameObject.activeInHierarchy)
        {
            _checkSocketCoroutine = StartCoroutine(CheckIfInSocketRoutine(args));
        }
    }
    
    private IEnumerator CheckIfInSocketRoutine(SelectExitEventArgs args)
    {
        yield return null;
        
        var isInSocket = args.interactorObject is XRSocketInteractor || _grabInteractable.isSelected;
        
        if (_grabInteractable.interactorsSelecting.Count > 0)
        {
            foreach (var interactor in _grabInteractable.interactorsSelecting)
            {
                if (interactor is XRSocketInteractor)
                {
                    isInSocket = true;
                    break;
                }
            }
        }
        
        if (!isInSocket)
        {
            OnResetTriggered?.Invoke();
        }

        _checkSocketCoroutine = null;
    }
}