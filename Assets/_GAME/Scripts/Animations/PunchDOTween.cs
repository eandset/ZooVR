using System.Collections;
using UnityEngine;
using DG.Tweening;

public class PunchDOTween : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Ease easeType = Ease.OutQuad;
    [SerializeField] private bool useAutoPlay;
    
    private Tween _currentTween;

    private IEnumerator Start()
    {
        if (!useAutoPlay)
            yield break;

        while (Application.isPlaying)
        {
            yield return PlayPunch();
        }
    }

    public IEnumerator PlayPunch()
    {
        var sequence = DOTween.Sequence();
        
        _currentTween?.Kill();
        
        var originalPosition = transform.position;
        var targetPosition = transform.position + offset;
        
        sequence.Append(transform.DOLocalMove(targetPosition, duration).SetEase(easeType));
        sequence.Append(transform.DOLocalMove(originalPosition, duration).SetEase(easeType));
        
        _currentTween = sequence;
        
        yield return sequence.Play().WaitForCompletion();
    }
}