using Oxide.Ext.CustomNpc.PluginExntesions;
namespace Oxide.Ext.CustomNpc.Gameplay.Controllers.NpcAttire
{
    public class NpcAttire_Kits_Controller : NpcAttire_Controller_Base
    {
        public const string PLUGIN_NAME = "Kits";

        private string m_kitName;
        private CustomNpc_Controller m_npc;

        public NpcAttire_Kits_Controller(CustomNpc_Controller npc, string kitName)
        {
            m_kitName = kitName;
            m_npc = npc;
        }

        public override void Equip()
        {
            PluginsExtensionsManager.Instance.LoadedPlugins[PLUGIN_NAME]
                .Call("GiveKit", m_npc.Component, m_kitName);
        }
    }
}
