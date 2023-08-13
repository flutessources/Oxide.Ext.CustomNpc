namespace Oxide.Ext.CustomNpc.Gameplay.AI.States
{
    public class DefaultChaseState : CustomAIState
    {
        public DefaultChaseState() : base(AIState.Chase)
        {

        }

        public override float GetWeight() => 50.0f;

        public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
        {
            base.StateThink(delta, brain, entity);

            if (m_npc.Component.CurrentTarget == null)
                return StateStatus.Error;

            SetNpcDestination();

            return StateStatus.Running;
        }

        private void SetNpcDestination()
        {
            var navigationSpeed = BaseNavigator.NavigationSpeed.Fast;

            if (m_npc.Component.CurrentWeapon is BaseProjectile)
            {
                navigationSpeed = m_npc.DistanceToTarget > 10f ? BaseNavigator.NavigationSpeed.Fast : BaseNavigator.NavigationSpeed.Normal;
            }

            m_npc.Pathfinding.SetDestination(m_npc.Component.CurrentTarget.transform.position, 2f, navigationSpeed);
        }

        public override bool CanEnter()
        {
            base.CanEnter();

            if (m_npc.DistanceFromHome > m_npc.Configuration.ChaseRange)
                return false;

            if (m_npc.Component.CurrentTarget == null)
                return false;

            return true;
        }

        public override bool CanLeave()
        {
            return true;
        }
    }
}
