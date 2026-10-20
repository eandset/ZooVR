using UnityEngine;
using UnityEngine.Events;

namespace QuestSystem
{
    /// <summary>
    /// Триггер-зона: при входе игрока шлёт Report, опционально запускает квест и вызывает UnityEvent.
    /// Работает и с 3D, и с 2D коллайдерами.
    /// </summary>
    public class QuestTriggerZone : MonoBehaviour
    {
        [SerializeField] private string requiredTag = "Player";
        [SerializeField] private string eventKey;
        [SerializeField, Min(1)] private int amount = 1;
        [SerializeField] private QuestConfig startQuest;
        [SerializeField] private bool once = true;
        public UnityEvent onTriggered;

        private bool _used;

        private void OnTriggerEnter(Collider other) => Handle(other.gameObject);
        private void OnTriggerEnter2D(Collider2D other) => Handle(other.gameObject);

        private void Handle(GameObject go)
        {
            if (_used && once) return;
            if (!string.IsNullOrEmpty(requiredTag) && !go.CompareTag(requiredTag)) return;
            _used = true;

            var m = QuestManager.Instance;
            if (m != null)
            {
                if (startQuest != null) m.StartQuest(startQuest);
                if (!string.IsNullOrEmpty(eventKey)) m.Report(eventKey, amount);
            }
            onTriggered?.Invoke();
        }
    }
}
