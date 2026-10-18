using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueNode", menuName = "VR/Dialogue Node")]
public class DialogueNodeSO : ScriptableObject
{
    [TextArea(3, 10)]
    [Tooltip("Текст, который выводится на главный монитор")]
    public string mainQuestionText;

    [Tooltip("Ссылка на следующий узел, если вариантов ответа НЕТ (просто продолжение монолога/диалога)")]
    public DialogueNodeSO nextNode;

    [Tooltip("Варианты ответа. Если они есть, поле nextNode игнорируется, а выбор идет через мониторы ответов")]
    public DialogueAnswer[] answers;
}

[System.Serializable]
public struct DialogueAnswer
{
    [Tooltip("Текст ответа на вспомогательном мониторе")]
    public string answerText;

    [Tooltip("Следующий узел диалога при выборе этого ответа")]
    public DialogueNodeSO nextNode;
}