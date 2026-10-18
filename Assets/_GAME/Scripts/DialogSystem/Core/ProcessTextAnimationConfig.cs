using DG.Tweening;
using TMPro;
using UnityEngine;

public abstract class ProcessTextAnimationConfig : ScriptableObject
{
    public abstract Tween Animate(TMP_Text text);
    public abstract void ResetAnimation(TMP_Text text);
}