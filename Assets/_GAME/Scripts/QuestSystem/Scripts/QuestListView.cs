using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace QuestSystem
{
    public enum QuestListFilter
    {
        All,
        Available,
        Active,
        Completed,
        Failed
    }

    /// <summary>
    /// View со списком квестов. Хранит карточки всех квестов из QuestManager,
    /// сама обновляется по его событиям, умеет фильтры и сортировку.
    /// Методы Show* / SetFilter(int) / SetCategory(string) вешаются на кнопки, Toggle и Dropdown.
    /// </summary>
    public class QuestListView : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private QuestItemView itemPrefab;
        [SerializeField] private RectTransform container;
        [SerializeField] private QuestManager manager; // если пусто - возьмётся QuestManager.Instance

        [Header("Filter")]
        [SerializeField] private QuestListFilter filter = QuestListFilter.All;
        [SerializeField] private string category = "";   // пусто = все категории
        [SerializeField] private bool showLocked = false;

        [Header("Optional UI")]
        [SerializeField] private GameObject emptyState;
        [SerializeField] private TMP_Text counterText;   // "Выполнено 3/10"

        [Header("Events")]
        public UnityEvent<QuestRuntime> onQuestSelected;
        public UnityEvent onListRefreshed;

        private readonly Dictionary<string, QuestItemView> _items = new();
        private readonly List<QuestItemView> _sorted = new();
        private QuestManager _m;

        // ---------------------------------------------------------------- lifecycle

        private void OnEnable()
        {
            _m = manager != null ? manager
                : QuestManager.Instance != null ? QuestManager.Instance
                : FindFirstObjectByType<QuestManager>();

            if (_m == null)
            {
                Debug.LogWarning("[Quests] QuestListView: QuestManager не найден.", this);
                return;
            }

            _m.QuestUpdated += OnQuestUpdated;
            _m.ListChanged += OnListChanged;

            BuildItems();
            Apply();
        }

        private void OnDisable()
        {
            if (_m == null) return;
            _m.QuestUpdated -= OnQuestUpdated;
            _m.ListChanged -= OnListChanged;
            _m = null;
        }

        // ---------------------------------------------------------------- build

        private void BuildItems()
        {
            if (itemPrefab == null || container == null)
            {
                Debug.LogWarning("[Quests] QuestListView: не назначены itemPrefab/container.", this);
                return;
            }

            for (int i = 0; i < _m.Quests.Count; i++)
            {
                var q = _m.Quests[i];
                if (_items.ContainsKey(q.Id)) continue;

                var item = Instantiate(itemPrefab, container);
                item.name = $"Quest_{q.Id}";
                item.Bind(q, _m, i);
                item.Selected += OnItemSelected;
                _items.Add(q.Id, item);
            }
        }

        // ---------------------------------------------------------------- reactions

        private void OnQuestUpdated(QuestRuntime q)
        {
            if (_items.TryGetValue(q.Id, out var item)) item.Refresh();
        }

        private void OnListChanged()
        {
            foreach (var item in _items.Values) item.Refresh();
            Apply();
        }

        private void OnItemSelected(QuestItemView item) => onQuestSelected?.Invoke(item.Quest);

        // ---------------------------------------------------------------- filter & sort

        private void Apply()
        {
            if (_m == null) return;

            _sorted.Clear();
            int visible = 0, completed = 0, total = 0;

            foreach (var item in _items.Values)
            {
                var q = item.Quest;
                total++;
                if (q.State == QuestState.Completed) completed++;

                bool show = Matches(q);
                if (item.gameObject.activeSelf != show) item.gameObject.SetActive(show);
                if (show)
                {
                    visible++;
                    _sorted.Add(item);
                }
            }

            _sorted.Sort(CompareItems);
            for (int i = 0; i < _sorted.Count; i++) _sorted[i].transform.SetSiblingIndex(i);

            if (emptyState != null) emptyState.SetActive(visible == 0);
            if (counterText != null) counterText.text = $"{completed}/{total}";
            onListRefreshed?.Invoke();
        }

        private bool Matches(QuestRuntime q)
        {
            if (q.State == QuestState.Locked && !showLocked) return false;

            if (!string.IsNullOrEmpty(category) && q.Config.category != category) return false;

            return filter switch
            {
                QuestListFilter.Available => q.State == QuestState.Available,
                QuestListFilter.Active => q.State == QuestState.Active,
                QuestListFilter.Completed => q.State == QuestState.Completed,
                QuestListFilter.Failed => q.State == QuestState.Failed,
                _ => true
            };
        }

        private static int Rank(QuestState s) => s switch
        {
            QuestState.Active => 0,
            QuestState.Available => 1,
            QuestState.Completed => 2,
            QuestState.Failed => 3,
            _ => 4
        };

        private static int CompareItems(QuestItemView a, QuestItemView b)
        {
            int c = Rank(a.Quest.State).CompareTo(Rank(b.Quest.State));
            return c != 0 ? c : a.Order.CompareTo(b.Order);
        }

        // ---------------------------------------------------------------- public API (кнопки / UnityEvent)

        public void SetFilter(QuestListFilter value)
        {
            filter = value;
            Apply();
        }

        /// <summary>Для Dropdown / Toggle: индекс совпадает с порядком QuestListFilter.</summary>
        public void SetFilter(int index) => SetFilter((QuestListFilter)Mathf.Clamp(index, 0, 4));

        public void ShowAll() => SetFilter(QuestListFilter.All);
        public void ShowAvailable() => SetFilter(QuestListFilter.Available);
        public void ShowActive() => SetFilter(QuestListFilter.Active);
        public void ShowCompleted() => SetFilter(QuestListFilter.Completed);
        public void ShowFailed() => SetFilter(QuestListFilter.Failed);

        public void SetCategory(string value)
        {
            category = value ?? "";
            Apply();
        }

        public void SetShowLocked(bool value)
        {
            showLocked = value;
            Apply();
        }

        public QuestItemView GetItem(string questId) => _items.TryGetValue(questId, out var i) ? i : null;
    }
}
