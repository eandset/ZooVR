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
    
    public void DisplayDialogue(params string[] texts)
    {
        if (currentSequence != null) StopCoroutine(currentSequence);
        currentSequence = StartCoroutine(AnimateDialogueRoutine(texts));
    }

    private IEnumerator AnimateDialogueRoutine(string[] texts)
    {
        InitializeTextDisplays();
     
        yield return mainMonitor.TextDisplay.TypeTextRoutine(texts[0]);

        var hasAnswers = texts.Length > 1;
        
        if (hasAnswers)
            mainMonitor.TextDisplay.AnimateFlash();
        
        mainMonitor.SetInteractive(!hasAnswers);

        yield return ShowAnswerChoices(texts);
    }

    private void InitializeTextDisplays()
    {
        mainMonitor.TextDisplay.ResetAnimations();
        
        foreach (var monitor in answerMonitors)
        {
            monitor.TextDisplay.SetText("");
            monitor.TextDisplay.ResetAnimations();
            
            monitor.SetInteractive(false);
        }
    }
    
    private IEnumerator ShowAnswerChoices(string[] texts)
    {
        int answerCount = texts.Length - 1;
        
        for (int i = 0; i < answerCount && i < answerMonitors.Count; i++)
        {
            yield return new WaitForSeconds(delayBetweenAnswers);

            yield return answerMonitors[i].TextDisplay.TypeTextRoutine(texts[i + 1]);

            answerMonitors[i].SetInteractive(true);
        }
    }

    public void ResetView(string endMessage = "The end.")
    {
        if (currentSequence != null) StopCoroutine(currentSequence);

        mainMonitor.TextDisplay.SetText(endMessage);
        mainMonitor.SetInteractive(false);

        foreach (var monitor in answerMonitors)
        {
            monitor.TextDisplay.SetText("");
            monitor.SetInteractive(false);
        }
    }
}