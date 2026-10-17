using System;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(InteractableSocketDropDetector))]
public class LilyLeafBridgeView : MonoBehaviour, IReturnable
{
    [SerializeField] private InteractableSocketDropDetector dropDetector;
    [SerializeField] private float returnDuration = 0.5f;

    public event Action OnReturnStarted;
    public event Action OnReturnCompleted;

    private Vector3 _originalPosition;
    private Quaternion _originalRotation;
    private Tween _returnTween;

    private void Awake()
    {
        _originalPosition = transform.position;
        _originalRotation = transform.rotation;
    }

    private void OnEnable()
    {
        dropDetector.OnDroppedOutsideSocket += ReturnPosition;
    }

    private void OnDisable()
    {
        dropDetector.OnDroppedOutsideSocket -= ReturnPosition;
    }

    private void ReturnPosition()
    {
        _returnTween?.Kill();

        if (returnDuration > 0f)
        {
            OnReturnStarted?.Invoke();
            AnimateReturn();
        }
        else
        {
            transform.position = _originalPosition;
            transform.rotation = _originalRotation;
        }
    }

    private void AnimateReturn()
    {
        var returnSequence = DOTween.Sequence();
        
        returnSequence.Join(transform.DOMove(_originalPosition, returnDuration).SetEase(Ease.OutQuad));
        returnSequence.Join(transform.DORotateQuaternion(_originalRotation, returnDuration).SetEase(Ease.OutQuad));
        
        returnSequence.OnComplete(() => OnReturnCompleted?.Invoke());

        _returnTween = returnSequence;
        _returnTween.Play();
    }
}