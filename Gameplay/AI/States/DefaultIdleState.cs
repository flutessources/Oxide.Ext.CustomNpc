namespace Oxide.Ext.CustomNpc.Gameplay.AI.States
{
    public class DefaultIdleState : CustomAIState
    {
        public DefaultIdleState() : base(AIState.Idle) 
        {
        }
        public override float GetWeight() => 50f;
    }
}
