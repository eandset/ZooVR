using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private GameObject key;

    public EntityId KeyEntityId => key.GetEntityId();
}