using ithappy.Animals_FREE;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(CreatureMover))]
public class CreatureTargetMover : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform m_TargetTransform;
    [SerializeField] private Vector3 m_TargetPosition;
    [SerializeField] private bool m_UseTransformTarget;

    [Header("Settings")]
    [SerializeField] private float m_StoppingDistance = 0.3f;
    [SerializeField] private bool m_IsRunning;
    [SerializeField] private bool m_AutoStopOnArrival = true;

    [Header("Events")]
    [SerializeField] private UnityEvent m_OnMovementStarted;
    [SerializeField] private UnityEvent m_OnMovementStopped;
    [SerializeField] private UnityEvent m_OnTargetReached;

    private CreatureMover m_Mover;
    private bool m_IsMovingToTarget;

    public bool IsMoving => m_IsMovingToTarget;
    public bool IsRunning => m_IsRunning;
    public float StoppingDistance => m_StoppingDistance;

    private void Awake()
    {
        m_Mover = GetComponent<CreatureMover>();
    }

    private void Update()
    {
        if (!m_IsMovingToTarget)
        {
            return;
        }

        var targetPosition = GetTargetPosition();
        var toTarget = targetPosition - transform.position;
        toTarget.y = 0f;

        var distance = toTarget.magnitude;

        if (m_AutoStopOnArrival && distance <= m_StoppingDistance)
        {
            m_IsMovingToTarget = false;
            m_Mover.SetInput(Vector2.zero, targetPosition, m_IsRunning, false);
            m_OnTargetReached?.Invoke();
            m_OnMovementStopped?.Invoke();
            return;
        }

        var axis = distance > Mathf.Epsilon ? Vector2.up : Vector2.zero;
        m_Mover.SetInput(axis, targetPosition, m_IsRunning, false);
    }

    private Vector3 GetTargetPosition()
    {
        return m_UseTransformTarget && m_TargetTransform != null
            ? m_TargetTransform.position
            : m_TargetPosition;
    }

    // ===================== Публичный API для UnityEvent =====================

    /// <summary>Задать цель как Transform (например, перетащить объект игрока).</summary>
    public void SetTarget(Transform target)
    {
        m_TargetTransform = target;
        m_UseTransformTarget = true;
    }

    /// <summary>Задать цель как точку в мире.</summary>
    public void SetTarget(Vector3 position)
    {
        m_TargetPosition = position;
        m_UseTransformTarget = false;
    }

    /// <summary>Задать цель и сразу начать движение (Transform).</summary>
    public void SetTargetAndMove(Transform target)
    {
        SetTarget(target);
        StartMoving();
    }

    /// <summary>Задать цель и сразу начать движение (точка).</summary>
    public void SetTargetAndMove(Vector3 position)
    {
        SetTarget(position);
        StartMoving();
    }

    /// <summary>Начать движение к текущей цели.</summary>
    public void StartMoving()
    {
        if (m_IsMovingToTarget)
        {
            return;
        }

        m_IsMovingToTarget = true;
        m_OnMovementStarted?.Invoke();
    }

    /// <summary>Остановить движение (персонаж замирает на месте).</summary>
    public void StopMoving()
    {
        if (!m_IsMovingToTarget)
        {
            return;
        }

        m_IsMovingToTarget = false;
        m_Mover.SetInput(Vector2.zero, GetTargetPosition(), m_IsRunning, false);
        m_OnMovementStopped?.Invoke();
    }

    /// <summary>Переключить бег/ходьбу.</summary>
    public void SetRunning(bool isRunning)
    {
        m_IsRunning = isRunning;
    }

    public void ToggleRunning()
    {
        m_IsRunning = !m_IsRunning;
    }

    /// <summary>Задать дистанцию, на которой движение считается завершённым.</summary>
    public void SetStoppingDistance(float distance)
    {
        m_StoppingDistance = Mathf.Max(0f, distance);
    }
}