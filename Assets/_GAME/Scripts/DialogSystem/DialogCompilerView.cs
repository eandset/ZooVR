using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogCompilerView : MonoBehaviour
{
    [Header("Monitors")]
    [SerializeField] private VRMonitorScreen mainMonitor;
    [SerializeField] private List<VRMonitorScreen> answerMonitors;

    public UnityEvent OnMainMonitorClicked;
    public UnityEvent<int> OnAnswerSelected;

    private void Awake()
    {
        mainMonitor.OnScreenActivated.AddListener(HandleMainMonitorClick);

        for (int i = 0; i < answerMonitors.Count; i++)
        {
            int index = i;
            answerMonitors[i].OnScreenActivated.AddListener(() => HandleAnswerClick(index));
        }
    }

    private void OnDestroy()
    {
        mainMonitor.OnScreenActivated.RemoveListener(HandleMainMonitorClick);

        foreach (var monitor in answerMonitors)
        {
            monitor.OnScreenActivated.RemoveAllListeners();
        }
    }

    public void DisplayDialogue(params string[] texts)
    {
        if (texts == null || texts.Length == 0)
            return;

        var hasAnswers = texts.Length > 1;
        
        mainMonitor.SetText(texts[0]);
        mainMonitor.SetInteractive(!hasAnswers);
        
        int answerCount = texts.Length - 1;

        for (int i = 0; i < answerMonitors.Count; i++)
        {
            if (i < answerCount)
            {
                answerMonitors[i].SetText(texts[i + 1]);
                answerMonitors[i].SetInteractive(true);
            }
            else
            {
                answerMonitors[i].SetText("");
                answerMonitors[i].SetInteractive(false);
            }
        }
    }

    public void ResetView(string endMessage = "Диалог окончен.")
    {
        mainMonitor.SetText(endMessage);
        mainMonitor.SetInteractive(false);

        foreach (var monitor in answerMonitors)
        {
            monitor.SetText("");
            monitor.SetInteractive(false);
        }
    }

    private void HandleMainMonitorClick()
    {
        OnMainMonitorClicked?.Invoke();
    }

    private void HandleAnswerClick(int index)
    {
        OnAnswerSelected?.Invoke(index);
    }
}