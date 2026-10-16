using UnityEngine;

public class FrogController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float delay;

    [Header("Action Weights")]
    [SerializeField, Min(1)] private int idleWeight;
    [SerializeField, Min(1)] private int jumpWeight;
    
    private static readonly int IdleIndex = Animator.StringToHash("IdleIndex");
    private static readonly int JumpTrigger = Animator.StringToHash("JumpTrigger");

    private readonly int[] _idleIndexes = { 1, 3 };

    private float _timer;

    private void Update()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
            return;
        }

        var randomAction = Random.Range(0, idleWeight + jumpWeight);
        var condition = randomAction < idleWeight;

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
        animator.SetTrigger(JumpTrigger);
    }

    private int GetRandomIdleIndex()
    {
        int indexArray = Random.Range(0, _idleIndexes.Length);
        return _idleIndexes[indexArray];
    }
}
