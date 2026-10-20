using System.Collections.Generic;
using UnityEngine;

namespace QuestSystem
{
    public enum QuestState
    {
        Locked,     // не выполнены prerequisites
        Available,  // можно взять
        Active,     // в процессе
        Completed,
        Failed
    }

    public sealed class ObjectiveRuntime
    {
        public ObjectiveConfig Config { get; }
        public int Current { get; internal set; }
        public int Target => Config.targetCount;
        public bool IsDone => Current >= Config.targetCount;
        public float Normalized => Target <= 0 ? 1f : Mathf.Clamp01((float)Current / Target);

        internal ObjectiveRuntime(ObjectiveConfig config) { Config = config; }
    }

    /// <summary>Состояние квеста во время игры. Конфиг не мутируется.</summary>
    public sealed class QuestRuntime
    {
        private readonly List<ObjectiveRuntime> _objectives = new();

        public QuestConfig Config { get; }
        public QuestState State { get; internal set; }
        public float TimeLeft { get; internal set; }
        public IReadOnlyList<ObjectiveRuntime> Objectives => _objectives;

        public string Id => Config.Id;
        public bool HasTimeLimit => Config.timeLimitSeconds > 0f;

        internal QuestRuntime(QuestConfig config)
        {
            Config = config;
            foreach (var o in config.objectives) _objectives.Add(new ObjectiveRuntime(o));
        }

        public bool AllRequiredDone
        {
            get
            {
                foreach (var o in _objectives)
                    if (!o.Config.optional && !o.IsDone) return false;
                return true;
            }
        }

        /// <summary>Общий прогресс по обязательным целям, 0..1.</summary>
        public float Progress
        {
            get
            {
                int cur = 0, total = 0;
                foreach (var o in _objectives)
                {
                    if (o.Config.optional) continue;
                    total += o.Target;
                    cur += Mathf.Min(o.Current, o.Target);
                }
                if (total == 0) return State == QuestState.Completed ? 1f : 0f;
                return (float)cur / total;
            }
        }

        /// <returns>true, если прогресс хоть одной цели изменился.</returns>
        internal bool ApplyProgress(string key, int amount)
        {
            bool changed = false;
            for (int i = 0; i < _objectives.Count; i++)
            {
                var o = _objectives[i];
                if (o.IsDone || o.Config.eventKey != key) continue;
                if (Config.sequentialObjectives && !PreviousDone(i)) continue;

                o.Current = Mathf.Min(o.Current + amount, o.Target);
                changed = true;
            }
            return changed;
        }

        private bool PreviousDone(int index)
        {
            for (int i = 0; i < index; i++)
                if (!_objectives[i].IsDone) return false;
            return true;
        }

        internal void ResetProgress()
        {
            foreach (var o in _objectives) o.Current = 0;
            TimeLeft = Config.timeLimitSeconds;
        }
    }
}
