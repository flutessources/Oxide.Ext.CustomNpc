using Oxide.Ext.CustomNpc.Gameplay.Components;
using System;
using System.Collections.Generic;
using static BaseAIBrain;
using UnityEngine;
using Oxide.Ext.CustomNpc.Gameplay.Managers;
using Oxide.Core;
using System.ComponentModel;

namespace Oxide.Ext.CustomNpc.Gameplay.Controllers
{
    public class CustomNpcBrain_Controller
    {
        public CustomNpcBrain_Component Component { get; private set; }
        private CustomNpc_Controller m_npc;
        public CustomNpc_Controller Npc => m_npc;

        private bool m_isInit;
        public virtual void Initialize(CustomNpc_Controller npc, CustomNpcBrain_Component component)
        {
            Component = component;
            m_npc = npc;

            component.Setup(this);

            Component.onAddStates += AddStates;
            Component.onInitializeAI += InitializeAI;
            Component.onThink += Think;

            m_isInit = true;
        }

        protected virtual void AddStates()
        {
            if (m_isInit == false)
                return;

            Component.states = new Dictionary<AIState, BasicAIState>();

            foreach (var stateName in m_npc.Configuration.States)
            {
                CustomNpc_Manager.IStatesFactory states = null;
                if (CustomNpc_Manager.States.TryGetValue(stateName, out states) == false)
                    continue;

                var state = states.Get();
                state.Setup(m_npc);
                Component.AddState(state);
            }

            if (Component.states.Count == 0)
            {
                Interface.Oxide.LogWarning("Not state added for this npc");
            }
        }

        protected virtual void InitializeAI()
        {
            if (m_isInit == false)
            {
                Interface.Oxide.LogError("[CustomNpc] InitializeAI brain without initialization !!");
                return;
            }

            m_npc.Component.HasBrain = true;
            Component.Navigator = m_npc.GameObject.GetComponent<BaseNavigator>();
            Component.Navigator.Speed = m_npc.Configuration.Speed;
            Component.InvokeRandomized(Component.DoMovementTick, 1f, 0.1f, 0.01f);

            Component.AttackRangeMultiplier = m_npc.Configuration.AttackRangeMultiplier;
            Component.MemoryDuration = m_npc.Configuration.MemoryDuration;
            Component.SenseRange = Math.Abs(m_npc.Configuration.SenseRange);
            Component.TargetLostRange = Component.SenseRange * 2f;
            Component.VisionCone = Vector3.Dot(Vector3.forward, Quaternion.Euler(0f, m_npc.Configuration.VisionCone, 0f) * Vector3.forward);
            Component.CheckVisionCone = m_npc.Configuration.CheckVisionCone;
            Component.CheckLOS = true;
            Component.IgnoreNonVisionSneakers = true;
            Component.MaxGroupSize = 0;
            Component.ListenRange = m_npc.Configuration.ListenRange;
            Component.HostileTargetsOnly = m_npc.Configuration.SenseRange < 0f;
            Component.IgnoreSafeZonePlayers = !Component.HostileTargetsOnly;
            Component.SenseTypes = EntityType.Player;
            Component.RefreshKnownLOS = false;
            Component.IgnoreNonVisionMaxDistance = Component.ListenRange / 3f;
            Component.IgnoreSneakersMaxDistance = Component.IgnoreNonVisionMaxDistance / 3f;
            Component.Senses.Init(
                m_npc.Component, Component, Component.MemoryDuration,
                Component.SenseRange, Component.TargetLostRange,
                Component.VisionCone, Component.CheckVisionCone,
                Component.CheckLOS, Component.IgnoreNonVisionSneakers,
                Component.ListenRange, Component.HostileTargetsOnly, false,
                Component.IgnoreSafeZonePlayers, Component.SenseTypes, Component.RefreshKnownLOS);

            Component.ThinkMode = AIThinkMode.Interval;
            Component.SetThinkRate(0.5f);
            Component.PathFinder = new HumanPathFinder();
            ((HumanPathFinder)Component.PathFinder).Init(m_npc.Component);
        }

        protected virtual void Think(float delta)
        {
            if (m_isInit == false)
                return;

            if (m_npc == null)
                return;

            if (Component == null)
                return;

            if (Component.Senses == null)
                return;

            if (Component.states == null)
                return;


            Component.SetLastThinkTime(Time.time);
            if (Component.sleeping) return;

            Component.Senses.Update();
            m_npc.SetBestTarget();


            if (Component.CurrentState != null)
            {
                var thinkResult = Component.CurrentState.StateThink(delta, Component, m_npc.Component);
                switch (thinkResult)
                {
                    case StateStatus.Finished:
                    case StateStatus.Error:
                        Component.CurrentState.StateLeave(Component, m_npc.Component);
                        Component.CurrentState = null;
                        break;
                    case StateStatus.Running:
                        if (Component.CurrentState.CanLeave() == false)
                        {
                            return;
                        }
                        break;
                }

            }

            float num = 0f;
            BasicAIState newState = null;
            foreach (BasicAIState value in Component.states.Values)
            {
                if (value == null || value.CanEnter() == false) continue;
                float weight = value.GetWeight();
                if (weight < num) continue;
                num = weight;
                newState = value;
            }
            if (newState != Component.CurrentState)
            {
                Component.CurrentState?.StateLeave(Component, m_npc.Component);
                Component.CurrentState = newState;
                Component.CurrentState?.StateEnter(Component, m_npc.Component);
            }
        }

    }
}
