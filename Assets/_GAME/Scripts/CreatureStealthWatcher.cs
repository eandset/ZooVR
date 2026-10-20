using ithappy.Animals_FREE;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(CreatureMover))]
public class CreatureStealthWatcher : MonoBehaviour
{
    [Header("Watch Timing")]
    [SerializeField] private float m_MinWatchInterval = 2f;
    [SerializeField] private float m_MaxWatchInterval = 5f;

    [Header("Watch Direction")]
    [SerializeField] private float m_WatchDistance = 5f;
    [SerializeField, Range(0f, 360f)] private float m_WatchAngleRange = 140f;
    [Tooltip("Минимальный угол доворота, чтобы гарантированно преодолеть порог в CreatureMover (~75°)")]
    [SerializeField] private float m_MinTurnAngle = 80f;

    [Header("Events")]
    [SerializeField] private UnityEvent m_OnWatchStarted;
    [SerializeField] private UnityEvent m_OnWatchStopped;

    private CreatureMover m_Mover;
    private Coroutine m_WatchRoutine;

    private Vector3 m_BaseForward;
    private Vector3 m_Origin;

    public bool IsWatching => m_WatchRoutine != null;

    private void Awake()
    {
        m_Mover = GetComponent<CreatureMover>();

        StartWatching();
    }

    // ===================== Публичный API (для триггеров / событий) =====================

    /// <summary>Начать слежку, используя текущее направление взгляда как базовое.</summary>
    public void StartWatching()
    {
        StartWatchingInternal(transform.position, transform.forward);
    }

    /// <summary>Остановить слежку. Персонаж замирает в текущем направлении.</summary>
    public void StopWatching()
    {
        if (m_WatchRoutine == null)
        {
            return;
        }

        StopCoroutine(m_WatchRoutine);
        m_WatchRoutine = null;

        // Фиксируем текущий взгляд, чтобы не было резкого "доворота" при следующем старте
        m_Mover.SetInput(Vector2.zero, transform.position + transform.forward, false, false);

        m_OnWatchStopped?.Invoke();
    }

    // ===================== Внутренняя логика =====================

    private void StartWatchingInternal(Vector3 origin, Vector3 baseForward)
    {
        StopWatchingSilent();

        m_Origin = origin;
        m_BaseForward = baseForward;

        m_WatchRoutine = StartCoroutine(WatchRoutine());
        m_OnWatchStarted?.Invoke();
    }

    private void StopWatchingSilent()
    {
        if (m_WatchRoutine != null)
        {
            StopCoroutine(m_WatchRoutine);
            m_WatchRoutine = null;
        }
    }

    private IEnumerator WatchRoutine()
    {
        // Сразу смотрим в базовом направлении
        LookTowards(m_BaseForward);

        while (true)
        {
            var wait = Random.Range(m_MinWatchInterval, m_MaxWatchInterval);
            yield return new WaitForSeconds(wait);

            LookRandomDirection();
        }
    }

    private void LookRandomDirection()
    {
        var halfRange = m_WatchAngleRange * 0.5f;

        // Гарантируем, что угол доворота будет достаточным, чтобы преодолеть
        // внутренний порог CreatureMover (m_Luft ~ 75 градусов)
        var minAngle = Mathf.Min(m_MinTurnAngle, halfRange);
        var sign = Random.value < 0.5f ? -1f : 1f;
        var angle = sign * Random.Range(minAngle, halfRange);

        var direction = Quaternion.Euler(0f, angle, 0f) * m_BaseForward;
        LookTowards(direction);
    }

    private void LookTowards(Vector3 direction)
    {
        direction.y = 0f;
        if (direction.sqrMagnitude < Mathf.Epsilon)
        {
            return;
        }

        var lookTarget = transform.position + direction.normalized * m_WatchDistance;

        // axis = Vector2.zero => персонаж не двигается, только поворачивается
        m_Mover.SetInput(Vector2.zero, lookTarget, false, false);
    }

    private void OnDestroy()
    {
        StopWatchingSilent();
    }
}
