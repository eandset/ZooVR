using System;
using System.Collections.Generic;
using UnityEngine;

namespace QuestSystem
{
    [Serializable]
    public class QuestSaveEntry
    {
        public string id;
        public QuestState state;
        public int[] progress;
        public float timeLeft;
    }

    [Serializable]
    public class QuestSaveData
    {
        public List<QuestSaveEntry> quests = new();
    }

    /// <summary>Подмените, чтобы хранить квесты в своей системе сохранений (файл, облако и т.д.).</summary>
    public interface IQuestSaveProvider
    {
        bool TryLoad(out string json);
        void Save(string json);
        void Clear();
    }

    public sealed class PlayerPrefsQuestSaveProvider : IQuestSaveProvider
    {
        private readonly string _key;
        public PlayerPrefsQuestSaveProvider(string key) { _key = key; }

        public bool TryLoad(out string json)
        {
            json = PlayerPrefs.GetString(_key, null);
            return !string.IsNullOrEmpty(json);
        }

        public void Save(string json)
        {
            PlayerPrefs.SetString(_key, json);
            PlayerPrefs.Save();
        }

        public void Clear() => PlayerPrefs.DeleteKey(_key);
    }
}
