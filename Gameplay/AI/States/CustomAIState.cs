using Oxide.Ext.CustomNpc.Gameplay.Controllers;
using static BaseAIBrain;

namespace Oxide.Ext.CustomNpc.Gameplay.AI.States
{
    public abstract class CustomAIState : BasicAIState
    {
        protected CustomNpc_Controller m_npc;

        public CustomAIState(AIState state) : base(state)
        {

        }

        public virtual void Setup(CustomNpc_Controller npc)
        {
            m_npc = npc;
        }

        public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
        {
            //Interface.Oxide.LogInfo("State Enter : " + this.GetType().ToString());
        }

        public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
        {
            //Interface.Oxide.LogInfo("State Leave : " + this.GetType().ToString());
        }

        public override bool CanEnter()
        {
            return true;
        }

        public override bool CanInterrupt()
        {
            return true;
        }

        public override bool CanLeave()
        {
            return true;
        }

        public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
        {
            return StateStatus.Running;
        }

        public virtual void Clear()
        {
            m_npc = null;
        }
    }
}
