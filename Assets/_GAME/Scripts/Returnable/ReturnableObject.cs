using System;
using UnityEngine;
using DG.Tweening;

public class ReturnableObject : MonoBehaviour, IReturnable
{
    [SerializeField] private float duration = 0.5f;

    public event Action OnReturnStarted;
    public event Action OnReturnCompleted;

    private Vector3 _originalPosition;
    private Quaternion _originalRotation;
    private Tween _returnTween;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _originalPosition = transform.position;
        _originalRotation = transform.rotation;
    }

    public void ReturnToStart()
    {
        _returnTween?.Kill();

        if (duration > 0f)
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
        var wasKinematic = _rigidbody && _rigidbody.isKinematic;
        if (_rigidbody)
        {
            if (!_rigidbody.isKinematic)
            {
                _rigidbody.linearVelocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            }

            _rigidbody.isKinematic = true;
        }

        var returnSequence = DOTween.Sequence();

        returnSequence.Join(transform.DOMove(_originalPosition, duration).SetEase(Ease.OutQuad));
        returnSequence.Join(transform.DORotateQuaternion(_originalRotation, duration).SetEase(Ease.OutQuad));

        returnSequence.OnComplete(() =>
        {
            if (_rigidbody)
            {
                _rigidbody.isKinematic = wasKinematic;
            }

            OnReturnCompleted?.Invoke();
        });

        _returnTween = returnSequence;
        _returnTween.Play();
    }
}