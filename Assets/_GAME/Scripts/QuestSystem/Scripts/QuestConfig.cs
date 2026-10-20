using System;
using System.Collections.Generic;
using UnityEngine;

namespace QuestSystem
{
    /// <summary>Описание одной цели квеста. Прогресс идёт по строковому ключу eventKey.</summary>
    [Serializable]
    public class ObjectiveConfig
    {
        public string id;
        [TextArea(1, 3)] public string description;

        [Tooltip("Ключ события, которое двигает прогресс: QuestManager.Report(\"kill_wolf\")")]
        public string eventKey;

        [Min(1)] public int targetCount = 1;

        [Tooltip("Необязательная цель не блокирует завершение квеста")]
        public bool optional;
    }

    /// <summary>Награда. Сама система ничего не выдаёт - она шлёт событие RewardGranted, а игра решает, что делать.</summary>
    [Serializable]
    public class RewardConfig
    {
        public string rewardId;
        public string displayName;
        public int amount = 1;
        public Sprite icon;
    }

    [CreateAssetMenu(menuName = "Quests/Quest Config", fileName = "Quest_New")]
    public class QuestConfig : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Уникальный id. Если пусто - берётся имя ассета.")]
        public string id;
        public string title;
        [TextArea(2, 6)] public string description;
        public Sprite icon;
        public string category;

        [Header("Rules")]
        [Tooltip("Стартует сам, как только станет доступным")]
        public bool autoStart;

        [Tooltip("Завершается сам, когда выполнены все обязательные цели. Иначе нужно вызвать CompleteQuest (сдача квеста).")]
        public bool autoComplete = true;

        [Tooltip("Цели выполняются строго по порядку")]
        public bool sequentialObjectives;

        [Tooltip("Лимит времени в секундах. 0 - без лимита. По истечении квест провален.")]
        [Min(0)] public float timeLimitSeconds;

        [Tooltip("Квесты, которые должны быть завершены, чтобы этот стал доступен")]
        public List<QuestConfig> prerequisites = new();

        [Header("Content")]
        public List<ObjectiveConfig> objectives = new();
        public List<RewardConfig> rewards = new();

        public string Id => string.IsNullOrEmpty(id) ? name : id;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(id)) id = name;
            for (int i = 0; i < objectives.Count; i++)
            {
                if (string.IsNullOrEmpty(objectives[i].id)) objectives[i].id = $"obj_{i}";
                if (objectives[i].targetCount < 1) objectives[i].targetCount = 1;
            }
        }
#endif
    }
}
