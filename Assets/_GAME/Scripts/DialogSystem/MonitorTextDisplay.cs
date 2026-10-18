using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class MonitorTextDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text screenText;

    private InputTextAnimationConfig _inputConfig;
    private ProcessTextAnimationConfig _processConfig;

    public void SetText(string text)
    {
        screenText.DOKill();

        screenText.alpha = 1f;
        screenText.text = text;
    }

    public IEnumerator SetAnimateText(string text)
    {
        StopProcessAnimation();

        if (_inputConfig)
        {
            yield return _inputConfig.Animate(screenText, text);
        }
        else
        {
            SetText(text);
        }
    }

    public void ResetText()
    {
        screenText.DOKill();

        screenText.text = "";
        screenText.alpha = 1f;
    }

    public void SetInputAnimation(InputTextAnimationConfig config)
    {
        _inputConfig = config;
    }

    public void SetProcessAnimation(ProcessTextAnimationConfig config)
    {
        _processConfig = config;
    }

    public void PlayProcessAnimation()
    {
        if (_processConfig)
        {
            _processConfig.Animate(screenText);
        }
    }

    public void StopProcessAnimation()
    {
        if (_processConfig)
        {
            _processConfig.ResetAnimation(screenText);
        }
    }
}