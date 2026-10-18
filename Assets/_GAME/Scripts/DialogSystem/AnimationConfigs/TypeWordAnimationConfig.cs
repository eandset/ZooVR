using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "TypeWordAnimationConfig", menuName = "DialogSystem/Animations/Type Input")]
public class TypeWordAnimationConfig : InputTextAnimationConfig
{
    [SerializeField] private float speed = 0.03f;
    
    public override IEnumerator Animate(TMP_Text text, string message)
    {
        text.DOKill();
        text.alpha = 1f;
        text.text = "";
        
        foreach (char letter in message)
        {
            text.text += letter;
            yield return new WaitForSeconds(speed);
        }
    }
}