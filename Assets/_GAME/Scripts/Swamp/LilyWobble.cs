using System;
using UnityEngine;
using DG.Tweening;

[Serializable]
public class LilyWobble : IFailHandler, IFailInitialize
{
    [SerializeField] private Transform target;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float strength = 5f;
    [SerializeField] private int vibrato = 10;

    private Quaternion _baseLocalRotation;
    private Tween _wobbleTween;

    public void Initialize()
    {
        _baseLocalRotation = target.localRotation;
    }
    
    public void HandleFail()
    {
        if (_wobbleTween != null && _wobbleTween.IsActive())
        {
            _wobbleTween.Kill();
            target.localRotation = _baseLocalRotation;
        }

        _wobbleTween = target.DOShakeRotation(duration, new Vector3(strength, 0f, strength), vibrato, 90f, false)
            .SetAutoKill(false)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => { target.localRotation = _baseLocalRotation; });

        _wobbleTween.Play();
    }
}