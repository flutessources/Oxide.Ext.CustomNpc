namespace Oxide.Ext.CustomNpc.Gameplay.AI.States
{
    public class DefaultRoamState : CustomAIState
    {
        public DefaultRoamState() : base(AIState.Roam)
        {
        }

        public override float GetWeight() => 25f;

        public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
        {
            base.StateThink(delta, brain, entity);

            if (m_npc.DistanceFromHome > m_npc.Configuration.RoamRange) m_npc.Pathfinding.SetDestination(m_npc.Component.HomePosition, 2f, BaseNavigator.NavigationSpeed.Fast);
            else if (!brain.Navigator.Moving && m_npc.Configuration.RoamRange > 2f) m_npc.Pathfinding.SetDestination(m_npc.Pathfinding.GetRandomPositionAround(m_npc.Component.HomePosition, m_npc.Configuration.RoamRange - 2f), 2f, BaseNavigator.NavigationSpeed.Slowest);
            return StateStatus.Running;
        }
    }
}
