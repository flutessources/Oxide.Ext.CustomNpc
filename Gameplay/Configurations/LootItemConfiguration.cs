using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oxide.Ext.CustomNpc.Gameplay.Configurations
{
    public class LootItemConfiguration
    {
        public string ItemShortname;
        public ulong SkinId;
        public int QuantityMin;
        public int QuantityMax;
        public int ChanceToLoot;
    }
}
