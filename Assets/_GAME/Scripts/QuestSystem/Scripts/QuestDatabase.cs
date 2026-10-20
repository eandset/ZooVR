using System.Collections.Generic;
using UnityEngine;

namespace QuestSystem
{
    /// <summary>Список всех квестов игры. Порядок здесь = порядок в списке по умолчанию.</summary>
    [CreateAssetMenu(menuName = "Quests/Quest Database", fileName = "QuestDatabase")]
    public class QuestDatabase : ScriptableObject
    {
        public List<QuestConfig> quests = new();

        public QuestConfig Find(string id)
        {
            foreach (var q in quests)
                if (q != null && q.Id == id) return q;
            return null;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            var seen = new HashSet<string>();
            foreach (var q in quests)
            {
                if (q == null) continue;
                if (!seen.Add(q.Id))
                    Debug.LogWarning($"[Quests] Дублирующийся id квеста: '{q.Id}' в {name}", this);
            }
        }
#endif
    }
}
