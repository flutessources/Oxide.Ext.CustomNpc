using Oxide.Core;
using Oxide.Core.Extensions;
using System;


namespace Oxide.Ext.CustomNpc
{
    public class CustomNpcExtension : Extension
    {
        public override string Name => "CustomNpc";
        public override string Author => "Flutes";
        public override VersionNumber Version => new VersionNumber(1, 0, 0);

        public CustomNpcExtension(ExtensionManager manager) : base(manager) { }

        public override void Load()
        {
            base.Load();

            Manager.RegisterPluginLoader(new CustomNpcPluginLoader());
        }

        public override void OnModLoad()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, exception) =>
            {
                Interface.Oxide.LogException("An exception was thrown!", exception.ExceptionObject as Exception);
            };

            Interface.Oxide.LogInfo("Loaded");
            //new PluginsExtensionsManager();
        }
    }
}
