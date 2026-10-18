using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueNode", menuName = "DialogSystem/Node")]
public class DialogueNodeSO : ScriptableObject
{
    [field: SerializeField] public Dialogue Node { get; private set; }
    [field: SerializeField]  public Dialogue[] Answers { get; private set; }
}

[System.Serializable]
public struct Dialogue
{
    public DialogueText Text;
    public DialogueNodeSO NextNode;
}

[System.Serializable]
public struct DialogueText
{
    [TextArea(3, 10)]
    public string Message;
    
    [Header("Animation")]
    [CanBeNull] public InputTextAnimationConfig InputAnimation;
    [CanBeNull] public ProcessTextAnimationConfig ProcessAnimation;
}