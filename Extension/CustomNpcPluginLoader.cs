using Oxide.Core.Plugins;
using Oxide.Game.Rust;
using System;

namespace Oxide.Ext.CustomNpc
{
    public class CustomNpcPluginLoader : PluginLoader
    {
        public override Type[] CorePlugins => new Type[] { typeof(CustomNpcPlugin) };
    }
}
