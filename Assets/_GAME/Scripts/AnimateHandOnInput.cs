using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHandOnInput : MonoBehaviour
{
    private static readonly int Trigger = Animator.StringToHash("Trigger");
    private static readonly int Grip = Animator.StringToHash("Grip");
    
    [SerializeField] private InputActionProperty triggerValue;
    [SerializeField] private InputActionProperty gripValue;
    
    [SerializeField] private Animator animator;
    
    private void Update()
    {
        float trigger = triggerValue.action.ReadValue<float>();
        float grip = gripValue.action.ReadValue<float>();

        animator.SetFloat(Trigger, trigger);
        animator.SetFloat(Grip, grip);
    }
}
