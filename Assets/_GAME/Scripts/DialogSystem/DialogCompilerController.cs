using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using UnityEngine.Events;

[Serializable]
public class DialogueEndingAction
{
    public DialogueNodeSO endingNode;
    
    public UnityEvent onEndingReached;
}

public class DialogCompilerController : MonoBehaviour
{
    [SerializeField] private DialogCompilerView dialogueView;
    [SerializeField] private DialogueNodeSO currentDialogueNode;

    [Header("Endings & Actions")]
    [SerializeField] private List<DialogueEndingAction> endingActions;

    private void OnEnable()
    {
        dialogueView.OnMainMonitorClicked.AddListener(ProceedToNextNode);
        dialogueView.OnAnswerSelected.AddListener(HandleAnswerChosen);
    }

    private void OnDisable()
    {
        dialogueView.OnMainMonitorClicked.RemoveListener(ProceedToNextNode);
        dialogueView.OnAnswerSelected.RemoveListener(HandleAnswerChosen);
    }

    private void Start()
    {
        ShowNode(currentDialogueNode);
    }
    
    public void StartDialog(DialogueNodeSO node)
    {
        ShowNode(node);
    }

    private void ShowNode(DialogueNodeSO node)
    {
        if (!node)
        {
            dialogueView.ResetView();
            TriggerEndingAction(currentDialogueNode);
            return;
        }

        currentDialogueNode = node;

        var textsToDisplay = new List<string> { node.Node.Text };

        if (node.Answers != null)
        {
            textsToDisplay.AddRange(node.Answers.Select(answer => answer.Text));
        }

        dialogueView.DisplayDialogue(textsToDisplay.ToArray());
    }

    private void ProceedToNextNode()
    {
        if (!currentDialogueNode)
            return;

        if (currentDialogueNode.Answers is { Length: > 0 })
            return;

        ShowNode(currentDialogueNode.Node.NextNode);
    }

    private void HandleAnswerChosen(int answerIndex)
    {
        if (!currentDialogueNode || currentDialogueNode.Answers == null)
            return;

        if (answerIndex >= 0 && answerIndex < currentDialogueNode.Answers.Length)
        {
            var selectedAnswer = currentDialogueNode.Answers[answerIndex];
            ShowNode(selectedAnswer.NextNode);
        }
    }

    private void TriggerEndingAction(DialogueNodeSO lastNode)
    {
        if (!lastNode) return;
        
        var ending = endingActions.FirstOrDefault(e => e.endingNode == lastNode);

        ending?.onEndingReached?.Invoke();
    }
}