using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oxide.Ext.CustomNpc.Gameplay.Configurations
{
    public class LootTableConfiguration
    {
        public List<LootItemConfiguration> Items = new List<LootItemConfiguration>();
        public int MaxItems;
    }
}
