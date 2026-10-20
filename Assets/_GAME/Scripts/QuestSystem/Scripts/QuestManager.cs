using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace QuestSystem
{
    /// <summary>
    /// Ядро системы. Живёт в сцене одним экземпляром.
    /// Прогресс двигается через Report("ключ"), состояния - через Start/Complete/Fail/Reset.
    /// Все изменения доступны и как C#-события, и как UnityEvent в инспекторе.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance { get; private set; }

        [Header("Data")]
        [SerializeField] private QuestDatabase database;

        [Header("Behaviour")]
        [SerializeField] private bool dontDestroyOnLoad = true;
        [SerializeField] private bool useUnscaledTime = false;

        [Header("Save")]
        [SerializeField] private bool autoLoad = true;
        [SerializeField] private bool autoSave = true;
        [SerializeField] private string saveKey = "quests.save";

        [Header("Unity Events (настраиваются в инспекторе)")]
        public UnityEvent<QuestRuntime> onQuestUnlocked;
        public UnityEvent<QuestRuntime> onQuestStarted;
        public UnityEvent<QuestRuntime> onQuestUpdated;
        public UnityEvent<QuestRuntime> onQuestCompleted;
        public UnityEvent<QuestRuntime> onQuestFailed;
        public UnityEvent<RewardConfig> onRewardGranted;

        // C#-события для кода
        public event Action<QuestRuntime> QuestUnlocked;
        public event Action<QuestRuntime> QuestStarted;
        public event Action<QuestRuntime> QuestUpdated;
        public event Action<QuestRuntime> QuestCompleted;
        public event Action<QuestRuntime> QuestFailed;
        public event Action<QuestRuntime, RewardConfig> RewardGranted;
        /// <summary>Любое структурное изменение (смена состояния, сброс, загрузка).</summary>
        public event Action ListChanged;

        private readonly List<QuestRuntime> _quests = new();
        private readonly Dictionary<string, QuestRuntime> _byId = new();
        private IQuestSaveProvider _saveProvider;
        private bool _dirty;

        public IReadOnlyList<QuestRuntime> Quests => _quests;

        // ---------------------------------------------------------------- lifecycle

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            if (dontDestroyOnLoad)
            {
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }

            _saveProvider = new PlayerPrefsQuestSaveProvider(saveKey);
            Build();
            if (autoLoad) Load();
            RefreshLockedStates(raiseEvents: false);
        }

        private void Start()
        {
            // В Start, чтобы все подписчики (view, хуки) уже успели подписаться
            RefreshAvailability();
            ListChanged?.Invoke();
        }

        private void Update()
        {
            float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            for (int i = 0; i < _quests.Count; i++)
            {
                var q = _quests[i];
                if (q.State != QuestState.Active || !q.HasTimeLimit) continue;
                q.TimeLeft -= dt;
                if (q.TimeLeft <= 0f)
                {
                    q.TimeLeft = 0f;
                    FailQuest(q);
                }
            }
        }

        private void LateUpdate()
        {
            if (_dirty && autoSave) Save();
        }

        private void OnApplicationPause(bool paused) { if (paused && autoSave) Save(); }
        private void OnApplicationQuit() { if (autoSave) Save(); }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        /// <summary>Можно подменить хранилище до первого сохранения/загрузки (например в Awake другого скрипта с меньшим порядком).</summary>
        public void SetSaveProvider(IQuestSaveProvider provider) => _saveProvider = provider;

        private void Build()
        {
            _quests.Clear();
            _byId.Clear();
            if (database == null)
            {
                Debug.LogWarning("[Quests] QuestDatabase не назначен.", this);
                return;
            }
            foreach (var cfg in database.quests)
            {
                if (cfg == null) continue;
                if (_byId.ContainsKey(cfg.Id))
                {
                    Debug.LogWarning($"[Quests] Дубликат id '{cfg.Id}', пропущен.", this);
                    continue;
                }
                var rt = new QuestRuntime(cfg) { State = QuestState.Locked };
                rt.TimeLeft = cfg.timeLimitSeconds;
                _quests.Add(rt);
                _byId.Add(cfg.Id, rt);
            }
        }

        // ---------------------------------------------------------------- queries

        public QuestRuntime Get(string id) => id != null && _byId.TryGetValue(id, out var q) ? q : null;
        public QuestRuntime Get(QuestConfig cfg) => cfg == null ? null : Get(cfg.Id);

        public QuestState GetState(string id) => Get(id)?.State ?? QuestState.Locked;
        public bool IsCompleted(string id) => GetState(id) == QuestState.Completed;
        public bool IsActive(string id) => GetState(id) == QuestState.Active;

        public IEnumerable<QuestRuntime> GetByState(QuestState state)
        {
            foreach (var q in _quests)
                if (q.State == state) yield return q;
        }

        // ---------------------------------------------------------------- commands

        public bool StartQuest(string id) => StartQuest(Get(id));
        public bool StartQuest(QuestConfig cfg) => StartQuest(Get(cfg));

        public bool StartQuest(QuestRuntime q)
        {
            if (q == null || q.State != QuestState.Available) return false;

            q.ResetProgress();
            q.State = QuestState.Active;
            Raise(q, QuestStarted, onQuestStarted);
            MarkChanged();
            return true;
        }

        /// <summary>Сообщить о событии в игре. Двигает все активные квесты с подходящей целью.</summary>
        /// <returns>Сколько квестов изменилось.</returns>
        public int Report(string eventKey, int amount = 1)
        {
            if (string.IsNullOrEmpty(eventKey) || amount <= 0) return 0;

            List<QuestRuntime> touched = null;
            foreach (var q in _quests)
            {
                if (q.State != QuestState.Active) continue;
                if (!q.ApplyProgress(eventKey, amount)) continue;
                (touched ??= new List<QuestRuntime>()).Add(q);
            }
            if (touched == null) return 0;

            // Сначала все апдейты, потом завершения - чтобы цепочки автостарта не получили тот же Report
            foreach (var q in touched) Raise(q, QuestUpdated, onQuestUpdated);
            foreach (var q in touched)
                if (q.State == QuestState.Active && q.Config.autoComplete && q.AllRequiredDone)
                    CompleteQuest(q);

            MarkChanged();
            return touched.Count;
        }

        public bool CompleteQuest(string id, bool force = false) => CompleteQuest(Get(id), force);
        public bool CompleteQuest(QuestConfig cfg, bool force = false) => CompleteQuest(Get(cfg), force);

        /// <param name="force">Завершить, даже если цели не выполнены (скрипты, чит-команды).</param>
        public bool CompleteQuest(QuestRuntime q, bool force = false)
        {
            if (q == null || q.State != QuestState.Active) return false;
            if (!force && !q.AllRequiredDone) return false;

            q.State = QuestState.Completed;
            Raise(q, QuestCompleted, onQuestCompleted);

            foreach (var reward in q.Config.rewards)
            {
                if (reward == null) continue;
                RewardGranted?.Invoke(q, reward);
                onRewardGranted?.Invoke(reward);
            }

            RefreshAvailability();
            MarkChanged();
            return true;
        }

        public bool FailQuest(string id) => FailQuest(Get(id));
        public bool FailQuest(QuestConfig cfg) => FailQuest(Get(cfg));

        public bool FailQuest(QuestRuntime q)
        {
            if (q == null || q.State != QuestState.Active) return false;
            q.State = QuestState.Failed;
            Raise(q, QuestFailed, onQuestFailed);
            MarkChanged();
            return true;
        }

        /// <summary>Вернуть квест в начальное состояние (Available/Locked в зависимости от prerequisites).</summary>
        public bool ResetQuest(string id) => ResetQuest(Get(id));
        public bool ResetQuest(QuestConfig cfg) => ResetQuest(Get(cfg));

        public bool ResetQuest(QuestRuntime q)
        {
            if (q == null) return false;
            q.ResetProgress();
            q.State = QuestState.Locked;
            RefreshAvailability();
            MarkChanged();
            return true;
        }

        public void ResetAll(bool clearSave = true)
        {
            foreach (var q in _quests)
            {
                q.ResetProgress();
                q.State = QuestState.Locked;
            }
            if (clearSave) _saveProvider?.Clear();
            RefreshAvailability();
            MarkChanged();
        }

        // ---------------------------------------------------------------- availability

        private bool PrerequisitesMet(QuestRuntime q)
        {
            foreach (var p in q.Config.prerequisites)
            {
                if (p == null) continue;
                if (GetState(p.Id) != QuestState.Completed) return false;
            }
            return true;
        }

        private void RefreshLockedStates(bool raiseEvents)
        {
            foreach (var q in _quests)
            {
                if (q.State != QuestState.Locked || !PrerequisitesMet(q)) continue;
                q.State = QuestState.Available;
                if (raiseEvents) Raise(q, QuestUnlocked, onQuestUnlocked);
            }
        }

        private void RefreshAvailability()
        {
            RefreshLockedStates(raiseEvents: true);
            foreach (var q in _quests)
                if (q.State == QuestState.Available && q.Config.autoStart)
                    StartQuest(q);
        }

        // ---------------------------------------------------------------- save / load

        public void Save()
        {
            _dirty = false;
            if (_saveProvider == null) return;

            var data = new QuestSaveData();
            foreach (var q in _quests)
            {
                var entry = new QuestSaveEntry
                {
                    id = q.Id,
                    state = q.State,
                    timeLeft = q.TimeLeft,
                    progress = new int[q.Objectives.Count]
                };
                for (int i = 0; i < entry.progress.Length; i++) entry.progress[i] = q.Objectives[i].Current;
                data.quests.Add(entry);
            }
            _saveProvider.Save(JsonUtility.ToJson(data));
        }

        public bool Load()
        {
            if (_saveProvider == null || !_saveProvider.TryLoad(out var json)) return false;

            var data = JsonUtility.FromJson<QuestSaveData>(json);
            if (data?.quests == null) return false;

            foreach (var entry in data.quests)
            {
                var q = Get(entry.id);
                if (q == null) continue; // квест удалили из базы - игнорируем
                q.State = entry.state;
                q.TimeLeft = entry.timeLeft;
                if (entry.progress == null) continue;
                int n = Mathf.Min(entry.progress.Length, q.Objectives.Count);
                for (int i = 0; i < n; i++) q.Objectives[i].Current = entry.progress[i];
            }
            ListChanged?.Invoke();
            return true;
        }

        // ---------------------------------------------------------------- helpers

        private void Raise(QuestRuntime q, Action<QuestRuntime> csEvent, UnityEvent<QuestRuntime> unityEvent)
        {
            csEvent?.Invoke(q);
            unityEvent?.Invoke(q);
        }

        private void MarkChanged()
        {
            _dirty = true;
            ListChanged?.Invoke();
        }
    }
}
