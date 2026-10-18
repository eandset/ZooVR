using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogCompilerController : MonoBehaviour
{
    [SerializeField] private DialogCompilerView dialogueView;
    [SerializeField] private DialogueNodeSO currentDialogueNode;

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

    private void ShowNode(DialogueNodeSO node)
    {
        if (!node)
        {
            dialogueView.ResetView();
            return;
        }

        currentDialogueNode = node;

        var textsToDisplay = new List<string> { node.mainQuestionText };
        textsToDisplay.AddRange(node.answers.Select(answer => answer.answerText));

        dialogueView.DisplayDialogue(textsToDisplay.ToArray());
    }

    private void ProceedToNextNode()
    {
        if (!currentDialogueNode)
            return;
        
        if (currentDialogueNode.answers is { Length: > 0 })
            return;

        ShowNode(currentDialogueNode.nextNode);
    }
    
    private void HandleAnswerChosen(int answerIndex)
    {
        if (!currentDialogueNode || currentDialogueNode.answers == null)
            return;

        if (answerIndex >= 0 && answerIndex < currentDialogueNode.answers.Length)
        {
            var selectedAnswer = currentDialogueNode.answers[answerIndex];
            ShowNode(selectedAnswer.nextNode);
        }
    }
}