using UnityEngine;

namespace Oxide.Ext.CustomNpc.Gameplay.AI.States
{
    internal class DefaultMoveTowardsState : CustomAIState
    {
        public DefaultMoveTowardsState()
            : base(AIState.MoveTowards)
        {
        }

        public override float GetWeight() => 50.0f;

        public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
        {
            base.StateLeave(brain, entity);
            Stop();
        }

        private void Stop()
        {
            brain.Navigator.Stop();
        }

        public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
        {
            base.StateThink(delta, brain, entity);

            BaseEntity targetEntity = GetMemoryTargetEntity();
            if (targetEntity == null)
            {
                Stop();
                return StateStatus.Error;
            }

            FaceTarget();

            if (!SetNavigatorDestination(targetEntity.transform.position))
            {
                return StateStatus.Error;
            }

            return brain.Navigator.Moving ? StateStatus.Running : StateStatus.Finished;
        }

        private BaseEntity GetMemoryTargetEntity()
        {
            return brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
        }

        private bool SetNavigatorDestination(Vector3 destination)
        {
            return brain.Navigator.SetDestination(destination, brain.Navigator.MoveTowardsSpeed, 0.25f);
        }

        private void FaceTarget()
        {
            if (!brain.Navigator.FaceMoveTowardsTarget) return;

            BaseEntity targetEntity = GetMemoryTargetEntity();
            if (targetEntity == null)
            {
                brain.Navigator.ClearFacingDirectionOverride();
                return;
            }

            if (IsCloseToTarget(targetEntity.transform.position))
            {
                brain.Navigator.SetFacingDirectionEntity(targetEntity);
            }
        }

        private bool IsCloseToTarget(Vector3 targetPosition)
        {
            return Vector3.Distance(targetPosition, brain.transform.position) <= 1.5f;
        }



    }
}
