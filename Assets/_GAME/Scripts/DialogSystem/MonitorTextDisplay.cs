using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class MonitorTextDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text screenText;
    [SerializeField] private float typingSpeed;

    private Tween _flashTween;

    public void SetText(string text)
    {
        screenText.text = text;
    }

    public IEnumerator TypeTextRoutine(string text)
    {
        screenText.text = "";
        foreach (char letter in text)
        {
            screenText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void ResetAnimations()
    {
        _flashTween?.Kill();
        screenText.alpha = 1;
    }
    
    public void AnimateFlash()
    {
        _flashTween?.Kill();

        _flashTween = screenText.DOFade(0.5f, 0.5f)
            .SetLoops(-1, LoopType.Yoyo);
    }
}