using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueNode", menuName = "VR/Dialogue Node")]
public class DialogueNodeSO : ScriptableObject
{
    [field: SerializeField] public Dialogue Node { get; private set; }
    [field: SerializeField]  public Dialogue[] Answers { get; private set; }
}

[System.Serializable]
public struct Dialogue
{
    [TextArea(3, 10)]
    public string Text;
    
    public DialogueNodeSO NextNode;
}