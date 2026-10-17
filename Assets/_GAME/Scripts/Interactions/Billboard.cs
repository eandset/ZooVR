using Unity.Mathematics;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public enum LookAxis
    {
        ForwardZ, // Направлять ось Z (стандартный Forward)
        ForwardX // Направлять ось X (редкие случаи, если меш повернут иначе)
    }
    
    [SerializeField] private LookAxis lookAxis = LookAxis.ForwardZ;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Lock")]
    [SerializeField] private bool3 lockAxis;

    private Transform targetTransform;

    private void Start()
    {
        FindTarget();
    }

    private void LateUpdate()
    {
        // Если цель была уничтожена, пробуем найти ее снова
        if (!targetTransform)
        {
            FindTarget();

            if (!targetTransform)
                return;
        }

        // Вычисляем направление на цель
        Vector3 directionToTarget = targetTransform.position - transform.position;

        if (directionToTarget == Vector3.zero) return;

        // Определяем базовое вращение с учетом выбранной оси
        Quaternion targetRotation = Quaternion.identity;

        if (lookAxis == LookAxis.ForwardZ)
        {
            // Стандартный случай: смотрим вперед по Z
            targetRotation = Quaternion.LookRotation(directionToTarget);
        }
        else if (lookAxis == LookAxis.ForwardX)
        {
            // Поворот так, чтобы локальная ось X смотрела на цель
            targetRotation = Quaternion.LookRotation(directionToTarget) * Quaternion.Euler(0, -90, 0);
        }

        // Применяем блокировку осей, если нужно
        if (lockAxis.x || lockAxis.y || lockAxis.z)
        {
            Vector3 currentEuler = transform.rotation.eulerAngles;
            Vector3 targetEuler = targetRotation.eulerAngles;

            float finalX = lockAxis.x ? currentEuler.x : targetEuler.x;
            float finalY = lockAxis.y ? currentEuler.y : targetEuler.y;
            float finalZ = lockAxis.z ? currentEuler.z : targetEuler.z;

            targetRotation = Quaternion.Euler(finalX, finalY, finalZ);
        }

        // Применяем вращение (мгновенно или сглажено)
        if (rotationSpeed <= 0f)
        {
            transform.rotation = targetRotation;
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void FindTarget()
    {
        targetTransform = Camera.main?.transform;
    }
}