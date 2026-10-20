using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class OffsetMoveAnimation : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float duration;
    [SerializeField] private Ease ease;

    public UnityEvent OnOffsetMoveStarted;
    public UnityEvent OnOffestMoveCompleted;
    public UnityEvent OnStartMoveStarted;
    public UnityEvent OnStartMoveCompleted;

    private Tween _tween;
    private Vector3 _startPosition;

    private void Awake()
    {
        _startPosition = transform.position;
    }

    public void MoveOffset()
    {
        _tween?.Kill();

        OnOffsetMoveStarted.Invoke();

        _tween = transform.DOMove(_startPosition + offset, duration)
            .SetEase(ease)
            .OnComplete(() => OnOffestMoveCompleted.Invoke());
    }

    public void MoveToStart()
    {
        _tween?.Kill();

        OnStartMoveStarted.Invoke();

        _tween = transform.DOMove(_startPosition, duration)
            .SetEase(ease)
            .OnComplete(() => OnStartMoveCompleted.Invoke());
    }
}
