using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogCompilerView : MonoBehaviour
{
    [Header("Monitors")]
    [SerializeField] private VRMonitorScreen mainMonitor;

    [SerializeField] private List<VRMonitorScreen> answerMonitors;

    [Header("Settings")]
    [SerializeField] private float delayBetweenAnswers = 0.2f;

    public UnityEvent OnMainMonitorClicked;
    public UnityEvent<int> OnAnswerSelected;

    private Coroutine currentSequence;

    private void Awake()
    {
        mainMonitor.OnScreenActivated.AddListener(() => OnMainMonitorClicked?.Invoke());

        for (int i = 0; i < answerMonitors.Count; i++)
        {
            int index = i;
            answerMonitors[i].OnScreenActivated.AddListener(() => OnAnswerSelected?.Invoke(index));
        }
    }
    
    public void DisplayDialogue(params DialogueText[] texts)
    {
        if (currentSequence != null) StopCoroutine(currentSequence);
        currentSequence = StartCoroutine(AnimateDialogueRoutine(texts));
    }

    private IEnumerator AnimateDialogueRoutine(DialogueText[] texts)
    {
        InitializeTextDisplays();
        
        yield return ShowMainText(texts[0]);
        
        bool hasAnswers = texts.Length > 1;
        
        if (hasAnswers)
        {
            yield return ShowAnswerChoices(texts);
        }
        
        mainMonitor.SetInteractive(!hasAnswers);
    }

    private void InitializeTextDisplays()
    {
        mainMonitor.SetInteractive(false);
        
        foreach (var monitor in answerMonitors)
        {
            monitor.TextDisplay.ResetText();
            monitor.SetInteractive(false);
        }
    }

    private IEnumerator ShowMainText(DialogueText text)
    {
        mainMonitor.SetInteractive(false);

        mainMonitor.TextDisplay.SetInputAnimation(text.InputAnimation);
        
        yield return mainMonitor.TextDisplay.SetAnimateText(text.Message);
        
        mainMonitor.TextDisplay.SetProcessAnimation(text.ProcessAnimation);
        mainMonitor.TextDisplay.PlayProcessAnimation();
    }
    
    private IEnumerator ShowAnswerChoices(DialogueText[] texts)
    {
        int answerCount = texts.Length - 1;
        
        for (int i = 0; i < answerCount && i < answerMonitors.Count; i++)
        {
            var dialogueText = texts[i + 1];
            var monitor = answerMonitors[i];
            
            monitor.TextDisplay.SetInputAnimation(dialogueText.InputAnimation);
            
            yield return new WaitForSeconds(delayBetweenAnswers);
            
            yield return monitor.TextDisplay.SetAnimateText(dialogueText.Message);
            
            monitor.TextDisplay.SetProcessAnimation(dialogueText.ProcessAnimation);
            monitor.TextDisplay.PlayProcessAnimation();
            
            monitor.SetInteractive(true);
        }
    }

    public void ResetView(string endMessage = "The end.")
    {
        if (currentSequence != null) StopCoroutine(currentSequence);

        mainMonitor.TextDisplay.SetInputAnimation(null);
        mainMonitor.TextDisplay.SetProcessAnimation(null);
        
        mainMonitor.TextDisplay.SetText(endMessage);
        mainMonitor.SetInteractive(false);

        foreach (var monitor in answerMonitors)
        {
            monitor.TextDisplay.ResetText();
            monitor.SetInteractive(false);
        }
    }
}