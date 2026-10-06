using UnityEngine;

[ExecuteInEditMode]
public class RandomInitRotation : MonoBehaviour
{
    [SerializeField] private Vector3 min;
    [SerializeField] private Vector3 max;
    
    private void OnEnable()
    {
        transform.rotation = Quaternion.Euler(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
    }
}
