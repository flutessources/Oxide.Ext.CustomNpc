namespace Oxide.Ext.CustomNpc.Gameplay.AI.States
{
    public class DefaultCombatStationaryState : CustomAIState
    {
        private float m_nextStrafeTime;

        public DefaultCombatStationaryState() : base(AIState.CombatStationary)
        {
        }

        public override float GetWeight() => 80f;

        public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
        {
            base.StateLeave(brain, entity);
            ResetState();
        }

        private void ResetState()
        {
            m_nextStrafeTime = 0.0f;
            if (!m_npc.Component.IsMounted())
                m_npc.Component.SetDucked(false);
            brain.Navigator.ClearFacingDirectionOverride();
        }

        public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
        {
            base.StateThink(delta, brain, entity);

            if (m_npc.Component.CurrentTarget == null)
                return StateStatus.Error;

            brain.Navigator.SetFacingDirectionEntity(m_npc.Component.CurrentTarget);
            HandleWeaponActions();

            return StateStatus.Running;
        }

        private void HandleWeaponActions()
        {
            if (m_npc.Component.CurrentWeapon is BaseProjectile && UnityEngine.Time.time > m_nextStrafeTime)
            {
                ProcessProjectileWeaponActions();
            }
            else if (m_npc.Component.CurrentWeapon is FlameThrower && m_npc.DistanceToTarget < m_npc.Component.CurrentWeapon.effectiveRange)
            {
                m_npc.FireFlameThrower();
            }
            else if (m_npc.Component.CurrentWeapon is BaseMelee && m_npc.DistanceToTarget < m_npc.Component.CurrentWeapon.effectiveRange * 2f)
            {
                m_npc.UseMeleeWeapon();
            }
        }

        private void ProcessProjectileWeaponActions()
        {
            float deltaTime = DetermineDeltaTime();
            m_nextStrafeTime = UnityEngine.Time.time + deltaTime;

            if (!m_npc.Component.IsMounted())
            {
                bool shouldDuck = UnityEngine.Random.Range(0, 3) == 1;
                m_npc.Component.SetDucked(shouldDuck);
            }

            if (m_npc.Component.CurrentWeapon is BaseLauncher)
            {
                m_npc.FireGrenadeLauncher();
            }
            else
            {
                m_npc.Component.ShotTest(m_npc.DistanceToTarget);
            }
        }

        private float DetermineDeltaTime()
        {
            if (m_npc.Component.CurrentWeapon is BaseLauncher)
            {
                return UnityEngine.Random.Range(0.5f, 1.5f);
            }
            return UnityEngine.Random.Range(1f, 3f);
        }

        public override bool CanEnter()
        {
            if (m_npc.Component.CurrentWeapon == null || m_npc.Component.CurrentTarget == null)
                return false;

            bool hasSpecificWeaponConditions =
                (m_npc.Component.CurrentWeapon.ShortPrefabName == "mgl.entity" && m_npc.Component.IsReloadGrenadeLauncher) ||
                (m_npc.Component.CurrentWeapon is FlameThrower && m_npc.Component.IsReloadFlameThrower);

            if (hasSpecificWeaponConditions)
                return false;

            if (m_npc.DistanceToTarget > m_npc.Component.EngagementRange() || !m_npc.Component.CanSeeTarget(m_npc.Component.CurrentTarget) || m_npc.IsBehindBarricade())
                return false;

            return true;
        }
    }
}
