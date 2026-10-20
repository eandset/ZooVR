using Unity.Mathematics;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public enum LookAxis
    {
        ForwardZ,      // Локальная ось Z смотрит на цель
        ForwardX,      // Локальная ось X смотрит на цель
        BackZ,         // Ось -Z смотрит на цель
        BackX          // Ось -X смотрит на цель
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
        if (!targetTransform)
        {
            FindTarget();

            if (!targetTransform)
                return;
        }

        Vector3 directionToTarget =
            targetTransform.position - transform.position;

        if (directionToTarget.sqrMagnitude < 0.000001f)
            return;

        Quaternion targetRotation;

        switch (lookAxis)
        {
            case LookAxis.ForwardZ:
                targetRotation = Quaternion.LookRotation(directionToTarget);
                break;

            case LookAxis.ForwardX:
                targetRotation =
                    Quaternion.LookRotation(directionToTarget) *
                    Quaternion.Euler(0f, -90f, 0f);
                break;

            case LookAxis.BackZ:
                targetRotation = Quaternion.LookRotation(-directionToTarget);
                break;

            case LookAxis.BackX:
                targetRotation =
                    Quaternion.LookRotation(-directionToTarget) *
                    Quaternion.Euler(0f, -90f, 0f);
                break;
            
            default:
                targetRotation = Quaternion.LookRotation(directionToTarget);
                break;
        }

        // Блокировка вращения по выбранным осям.
        if (lockAxis.x || lockAxis.y || lockAxis.z)
        {
            Vector3 currentEuler = transform.rotation.eulerAngles;
            Vector3 targetEuler = targetRotation.eulerAngles;

            float finalX = lockAxis.x ? currentEuler.x : targetEuler.x;
            float finalY = lockAxis.y ? currentEuler.y : targetEuler.y;
            float finalZ = lockAxis.z ? currentEuler.z : targetEuler.z;

            targetRotation = Quaternion.Euler(finalX, finalY, finalZ);
        }

        // Применение вращения.
        if (rotationSpeed <= 0f)
        {
            transform.rotation = targetRotation;
        }
        else
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private void FindTarget()
    {
        targetTransform = Camera.main?.transform;
    }
}