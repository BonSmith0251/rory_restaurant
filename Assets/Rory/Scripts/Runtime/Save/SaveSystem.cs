using System;
using System.IO;
using UnityEngine;

namespace Rory.Save
{
    /// <summary>Five player slots and the global settings, stored as JSON in Application.persistentDataPath/Saves.</summary>
    public class SaveSystem
    {
        readonly string folder;
        readonly PlayerProfile[] slots = new PlayerProfile[GameRules.SaveSlots];

        public GameSettings Settings { get; private set; } = new GameSettings();
        public PlayerProfile Current { get; private set; }

        public event Action CurrentChanged;

        public SaveSystem(string folder = null)
        {
            this.folder = folder ?? Path.Combine(Application.persistentDataPath, "Saves");
        }

        string SlotPath(int slot) => Path.Combine(folder, $"slot{slot}.json");
        string SettingsPath => Path.Combine(folder, "settings.json");

        public bool IsUsed(int slot) => slots[slot] != null;
        public PlayerProfile GetSlot(int slot) => slots[slot];

        public void LoadAll()
        {
            Settings = Read<GameSettings>(SettingsPath) ?? new GameSettings();
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i] = Read<PlayerProfile>(SlotPath(i));
                if (slots[i] == null) continue;
                slots[i].slot = i;
                slots[i].Normalize();
            }
            Current = Settings.lastSlot >= 0 && Settings.lastSlot < slots.Length ? slots[Settings.lastSlot] : null;
            CurrentChanged?.Invoke();
        }

        /// <summary>Creates a fresh profile in <paramref name="slot"/> (overwriting it) and selects it.</summary>
        public PlayerProfile CreatePlayer(int slot, string playerName)
        {
            var p = new PlayerProfile { name = playerName, slot = slot };
            slots[slot] = p;
            Save(p);
            Select(slot);
            return p;
        }

        public void Select(int slot)
        {
            Current = slots[slot];
            Settings.lastSlot = Current != null ? slot : -1;
            SaveSettings();
            CurrentChanged?.Invoke();
        }

        public void Delete(int slot)
        {
            slots[slot] = null;
            if (File.Exists(SlotPath(slot))) File.Delete(SlotPath(slot));
            if (Settings.lastSlot != slot) return;
            Settings.lastSlot = -1;
            Current = null;
            SaveSettings();
            CurrentChanged?.Invoke();
        }

        /// <summary>Writes the current player (or <paramref name="profile"/>) to disk.</summary>
        public void Save(PlayerProfile profile = null)
        {
            profile = profile ?? Current;
            if (profile != null) Write(SlotPath(profile.slot), profile);
        }

        public void SaveSettings() => Write(SettingsPath, Settings);

        void Write(string path, object data)
        {
            try
            {
                Directory.CreateDirectory(folder);
                var tmp = path + ".tmp";
                File.WriteAllText(tmp, JsonUtility.ToJson(data, true));
                if (File.Exists(path)) File.Delete(path);
                File.Move(tmp, path);
            }
            catch (Exception e)
            {
                Debug.LogError($"[Rory] Could not save {path}: {e.Message}");
            }
        }

        static T Read<T>(string path) where T : class
        {
            try
            {
                return File.Exists(path) ? JsonUtility.FromJson<T>(File.ReadAllText(path)) : null;
            }
            catch (Exception e)
            {
                Debug.LogError($"[Rory] Corrupt save {path}: {e.Message}");
                return null;
            }
        }
    }
}
