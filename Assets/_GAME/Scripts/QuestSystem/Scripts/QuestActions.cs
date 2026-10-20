using UnityEngine;

namespace QuestSystem
{
    /// <summary>
    /// Публичные методы с одним аргументом - чтобы вызывать из UnityEvent (кнопки, диалоги, триггеры, Timeline).
    /// Повесьте на любой объект и выбирайте методы в инспекторе.
    /// </summary>
    public class QuestActions : MonoBehaviour
    {
        [Header("Для ReportConfigured()")]
        [SerializeField] private string eventKey;
        [SerializeField, Min(1)] private int amount = 1;

        private static QuestManager M => QuestManager.Instance;

        public void StartQuest(QuestConfig quest) => M?.StartQuest(quest);
        public void CompleteQuest(QuestConfig quest) => M?.CompleteQuest(quest);
        public void ForceCompleteQuest(QuestConfig quest) => M?.CompleteQuest(quest, force: true);
        public void FailQuest(QuestConfig quest) => M?.FailQuest(quest);
        public void ResetQuest(QuestConfig quest) => M?.ResetQuest(quest);

        public void StartQuestById(string id) => M?.StartQuest(id);
        public void CompleteQuestById(string id) => M?.CompleteQuest(id);

        /// <summary>Report(ключ) на 1 единицу прогресса.</summary>
        public void Report(string key) => M?.Report(key, 1);

        /// <summary>Report с ключом и количеством, заданными в инспекторе этого компонента.</summary>
        public void ReportConfigured() => M?.Report(eventKey, amount);

        public void SaveQuests() => M?.Save();
        public void ResetAllQuests() => M?.ResetAll();
    }
}
