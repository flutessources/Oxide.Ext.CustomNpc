using Oxide.Core;
using Oxide.Ext.CustomNpc.Gameplay.AI.States;
using Oxide.Ext.CustomNpc.Gameplay.Components;
using Oxide.Ext.CustomNpc.Gameplay.Configurations;
using Oxide.Ext.CustomNpc.Gameplay.Controllers;
using Oxide.Ext.CustomNpc.Gameplay.Entities;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


namespace Oxide.Ext.CustomNpc.Gameplay.Managers
{
    public static class CustomNpc_Manager
    {
        private static Dictionary<ulong, CustomNpc_Entity> m_spawnedNpcs = new Dictionary<ulong, CustomNpc_Entity>();
        public static IReadOnlyDictionary<ulong, CustomNpc_Entity> SpawnedNpcs => m_spawnedNpcs;

        private static Dictionary<CustomNpc_Component, CustomNpc_Entity> m_spawnedNpcsByComponent = new Dictionary<CustomNpc_Component, CustomNpc_Entity>();
        public static IReadOnlyDictionary<CustomNpc_Component, CustomNpc_Entity> SpawnedNpcsByComponent => m_spawnedNpcsByComponent;

        private static Dictionary<string, IStatesFactory> m_states = new Dictionary<string, IStatesFactory>();
        public static IReadOnlyDictionary<string, IStatesFactory> States => m_states;

        public static void Setup()
        {
            RegisterDefaultStates();
        }

        public static CustomNpc_Entity InstanceNpcWithCustomComponents<Tnpc, Tbrain>(Vector3 position, CustomNpc_Configuration configuration)
            where Tnpc : CustomNpc_Component
            where Tbrain : CustomNpcBrain_Component
        {
            Interface.Oxide.LogInfo($"InstanceNpc at {position.ToString()} position ...");

            ScientistNPC scientistNpc = GameManager.server.CreateEntity("assets/rust.ai/agents/npcplayer/humannpc/scientist/scientistnpc_heavy.prefab", position, Quaternion.identity, false) as ScientistNPC;

            if (scientistNpc == null)
            {
                Interface.Oxide.LogInfo("Scientist npc is null");
                return null;
            }

            ScientistBrain scientistBrain = scientistNpc.GetComponent<ScientistBrain>();

            if (scientistBrain == null)
            {
                Interface.Oxide.LogInfo("Scientist npc breain is null");
                return null;
            }

            CustomNpc_Component customNpcComponent = scientistNpc.gameObject.AddComponent<Tnpc>();
            CustomNpcBrain_Component customBrainComponent = scientistNpc.gameObject.AddComponent<Tbrain>();

            CopySerializableFields(scientistNpc, customNpcComponent);
            CopySerializableFields(scientistBrain, customBrainComponent);

            UnityEngine.Object.DestroyImmediate(scientistNpc, true);
            UnityEngine.Object.DestroyImmediate(scientistBrain, true);

            CustomNpc_Controller customNpc = new CustomNpc_Controller(customNpcComponent, configuration);
            CustomNpcBrain_Controller brain = new CustomNpcBrain_Controller(customNpc, customBrainComponent);
            CustomNpc_Entity entity = new CustomNpc_Entity(customNpcComponent.gameObject, customNpc);


            entity.Start(brain);
            m_spawnedNpcs.Add(entity.Controller.Component.net.ID.Value, entity);
            m_spawnedNpcsByComponent.Add(customNpcComponent, entity);

            return entity;
        }

        public static CustomNpc_Entity InstanceNpc(Vector3 position, CustomNpc_Configuration configuration)
        {
            Interface.Oxide.LogInfo($"InstanceNpc at {position.ToString()} position ...");

            ScientistNPC scientistNpc = GameManager.server.CreateEntity("assets/rust.ai/agents/npcplayer/humannpc/scientist/scientistnpc_heavy.prefab", position, Quaternion.identity, false) as ScientistNPC;
            
            if (scientistNpc == null)
            {
                Interface.Oxide.LogInfo("Scientist npc is null");
                return null;
            }
            
            ScientistBrain scientistBrain = scientistNpc.GetComponent<ScientistBrain>();

            if (scientistBrain == null)
            {
                Interface.Oxide.LogInfo("Scientist npc breain is null");
                return null;
            }

            CustomNpc_Component customNpcComponent = scientistNpc.gameObject.AddComponent<CustomNpc_Component>();
            CustomNpcBrain_Component customBrainComponent = scientistNpc.gameObject.AddComponent<CustomNpcBrain_Component>();

            CopySerializableFields(scientistNpc, customNpcComponent);
            CopySerializableFields(scientistBrain, customBrainComponent);

            UnityEngine.Object.DestroyImmediate(scientistNpc, true);
            UnityEngine.Object.DestroyImmediate(scientistBrain, true);

            CustomNpc_Controller customNpc = new CustomNpc_Controller(customNpcComponent, configuration);
            CustomNpcBrain_Controller brain = new CustomNpcBrain_Controller(customNpc, customBrainComponent);
            CustomNpc_Entity entity = new CustomNpc_Entity(customNpcComponent.gameObject, customNpc);

            entity.Start(brain);
            m_spawnedNpcs.Add(entity.Controller.Component.net.ID.Value, entity);
            m_spawnedNpcsByComponent.Add(customNpcComponent, entity);

            return entity;
        }

        public static void DestroyAllNpcs()
        {
            foreach(var npc in m_spawnedNpcs)
            {
                m_spawnedNpcsByComponent.Remove(npc.Value.Controller.Component);
                npc.Value.Controller.Component.Kill();
            }

            m_spawnedNpcs.Clear();
        }

        public static void OnNpcDestroyed(CustomNpc_Component component)
        {

            Interface.Oxide.LogInfo($"OnNpcDestroyed1");

            if (component == null)
                return;

            Interface.Oxide.LogInfo($"OnNpcDestroyed2");

            CustomNpc_Entity entity = null;
            if (m_spawnedNpcsByComponent.TryGetValue(component, out entity) == false)
                return;

            Interface.Oxide.LogInfo($"OnNpcDestroyed3");

            m_spawnedNpcs.Remove(entity.Controller.Component.net.ID.Value);
            m_spawnedNpcsByComponent.Remove(component);
        }

        #region States
        public interface IStatesFactory
        {
            CustomAIState Get();
        }

        public class StatesFactory<T> : IStatesFactory where T : CustomAIState, new()
        {      
            public CustomAIState Get()
            {
                return new T();
            }

        }

        public static void RegisterState<T>() where T : CustomAIState, new()
        {
            string name = typeof(T).Name;

            if (m_states.ContainsKey(name))
                return;

            m_states.Add(name, new StatesFactory<T>());
        }

        public static void UnregisterState(Type type)
        {
            string name = type.Name;

            if (m_states.ContainsKey(name) == false)
                return;

            m_states.Remove(name);
        }

        private static void RegisterDefaultStates()
        {
            RegisterState<DefaultRoamState>();
            RegisterState<DefaultChaseState>();
            RegisterState<DefaultCombatState>();
            RegisterState<DefaultIdleState>();
            RegisterState<DefaultCombatStationaryState>();
            RegisterState<DefaultHealState>();
            RegisterState<DefaultHealState>();
            RegisterState<ChangeWeaponState>();
        }
        #endregion

        private static void CopySerializableFields<T>(T src, T dst)
        {
            FieldInfo[] srcFields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo field in srcFields)
            {
                object value = field.GetValue(src);
                field.SetValue(dst, value);
            }
        }
    }
}
