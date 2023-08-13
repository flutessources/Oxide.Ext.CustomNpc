using System.Collections.Generic;

namespace Oxide.Ext.CustomNpc.Gameplay.Configurations
{
    public class CustomNpc_BeltItem
    {
        public string ShortName;
        public ulong SkinId;
        public int Amount;
        public List<string> Mods = new List<string>();
        public string Ammo;
    }
}
