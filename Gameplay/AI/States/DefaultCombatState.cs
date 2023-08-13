namespace Oxide.Ext.CustomNpc.Gameplay.AI.States
{
    internal class DefaultCombatState : CustomAIState
    {
        private float m_nextStrafeTime;

        public DefaultCombatState() : base(AIState.Combat)
        {
        }

        public override float GetWeight() => 75f;

        public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
        {
            base.StateLeave(brain, entity);

            m_npc.Component.SetDucked(false);
            brain.Navigator.ClearFacingDirectionOverride();
        }

        public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
        {
            base.StateThink(delta, brain, entity);

            if (m_npc.Component.CurrentTarget == null)
                return StateStatus.Error;

            brain.Navigator.SetFacingDirectionEntity(m_npc.Component.CurrentTarget);

            var currentWeapon = m_npc.Component.CurrentWeapon;
            var isTimeToStrafe = UnityEngine.Time.time > m_nextStrafeTime;

            if (currentWeapon is BaseProjectile && isTimeToStrafe)
            {
                HandleBaseProjectileActions(currentWeapon);
            }
            else if (currentWeapon is FlameThrower)
            {
                HandleFlameThrowerActions();
            }
            else if (currentWeapon is BaseMelee)
            {
                HandleBaseMeleeActions();
            }

            return StateStatus.Running;
        }

        private void HandleBaseProjectileActions(AttackEntity currentWeapon)
        {
            float deltaTime;
            if (UnityEngine.Random.Range(0, 3) == 1)
            {
                deltaTime = currentWeapon is BaseLauncher ? UnityEngine.Random.Range(0.5f, 1f) : UnityEngine.Random.Range(1f, 2f);
                m_nextStrafeTime = UnityEngine.Time.time + deltaTime;
                m_npc.Component.SetDucked(true);
                brain.Navigator.Stop();
            }
            else
            {
                deltaTime = currentWeapon is BaseLauncher ? UnityEngine.Random.Range(1f, 1.5f) : UnityEngine.Random.Range(2f, 3f);
                m_nextStrafeTime = UnityEngine.Time.time + deltaTime;
                m_npc.Component.SetDucked(false);
                m_npc.Pathfinding.SetDestination(m_npc.Pathfinding.GetRandomPositionAround(m_npc.GameObject.transform.position, 2f), 2f, BaseNavigator.NavigationSpeed.Normal);
            }

            if (currentWeapon is BaseLauncher)
            {
                m_npc.FireGrenadeLauncher();
            }
            else
            {
                m_npc.Component.ShotTest(m_npc.DistanceToTarget);
            }
        }

        private void HandleFlameThrowerActions()
        {
            if (m_npc.DistanceToTarget < m_npc.Component.CurrentWeapon.effectiveRange)
            {
                m_npc.FireFlameThrower();
            }
            m_npc.Pathfinding.SetDestination(m_npc.Component.CurrentTarget.transform.position, 2f, BaseNavigator.NavigationSpeed.Fast);
        }

        private void HandleBaseMeleeActions()
        {
            if (m_npc.DistanceToTarget < m_npc.Component.CurrentWeapon.effectiveRange * 2f)
            {
                m_npc.UseMeleeWeapon();
            }
            m_npc.Pathfinding.SetDestination(m_npc.Component.CurrentTarget.transform.position, 2f, BaseNavigator.NavigationSpeed.Fast);
        }

        public override bool CanEnter()
        {
            base.CanEnter();

            if (m_npc.Component.IsEquipingWeapon) return false;
            if (m_npc.Component.CurrentWeapon == null) return false;
            if (m_npc.Component.CurrentWeapon.ShortPrefabName == "mgl.entity" && m_npc.Component.IsReloadGrenadeLauncher) return false;
            if (m_npc.Component.CurrentWeapon is FlameThrower && m_npc.Component.IsReloadFlameThrower) return false;
            if (m_npc.DistanceFromHome > m_npc.Configuration.ChaseRange) return false;
            if (m_npc.Component.CurrentTarget == null) return false;
            if (m_npc.DistanceToTarget > m_npc.Component.EngagementRange()) return false;
            if (!m_npc.Component.CanSeeTarget(m_npc.Component.CurrentTarget)) return false;
            if (m_npc.IsBehindBarricade()) return false;
            return true;
        }
    }

}
