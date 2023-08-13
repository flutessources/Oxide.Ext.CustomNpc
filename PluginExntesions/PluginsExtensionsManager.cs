using Oxide.Core;
using Oxide.Core.Plugins;
using System;
using System.Collections.Generic;

namespace Oxide.Ext.CustomNpc.PluginExntesions
{
    public class PluginsExtensionsManager : IDisposable
    {
        public static PluginsExtensionsManager Instance { get; private set; }

        public IReadOnlyDictionary<string, Plugin> LoadedPlugins => m_loadedPlugins;

        private Dictionary<string, Plugin> m_loadedPlugins = new Dictionary<string, Plugin>();
        private readonly List<string> m_neededPlugins = new List<string>()
        {
            "Kits"
        };


        public PluginsExtensionsManager()
        {
            if (Instance != null)
                return;

            Instance = this;

            RegisterPlugins();
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            Interface.Oxide.RootPluginManager.OnPluginAdded += OnPluginAdded;
            Interface.Oxide.RootPluginManager.OnPluginAdded -= OnPluginRemoved;
        }
        
        private void UnregisterEvents()
        {
            Interface.Oxide.RootPluginManager.OnPluginAdded -= OnPluginAdded;
            Interface.Oxide.RootPluginManager.OnPluginAdded -= OnPluginRemoved;
        }

        private void RegisterPlugins()
        {
            foreach (var pluginName in m_neededPlugins)
            {
                var plugin = Interface.Oxide.RootPluginManager.GetPlugin(pluginName);
                if (plugin == null)
                    continue;

                m_loadedPlugins.Add(pluginName, plugin);
            }
        }

        private void OnPluginAdded(Plugin plugin)
        {
            if (m_neededPlugins.Contains(plugin.Name) == false)
                return;

            if (m_loadedPlugins.ContainsKey(plugin.Name))
                return;

            m_loadedPlugins.Add(plugin.Name, plugin);
        }

        private void OnPluginRemoved(Plugin plugin)
        {
            if (m_neededPlugins.Contains(plugin.Name) == false)
                return;

            if (m_loadedPlugins.ContainsKey(plugin.Name) == false)
                return;

            m_loadedPlugins.Remove(plugin.Name);
        }

        private bool m_isDisposed;
        public void Dispose()
        {
            if (m_isDisposed)
            {
                Interface.Oxide.LogWarning($"attempt to dispose already disposed {nameof(PluginsExtensionsManager)} ");
                return;
            }

            UnregisterEvents();

            m_isDisposed = true;
        }
    }
}
