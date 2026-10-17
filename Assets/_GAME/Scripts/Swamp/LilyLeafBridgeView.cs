using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(LilyLeafBridge))]
public class LilyLeafBridgeView : MonoBehaviour
{
    [SerializeField] private float returnDuration = 0.5f;

    private LilyLeafBridge _bridgeLogic;

    private Vector3 _originalPosition;
    private Quaternion _originalRotation;

    private Tween _returnTween;

    private void Awake()
    {
        _bridgeLogic = GetComponent<LilyLeafBridge>();

        _originalPosition = transform.position;
        _originalRotation = transform.rotation;
    }

    private void OnEnable()
    {
        _bridgeLogic.OnResetTriggered += ReturnPosition;
    }

    private void OnDisable()
    {
        _bridgeLogic.OnResetTriggered -= ReturnPosition;
    }

    private void ReturnPosition()
    {
        _returnTween?.Kill();

        if (returnDuration > 0f)
        {
            var returnSequence = DOTween.Sequence();

            returnSequence.Join(transform.DOMove(_originalPosition, returnDuration).SetEase(Ease.OutQuad));
            returnSequence.Join(transform.DORotateQuaternion(_originalRotation, returnDuration).SetEase(Ease.OutQuad));

            _returnTween = returnSequence;
            _returnTween.Play();
        }
        else
        {
            transform.position = _originalPosition;
            transform.rotation = _originalRotation;
        }
    }
}