using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "FadeTextAnimationConfig", menuName = "DialogSystem/Animations/Fade Input")]
public class FadeTextAnimationConfig : InputTextAnimationConfig
{
    [SerializeField] private float duration = 0.3f;
    
    public override IEnumerator Animate(TMP_Text text, string message)
    {
        text.DOKill();
        
        text.alpha = 0f;
        text.text = message;
        
        yield return text.DOFade(1f, duration).WaitForCompletion();
    }
}