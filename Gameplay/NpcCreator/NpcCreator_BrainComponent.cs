using Oxide.Core;
using Oxide.Ext.CustomNpc.Gameplay.AI.States;
using Oxide.Ext.CustomNpc.Gameplay.Components;

using System.Collections.Generic;
using UnityEngine;

namespace Oxide.Ext.CustomNpc.Gameplay.NpcCreator
{
    public class NpcCreator_BrainComponent : CustomNpcBrain_Component
    {
        private Vector3 m_beforeTestPosition;

        public override void AddStates()
        {
            states = new Dictionary<AIState, BasicAIState>();
            DefaultIdleState idleState = new DefaultIdleState();
            idleState.Setup(m_controller.Npc);
        }

        public void StartTest()
        {
            m_beforeTestPosition = baseEntity.ServerPosition;
            base.AddStates();
        }

        public void StopTest()
        {
            CurrentState?.StateLeave(this, baseEntity);
            AddStates();

            m_controller.Npc.Pathfinding.SetDestination(m_beforeTestPosition, 2f, BaseNavigator.NavigationSpeed.Fast);
        }
    }
}
