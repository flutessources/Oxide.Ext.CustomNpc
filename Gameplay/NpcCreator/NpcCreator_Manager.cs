using Oxide.Core;
using Oxide.Core.Plugins;
using Oxide.Ext.CustomNpc.Gameplay.Components;
using Oxide.Ext.CustomNpc.Gameplay.Configurations;
using System.Collections.Generic;
using System.IO;


namespace Oxide.Ext.CustomNpc.Gameplay.NpcCreator
{
    public static class NpcCreator_Manager
    {
        public const string NPC_FILE_BASE = "npc_";

        private static Dictionary<ulong, NpcCreator_Controller> m_controllers = new Dictionary<ulong, NpcCreator_Controller>();
        public static IReadOnlyDictionary<ulong, NpcCreator_Controller> Controllers => m_controllers;

        private static Dictionary<string, CustomNpc_Configuration> m_npcConfigurations = new Dictionary<string, CustomNpc_Configuration>();
        public static IReadOnlyDictionary<string, CustomNpc_Configuration> NpcConfigurations => m_npcConfigurations;

        private static Dictionary<string, NpcCreator_NpcController> m_editedNpcs = new Dictionary<string, NpcCreator_NpcController>();
        public static IReadOnlyDictionary<string, NpcCreator_NpcController> EditesNpcs => m_editedNpcs;
        public static Plugin Plugin { get; private set; }

        public static bool IsStarted = false;

        public static bool StartCreator(BasePlayer player, Plugin plugin)
        {
            if (Plugin != plugin || plugin == null)
            {
                if (Plugin != null)
                    Stop();

                Plugin = plugin;
                StartNew();
            }

            if (m_controllers.ContainsKey(player.userID))
                return false;

            var creator = new NpcCreator_Controller(player);
            m_controllers.Add(player.userID, creator);

            creator.onAddNpc += OnNpcAdded;
            creator.onRemoveNpc += OnNpcRemoved;

            IsStarted = true;
            return true;
        }

        private static void StartNew()
        {
            LoadConfigs();
        }

        public static bool StopCreator(BasePlayer player)
        {
            if (m_controllers.ContainsKey(player.userID) == false)
                return false;

            var controller = m_controllers[player.userID];

            controller.Stop();
            m_controllers.Remove(player.userID);

            controller.onAddNpc -= OnNpcAdded;
            controller.onRemoveNpc -= OnNpcRemoved;

            if (m_controllers.Count == 0)
                IsStarted = false;

            return true;
        }

        public static void Stop()
        {
            foreach(var controller in m_controllers.Values)
            {
                controller.Stop();

                controller.onAddNpc -= OnNpcAdded;
                controller.onRemoveNpc -= OnNpcRemoved;
            }

            IsStarted = false;
            m_controllers.Clear();
            m_editedNpcs.Clear();
            m_npcConfigurations.Clear();

        }

        public static void OnEntityKill(ScientistNPC npcComponent)
        {
            var brain = npcComponent.Brain as NpcCreator_BrainComponent;
            if (brain == null)
                return;

            if (m_editedNpcs.ContainsKey(npcComponent.displayName) == false)
                return;

            var editedNpc = m_editedNpcs[npcComponent.displayName];
            editedNpc.OnKill();
        }

        private static void OnNpcAdded(NpcCreator_NpcController npc)
        {
            m_editedNpcs.Add(npc.Name, npc);
        }

        private static void OnNpcRemoved(string npcName)
        {
            m_editedNpcs.Remove(npcName);
        }

        public static void AddNpcConfiguration(string name, CustomNpc_Configuration config)
        {
            if (m_npcConfigurations.ContainsKey(name))
                return;

            m_npcConfigurations.Add(name, config);
        }

        public static void RemoveNpcConfiguration(string name)
        {
            if (m_npcConfigurations.ContainsKey(name) == false)
                return;

            m_npcConfigurations.Remove(name);
        }

        public static void Save()
        {
            foreach(var controller in m_controllers)
            {
                controller.Value.SaveAllNpcConfig();
            }
        }

        public static void LoadConfigs()
        {
            if (Directory.Exists($"{Interface.Oxide.DataFileSystem.Directory}/{Plugin.Name}") == false)
            {
                return;
            }

            var files = Interface.Oxide.DataFileSystem.GetFiles($"{Plugin.Name}");

            foreach (var filePath in files)
            {
                if (filePath.Contains(NPC_FILE_BASE) == false)
                    continue;

                string fileName = Path.GetFileNameWithoutExtension(filePath);
                var file = Interface.Oxide.DataFileSystem.GetFile($"{Plugin.Name}/{fileName}");

                if (file == null)
                {
                    Interface.Oxide.LogWarning($"[CustomNpc] Imposible to load config {fileName} for plugin {Plugin.Name}");
                }
                else
                {
                    Interface.Oxide.LogInfo($"[CustomNpc] Config {fileName} for plugin {Plugin.Name} loaded");
                }

                var config = file.ReadObject<CustomNpc_Configuration>();

                AddNpcConfiguration(fileName.Replace(NPC_FILE_BASE, ""), config);
            }
        }

        public static void ReloadConfig(string name)
        {
            if (m_npcConfigurations.ContainsKey(name) == false)
                return;

            var fileName = NpcCreator_Manager.NPC_FILE_BASE + name;

            var file = Interface.Oxide.DataFileSystem.GetFile($"{Plugin.Name}/{fileName}");
            var config = file.ReadObject<CustomNpc_Configuration>();

            m_npcConfigurations[name] = config;
        }
    }
}
