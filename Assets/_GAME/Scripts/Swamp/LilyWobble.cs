using System.Collections;
using UnityEngine;

public class LilyWobble : MonoBehaviour, IFailHandler
{
    [SerializeField] private Transform target;
    [SerializeField] private float wobbleAngle = 8f;
    [SerializeField] private float wobbleFrequency = 12f;
    [SerializeField] private float decayRate = 3f;
    [SerializeField] private float duration = 1.5f;

    private Quaternion baseLocalRotation;
    private Coroutine wobbleCoroutine;

    private void Awake()
    {
        baseLocalRotation = target.localRotation;
    }
    
    public void HandleFail()
    {
        if (wobbleCoroutine != null)
        {
            StopCoroutine(wobbleCoroutine);
        }

        wobbleCoroutine = StartCoroutine(WobbleRoutine());
    }

    private IEnumerator WobbleRoutine()
    {
        float elapsedTime = 0f;

        float randomDirX = Random.Range(-1f, 1f);
        float randomDirZ = Random.Range(-1f, 1f);
        Vector3 wobbleAxis = new Vector3(randomDirX, 0f, randomDirZ).normalized;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            
            float dampening = Mathf.Exp(-decayRate * elapsedTime);
            
            float currentAngle = Mathf.Sin(elapsedTime * wobbleFrequency) * wobbleAngle * dampening;
            
            Quaternion offsetRotation = Quaternion.AngleAxis(currentAngle, wobbleAxis);
            target.localRotation = baseLocalRotation * offsetRotation;

            yield return null;
        }
        
        target.localRotation = baseLocalRotation;
        wobbleCoroutine = null;
    }
}