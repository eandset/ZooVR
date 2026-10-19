using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private GameObject key;

    public GameObject Key => key;
}