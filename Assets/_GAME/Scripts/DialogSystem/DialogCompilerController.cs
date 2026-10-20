using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using UnityEngine.Events;

[Serializable]
public class DialogueEndingAction
{
    public DialogueNodeSO node;
    public UnityEvent onReached;
}

public class DialogCompilerController : MonoBehaviour
{
    [SerializeField] private DialogCompilerView dialogueView;
    [SerializeField] private DialogueNodeSO currentDialogueNode;

    [Header("Actions")]
    [SerializeField] private List<DialogueEndingAction> nodeActions;

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
            return;
        }
        
        TriggerAction(node);

        currentDialogueNode = node;

        var textsToDisplay = CollectAllTexts(node);
        dialogueView.DisplayDialogue(textsToDisplay.ToArray());
    }

    private static List<DialogueText> CollectAllTexts(DialogueNodeSO node)
    {
        var textsToDisplay = new List<DialogueText> { node.Node.Text };

        if (node.Answers != null)
        {
            textsToDisplay.AddRange(node.Answers.Select(x => x.Text));
        }

        return textsToDisplay;
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

    private void TriggerAction(DialogueNodeSO node)
    {
        var ending = nodeActions.FirstOrDefault(e => e.node == node);
        ending?.onReached?.Invoke();
    }
}