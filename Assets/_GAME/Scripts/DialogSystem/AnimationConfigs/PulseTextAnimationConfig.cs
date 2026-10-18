using DG.Tweening;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "PulseTextAnimationConfig", menuName = "DialogSystem/Animations/Pulse Process")]
public class PulseTextAnimationConfig : ProcessTextAnimationConfig
{
    [SerializeField] private float pulseDuration = 0.5f;
    [SerializeField] private float targetValue = 0.5f;
    
    public override Tween Animate(TMP_Text text)
    {
        text.DOKill();
        text.alpha = 1f;

        return text.DOFade(targetValue, pulseDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public override void ResetAnimation(TMP_Text text)
    {
        text.alpha = 1f;
    }
}