using System.Collections;
using UnityEngine;

public class FrogController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject frog;
    [SerializeField] private Transform waterPoint;
    [SerializeField] private GameObject vfxWater;
    [SerializeField] private float delay;

    [Header("Action Weights")]
    [SerializeField, Min(1)] private int idleWeight;
    [SerializeField, Min(1)] private int jumpWeight;

    private const string IdleIndex = nameof(IdleIndex);
    private const string JumpTrigger = nameof(JumpTrigger);

    private readonly int[] _idleIndexes = { 1, 3 };

    private float _timer;
    private bool _isInWater;

    private void Update()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
            return;
        }

        var randomAction = Random.Range(0, idleWeight + jumpWeight);
        var condition = _isInWater ? randomAction >= idleWeight : randomAction < idleWeight;

        if (condition)
            Idle();
        else
            Jump();

        _timer = delay;
    }

    private void Idle()
    {
        animator.SetInteger(IdleIndex, GetRandomIdleIndex());
    }

    private void Jump()
    {
        var vfxSpawned = Instantiate(vfxWater, waterPoint.transform.position, Quaternion.identity);
        Destroy(vfxSpawned, 2);

        if (_isInWater)
        {
            frog.transform.SetPositionAndRotation(waterPoint.position, waterPoint.rotation);
            frog.SetActive(true);
        }

        _isInWater = !_isInWater;

        animator.SetTrigger(JumpTrigger);
    }

    private int GetRandomIdleIndex()
    {
        int indexArray = Random.Range(0, _idleIndexes.Length);
        return _idleIndexes[indexArray];
    }
}
