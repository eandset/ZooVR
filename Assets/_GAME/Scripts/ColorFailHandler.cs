using System;
using UnityEngine;
using DG.Tweening;

[Serializable]
public class ColorFailHandler : IFailHandler, IFailInitialize
{
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    
    [SerializeField] private Renderer targetRenderer;

    [Header("Pulse Settings")]
    [SerializeField] private Color pulseColor = Color.red;
    [SerializeField] private float pulseIntensity = 1f;
    [SerializeField] private float pulseSpeed = 15f;
    [SerializeField] private float duration = 1f;
    [SerializeField] private Ease ease;

    private Material _material;
    private Tween _pulseTween;
    private Color originalEmissionColor;

    public void Initialize()
    {
        _material = targetRenderer.material;

        if (_material.HasProperty(EmissionColor))
        {
            originalEmissionColor = _material.GetColor(EmissionColor);
        }
    }

    public void HandleFail()
    {
        _pulseTween?.Kill();

        var targetColor = pulseColor * pulseIntensity;
        
        var halfCycleDuration = 1f / pulseSpeed;
        var totalLoops = Mathf.Max(1, Mathf.RoundToInt(duration / halfCycleDuration));
        
        _pulseTween = _material.DOColor(targetColor, EmissionColor, halfCycleDuration)
            .SetLoops(totalLoops, LoopType.Yoyo)
            .SetEase(ease)
            .OnComplete(() => 
            {
                _material.SetColor(EmissionColor, originalEmissionColor);
            });
    }
}