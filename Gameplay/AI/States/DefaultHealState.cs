using Oxide.Core;
using System.Linq;

namespace Oxide.Ext.CustomNpc.Gameplay.AI.States
{
    internal class DefaultHealState : CustomAIState
    {
        public DefaultHealState() : base(AIState.Cooldown)
        {
        }

        public override float GetWeight() => 100;

        public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
        {
            base.StateThink(delta, brain, entity);

            if (m_npc.Component.IsHealing)
            {
                return StateStatus.Running;
            }
            else
            {
                if (CanEnter() == false)
                {
                    m_npc.Heal();
                    return StateStatus.Running;
                }
            }

            return StateStatus.Finished;
        }

        public override bool CanLeave()
        {
            base.CanLeave();

            return CanEnter() == false;
        }

        public override bool CanEnter()
        {
            base.CanEnter();

            if (m_npc.Component.IsHealing || m_npc.Component.health >= m_npc.Configuration.MaxHealth|| m_npc.Component.IsEquipingWeapon) return false;
            return m_npc.Component.inventory.containerBelt.itemList.Any(x => x.info.shortname == "syringe.medical");
        }

    }
}
