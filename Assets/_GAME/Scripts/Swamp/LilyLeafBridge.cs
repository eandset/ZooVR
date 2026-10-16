using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRGrabInteractable))]
public class LilyLeafBridge : MonoBehaviour
{
    [Header("Настройки возврата")]
    [Tooltip("Время возврата на стартовую точку в секундах (0 — мгновенно)")]
    [SerializeField] private float returnDuration = 0.5f;
    [SerializeField] private bool smoothReturn = true;

    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Coroutine returnCoroutine;
    private Coroutine checkSocketCoroutine;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        // Сохраняем исходные координаты при запуске
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    private void OnEnable()
    {
        grabInteractable.selectExited.AddListener(OnSelectExited);
    }

    private void OnDisable()
    {
        grabInteractable.selectExited.RemoveListener(OnSelectExited);
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        // Прерываем предыдущие проверки возврата
        if (checkSocketCoroutine != null)
        {
            StopCoroutine(checkSocketCoroutine);
            checkSocketCoroutine = null;
        }

        // Запускаем корутину только если сам объект кувшинки активен на сцене
        if (gameObject.activeInHierarchy)
        {
            checkSocketCoroutine = StartCoroutine(CheckIfInSocketRoutine(args));
        }
    }

    /// <summary>
    /// Ждем один кадр, чтобы XR Toolkit успел зафиксировать попадание в сокет,
    /// и только после этого решаем, возвращать ли кувшинку на старт.
    /// </summary>
    private IEnumerator CheckIfInSocketRoutine(SelectExitEventArgs args)
    {
        // Даем системе один кадр на обновление состояния взаимодействия
        yield return null;

        // Если кувшинка в этот момент уже подцеплена каким-то сокетом (например, другой кувшинки или базы)
        // или если interactorObject это сокет — отменяем возврат
        bool isInSocket = args.interactorObject is XRSocketInteractor || grabInteractable.isSelected;

        // Дополнительная проверка через XR Toolkit: проверяем, находится ли интерактив в сокете прямо сейчас
        if (grabInteractable.interactorsSelecting.Count > 0)
        {
            foreach (var interactor in grabInteractable.interactorsSelecting)
            {
                if (interactor is XRSocketInteractor)
                {
                    isInSocket = true;
                    break;
                }
            }
        }

        // Если кувшинка НЕ в сокете — плавно/мгновенно возвращаем её на стартовую позицию
        if (!isInSocket)
        {
            ReturnToStart();
        }

        checkSocketCoroutine = null;
    }

    private void ReturnToStart()
    {
        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
        }

        if (smoothReturn && returnDuration > 0f)
        {
            returnCoroutine = StartCoroutine(SmoothReturnRoutine());
        }
        else
        {
            ResetPositionInstant();
        }
    }

    private IEnumerator SmoothReturnRoutine()
    {
        bool wasKinematic = rb && rb.isKinematic;
        if (rb && !rb.isKinematic)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < returnDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / returnDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            transform.position = Vector3.Lerp(startPosition, originalPosition, smoothT);
            transform.rotation = Quaternion.Slerp(startRotation, originalRotation, smoothT);

            yield return null;
        }

        transform.position = originalPosition;
        transform.rotation = originalRotation;

        if (rb)
        {
            rb.isKinematic = wasKinematic;
        }

        returnCoroutine = null;
    }

    private void ResetPositionInstant()
    {
        if (rb && !rb.isKinematic)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }
}