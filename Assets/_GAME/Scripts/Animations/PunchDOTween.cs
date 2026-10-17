using UnityEngine;
using DG.Tweening;

public class PunchDOTween : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Ease easeType = Ease.OutQuad;

    private void Awake()
    {
        transform.DOLocalMove(transform.position + offset, duration)
            .SetEase(easeType)
            .SetLoops(-1, LoopType.Yoyo)
            .Play();
    }
}