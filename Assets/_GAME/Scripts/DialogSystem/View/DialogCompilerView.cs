using System.Collections.Generic;
using UnityEngine;

public class DialogCompilerView : DialogCompilerViewBase
{
    [Header("Monitors")]
    [SerializeField] private VRMonitorScreen mainMonitor;

    [SerializeField] private List<VRMonitorScreen> answerMonitors;

    private void Awake()
    {
        mainMonitor.OnScreenActivated.AddListener(() => OnMainMonitorClicked?.Invoke());

        for (int i = 0; i < answerMonitors.Count; i++)
        {
            int index = i;
            answerMonitors[i].OnScreenActivated.AddListener(() => OnAnswerSelected?.Invoke(index));
        }
    }
    
    public override void DisplayDialogue(DialogueText[] texts)
    {
        InitializeTextDisplays();
        
        ShowMainText(texts[0]);
        
        bool hasAnswers = texts.Length > 1;
        
        if (hasAnswers)
        {
            ShowAnswerChoices(texts);
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

    private void ShowMainText(DialogueText text)
    {
        mainMonitor.SetInteractive(false);
        
        mainMonitor.TextDisplay.SetText(text.Message);
    }
    
    private void ShowAnswerChoices(DialogueText[] texts)
    {
        int answerCount = texts.Length - 1;
        
        for (int i = 0; i < answerCount && i < answerMonitors.Count; i++)
        {
            var dialogueText = texts[i + 1];
            var monitor = answerMonitors[i];
            
            monitor.TextDisplay.SetText(dialogueText.Message);
            
            monitor.SetInteractive(true);
        }
    }

    public override void ResetView(string endMessage = "The end.")
    {
        mainMonitor.TextDisplay.SetText(endMessage);
        mainMonitor.SetInteractive(false);

        foreach (var monitor in answerMonitors)
        {
            monitor.TextDisplay.ResetText();
            monitor.SetInteractive(false);
        }
    }
}