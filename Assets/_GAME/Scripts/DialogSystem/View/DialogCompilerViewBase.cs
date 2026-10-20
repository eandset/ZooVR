using UnityEngine;
using UnityEngine.Events;

public abstract class DialogCompilerViewBase : MonoBehaviour
{
    public UnityEvent OnMainMonitorClicked;
    public UnityEvent<int> OnAnswerSelected;
    
    public abstract void DisplayDialogue(DialogueText[] texts);
    public abstract void ResetView(string message = "The end.");
}