using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace QuestSystem
{
    /// <summary>Одна строка/карточка квеста в списке. Все ссылки необязательные - назначайте только нужные.</summary>
    public class QuestItemView : MonoBehaviour
    {
        [Serializable]
        public class StateStyle
        {
            public QuestState state;
            public string label;
            public Color color = Color.white;
        }

        [Header("Texts")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text objectivesText;
        [SerializeField] private TMP_Text rewardsText;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text stateText;
        [SerializeField] private Image iconImage;

        [Header("Progress")]
        [SerializeField] private Image progressFill;      // Image Type = Filled
        [SerializeField] private Slider progressSlider;
        [SerializeField] private TMP_Text progressText;

        [Header("State")]
        [SerializeField] private Graphic stateTint;
        [SerializeField] private List<StateStyle> stateStyles = new();

        [Header("Buttons")]
        [SerializeField] private Button selectButton;
        [SerializeField] private Button actionButton;     // "Начать" / "Сдать"
        [SerializeField] private TMP_Text actionLabel;
        [SerializeField] private string startLabel = "Начать";
        [SerializeField] private string turnInLabel = "Сдать";

        [Header("Formatting")]
        [SerializeField] private string objectiveFormat = "{0} {1} ({2}/{3})";
        [SerializeField] private string doneMark = "[+]";
        [SerializeField] private string pendingMark = "[ ]";
        [SerializeField] private string optionalSuffix = " (необяз.)";
        [SerializeField] private string rewardFormat = "{0} x{1}";

        public QuestRuntime Quest { get; private set; }
        public int Order { get; private set; }
        public event Action<QuestItemView> Selected;

        private QuestManager _manager;
        private readonly StringBuilder _sb = new();

        private void Awake()
        {
            if (selectButton != null) selectButton.onClick.AddListener(() => Selected?.Invoke(this));
            if (actionButton != null) actionButton.onClick.AddListener(OnActionClicked);
        }

        public void Bind(QuestRuntime quest, QuestManager manager, int order)
        {
            Quest = quest;
            _manager = manager;
            Order = order;
            Refresh();
        }

        private void Update()
        {
            if (Quest != null && timerText != null) RefreshTimer();
        }

        public void Refresh()
        {
            if (Quest == null) return;
            var cfg = Quest.Config;

            if (titleText != null) titleText.text = cfg.title;
            if (descriptionText != null) descriptionText.text = cfg.description;
            if (iconImage != null)
            {
                iconImage.sprite = cfg.icon;
                iconImage.enabled = cfg.icon != null;
            }

            RefreshObjectives();
            RefreshRewards();
            RefreshProgress();
            RefreshState();
            RefreshAction();
            RefreshTimer();
        }

        private void RefreshObjectives()
        {
            if (objectivesText == null) return;
            _sb.Clear();
            foreach (var o in Quest.Objectives)
            {
                string desc = o.Config.description + (o.Config.optional ? optionalSuffix : "");
                _sb.AppendLine(string.Format(objectiveFormat,
                    o.IsDone ? doneMark : pendingMark, desc, o.Current, o.Target));
            }
            objectivesText.text = _sb.ToString().TrimEnd();
        }

        private void RefreshRewards()
        {
            if (rewardsText == null) return;
            _sb.Clear();
            foreach (var r in Quest.Config.rewards)
            {
                if (r == null) continue;
                string name = string.IsNullOrEmpty(r.displayName) ? r.rewardId : r.displayName;
                if (_sb.Length > 0) _sb.Append(", ");
                _sb.Append(string.Format(rewardFormat, name, r.amount));
            }
            rewardsText.text = _sb.ToString();
        }

        private void RefreshProgress()
        {
            float p = Quest.State == QuestState.Completed ? 1f : Quest.Progress;
            if (progressFill != null) progressFill.fillAmount = p;
            if (progressSlider != null) progressSlider.value = p;
            if (progressText != null) progressText.text = $"{Mathf.RoundToInt(p * 100f)}%";
        }

        private void RefreshState()
        {
            var style = FindStyle(Quest.State);
            if (stateText != null) stateText.text = style != null && !string.IsNullOrEmpty(style.label)
                ? style.label : Quest.State.ToString();
            if (stateTint != null) stateTint.color = style != null ? style.color : DefaultColor(Quest.State);
        }

        private void RefreshAction()
        {
            if (actionButton == null) return;

            bool canStart = Quest.State == QuestState.Available;
            bool canTurnIn = Quest.State == QuestState.Active && Quest.AllRequiredDone;
            actionButton.gameObject.SetActive(canStart || canTurnIn);
            if (actionLabel != null) actionLabel.text = canStart ? startLabel : turnInLabel;
        }

        private void RefreshTimer()
        {
            if (timerText == null) return;
            bool show = Quest.State == QuestState.Active && Quest.HasTimeLimit;
            if (timerText.gameObject.activeSelf != show) timerText.gameObject.SetActive(show);
            if (!show) return;
            var t = TimeSpan.FromSeconds(Mathf.Max(0f, Quest.TimeLeft));
            timerText.text = $"{(int)t.TotalMinutes:00}:{t.Seconds:00}";
        }

        private void OnActionClicked()
        {
            if (_manager == null || Quest == null) return;
            if (Quest.State == QuestState.Available) _manager.StartQuest(Quest);
            else if (Quest.State == QuestState.Active) _manager.CompleteQuest(Quest);
        }

        private StateStyle FindStyle(QuestState state)
        {
            foreach (var s in stateStyles)
                if (s.state == state) return s;
            return null;
        }

        private static Color DefaultColor(QuestState state) => state switch
        {
            QuestState.Active => new Color(1f, 0.85f, 0.3f),
            QuestState.Available => new Color(0.5f, 0.8f, 1f),
            QuestState.Completed => new Color(0.4f, 0.9f, 0.4f),
            QuestState.Failed => new Color(0.9f, 0.35f, 0.35f),
            _ => Color.gray
        };
    }
}
