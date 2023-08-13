using Oxide.Ext.CustomNpc.Gameplay.Configurations;

using System.Collections.Generic;
using System.Linq;

namespace Oxide.Ext.CustomNpc.Gameplay.Controllers.NpcAttire
{
    public class NpcAttire_Default_Controller : NpcAttire_Controller_Base
    {
        private List<CustomNpc_BeltItem> m_belt;
        private List<CustomNpc_WearItem> m_wear;
        private PlayerInventory m_inventory;

        public NpcAttire_Default_Controller(PlayerInventory inventory, List<CustomNpc_BeltItem> belt, List<CustomNpc_WearItem> wear)
        {
            m_belt = belt;
            m_wear = wear;
            m_inventory = inventory;
        }

        public override void Equip()
        {
            if (m_wear.Count > 0)
            {
                foreach (Item item in m_wear.Select(x => ItemManager.CreateByName(x.ShortName, 1, x.SkinId)))
                {
                    if (item == null) continue;
                    if (!item.MoveToContainer(m_inventory.containerWear)) item.Remove();
                }
            }
            if (m_belt.Count > 0)
            {
                foreach (var npcItem in m_belt)
                {
                    Item item = ItemManager.CreateByName(npcItem.ShortName, npcItem.Amount, npcItem.SkinId);
                    if (item == null) continue;
                    foreach (ItemDefinition itemDefinition in npcItem.Mods.Select(ItemManager.FindItemDefinition)) if (itemDefinition != null) item.contents.AddItem(itemDefinition, 1);
                    if (!item.MoveToContainer(m_inventory.containerBelt)) item.Remove();
                }
            }
        }

    }
}
