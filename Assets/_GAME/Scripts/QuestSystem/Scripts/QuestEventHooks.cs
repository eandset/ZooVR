using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace QuestSystem
{
    /// <summary>
    /// Привязка UnityEvent к конкретным квестам прямо в сцене:
    /// открыть дверь при старте квеста, включить NPC при завершении и т.д. Код не нужен.
    /// </summary>
    public class QuestEventHooks : MonoBehaviour
    {
        [Serializable]
        public class Hook
        {
            public QuestConfig quest;
            public UnityEvent onUnlocked;
            public UnityEvent onStarted;
            public UnityEvent onUpdated;
            public UnityEvent onCompleted;
            public UnityEvent onFailed;
        }

        [SerializeField] private List<Hook> hooks = new();

        private QuestManager _manager;

        private void OnEnable()
        {
            _manager = QuestManager.Instance != null ? QuestManager.Instance : FindAnyObjectByType<QuestManager>();
            if (_manager == null)
            {
                Debug.LogWarning("[Quests] QuestEventHooks: QuestManager не найден.", this);
                return;
            }
            _manager.QuestUnlocked += OnUnlocked;
            _manager.QuestStarted += OnStarted;
            _manager.QuestUpdated += OnUpdated;
            _manager.QuestCompleted += OnCompleted;
            _manager.QuestFailed += OnFailed;
        }

        private void OnDisable()
        {
            if (_manager == null) return;
            _manager.QuestUnlocked -= OnUnlocked;
            _manager.QuestStarted -= OnStarted;
            _manager.QuestUpdated -= OnUpdated;
            _manager.QuestCompleted -= OnCompleted;
            _manager.QuestFailed -= OnFailed;
            _manager = null;
        }

        private void OnUnlocked(QuestRuntime q) => Invoke(q, h => h.onUnlocked);
        private void OnStarted(QuestRuntime q) => Invoke(q, h => h.onStarted);
        private void OnUpdated(QuestRuntime q) => Invoke(q, h => h.onUpdated);
        private void OnCompleted(QuestRuntime q) => Invoke(q, h => h.onCompleted);
        private void OnFailed(QuestRuntime q) => Invoke(q, h => h.onFailed);

        private void Invoke(QuestRuntime q, Func<Hook, UnityEvent> select)
        {
            foreach (var h in hooks)
                if (h.quest != null && h.quest.Id == q.Id)
                    select(h)?.Invoke();
        }
    }
}
