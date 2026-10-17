using UnityEngine;
using DG.Tweening;

public class LilyWobble : MonoBehaviour, IFailHandler
{
    [SerializeField] private Transform target;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float strength = 5f; // Сила/угол вибрации
    [SerializeField] private int vibrato = 10;     // Частота (количество колебаний)

    private Quaternion _baseLocalRotation;
    private Tween _wobbleTween;

    private void Awake()
    {
        if (target == null)
            target = transform;
        
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
            .OnComplete(() => {
                target.localRotation = _baseLocalRotation;
            });
        
        _wobbleTween.Play();
    }
}