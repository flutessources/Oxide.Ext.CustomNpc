using Oxide.Core;
using Oxide.Ext.CustomNpc.Gameplay.Components;
using Oxide.Ext.CustomNpc.Gameplay.Configurations;
using Oxide.Ext.CustomNpc.Gameplay.Controllers;
using Oxide.Ext.CustomNpc.Gameplay.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Oxide.Ext.CustomNpc.Gameplay.Managers
{
    internal class NpcInstantiationFactory
    {
        // Tous les types par défaut
        public static CustomNpc_Entity InstanceNpcDefault(Vector3 position, CustomNpc_Configuration configuration)
        {
            return InstanceNpc<CustomNpc_Controller, CustomNpcBrain_Controller, CustomNpc_Component, CustomNpcBrain_Component>(position, configuration);
        }

        // NpcController custom
        public static CustomNpc_Entity InstanceNpcWithCustomController<TnpcController>(Vector3 position, CustomNpc_Configuration configuration)
            where TnpcController : CustomNpc_Controller, new()
        {
            return InstanceNpc<TnpcController, CustomNpcBrain_Controller, CustomNpc_Component, CustomNpcBrain_Component>(position, configuration);
        }

        // BrainController custom
        public static CustomNpc_Entity InstanceNpcWithCustomBrainController<TBrainController>(Vector3 position, CustomNpc_Configuration configuration)
            where TBrainController : CustomNpcBrain_Controller, new()
        {
            return InstanceNpc<CustomNpc_Controller, TBrainController, CustomNpc_Component, CustomNpcBrain_Component>(position, configuration);
        }

        // NpcComponent custom
        public static CustomNpc_Entity InstanceNpcWithCustomComponent<TNpcComponent>(Vector3 position, CustomNpc_Configuration configuration)
            where TNpcComponent : CustomNpc_Component
        {
            return InstanceNpc<CustomNpc_Controller, CustomNpcBrain_Controller, TNpcComponent, CustomNpcBrain_Component>(position, configuration);
        }

        // BrainComponent custom
        public static CustomNpc_Entity InstanceNpcWithCustomBrainComponent<TBrainComponent>(Vector3 position, CustomNpc_Configuration configuration)
            where TBrainComponent : CustomNpcBrain_Component
        {
            return InstanceNpc<CustomNpc_Controller, CustomNpcBrain_Controller, CustomNpc_Component, TBrainComponent>(position, configuration);
        }

        // NpcController et BrainController custom
        public static CustomNpc_Entity InstanceNpcWithCustomControllers<TnpcController, TBrainController>(Vector3 position, CustomNpc_Configuration configuration)
            where TnpcController : CustomNpc_Controller, new()
            where TBrainController : CustomNpcBrain_Controller, new()
        {
            return InstanceNpc<TnpcController, TBrainController, CustomNpc_Component, CustomNpcBrain_Component>(position, configuration);
        }

        // NpcController et NpcComponent custom
        public static CustomNpc_Entity InstanceNpcWithCustomControllerAndComponent<TnpcController, TNpcComponent>(Vector3 position, CustomNpc_Configuration configuration)
            where TnpcController : CustomNpc_Controller, new()
            where TNpcComponent : CustomNpc_Component
        {
            return InstanceNpc<TnpcController, CustomNpcBrain_Controller, TNpcComponent, CustomNpcBrain_Component>(position, configuration);
        }

        // NpcController et BrainComponent custom
        public static CustomNpc_Entity InstanceNpcWithCustomControllerAndBrainComponent<TnpcController, TBrainComponent>(Vector3 position, CustomNpc_Configuration configuration)
            where TnpcController : CustomNpc_Controller, new()
            where TBrainComponent : CustomNpcBrain_Component
        {
            return InstanceNpc<TnpcController, CustomNpcBrain_Controller, CustomNpc_Component, TBrainComponent>(position, configuration);
        }

        // BrainController et NpcComponent custom
        public static CustomNpc_Entity InstanceNpcWithCustomBrainControllerAndComponent<TBrainController, TNpcComponent>(Vector3 position, CustomNpc_Configuration configuration)
            where TBrainController : CustomNpcBrain_Controller, new()
            where TNpcComponent : CustomNpc_Component
        {
            return InstanceNpc<CustomNpc_Controller, TBrainController, TNpcComponent, CustomNpcBrain_Component>(position, configuration);
        }

        // BrainController et BrainComponent custom
        public static CustomNpc_Entity InstanceNpcWithCustomBrainControllerAndBrainComponent<TBrainController, TBrainComponent>(Vector3 position, CustomNpc_Configuration configuration)
            where TBrainController : CustomNpcBrain_Controller, new()
            where TBrainComponent : CustomNpcBrain_Component
        {
            return InstanceNpc<CustomNpc_Controller, TBrainController, CustomNpc_Component, TBrainComponent>(position, configuration);
        }

        // NpcController, BrainController et NpcComponent custom
        public static CustomNpc_Entity InstanceNpcWithCustomControllerBrainControllerAndComponent<TnpcController, TBrainController, TNpcComponent>(Vector3 position, CustomNpc_Configuration configuration)
            where TnpcController : CustomNpc_Controller, new()
            where TBrainController : CustomNpcBrain_Controller, new()
            where TNpcComponent : CustomNpc_Component
        {
            return InstanceNpc<TnpcController, TBrainController, TNpcComponent, CustomNpcBrain_Component>(position, configuration);
        }

        // NpcController, BrainController et BrainComponent custom
        public static CustomNpc_Entity InstanceNpcWithCustomControllerBrainControllerAndBrainComponent<TnpcController, TBrainController, TBrainComponent>(Vector3 position, CustomNpc_Configuration configuration)
            where TnpcController : CustomNpc_Controller, new()
            where TBrainController : CustomNpcBrain_Controller, new()
            where TBrainComponent : CustomNpcBrain_Component
        {
            return InstanceNpc<TnpcController, TBrainController, CustomNpc_Component, TBrainComponent>(position, configuration);
        }

        // NpcController, NpcComponent et BrainComponent custom
        public static CustomNpc_Entity InstanceNpcWithCustomControllerComponentAndBrainComponent<TnpcController, TNpcComponent, TBrainComponent>(Vector3 position, CustomNpc_Configuration configuration)
            where TnpcController : CustomNpc_Controller, new()
            where TNpcComponent : CustomNpc_Component
            where TBrainComponent : CustomNpcBrain_Component
        {
            return InstanceNpc<TnpcController, CustomNpcBrain_Controller, TNpcComponent, TBrainComponent>(position, configuration);
        }

        // BrainController, NpcComponent et BrainComponent custom
        public static CustomNpc_Entity InstanceNpcWithCustomBrainControllerComponentAndBrainComponent<TBrainController, TNpcComponent, TBrainComponent>(Vector3 position, CustomNpc_Configuration configuration)
            where TBrainController : CustomNpcBrain_Controller, new()
            where TNpcComponent : CustomNpc_Component
            where TBrainComponent : CustomNpcBrain_Component
        {
            return InstanceNpc<CustomNpc_Controller, TBrainController, TNpcComponent, TBrainComponent>(position, configuration);
        }

        private static CustomNpc_Entity InstanceNpc<TnpcController, TBrainController, TNpcComponent, TBrainCoponent>(Vector3 position, CustomNpc_Configuration configuration)
            where TnpcController : CustomNpc_Controller, new()
            where TBrainController : CustomNpcBrain_Controller, new()
            where TNpcComponent : CustomNpc_Component
            where TBrainCoponent : CustomNpcBrain_Component
        {
            LogInstanceNpc(position);

            ScientistNPC scientistNpc = CreateScientistNpc(position);
            if (scientistNpc == null) return null;

            ScientistBrain scientistBrain = GetScientistBrain(scientistNpc);
            if (scientistBrain == null) return null;

            TNpcComponent customNpcComponent = scientistNpc.gameObject.AddComponent<TNpcComponent>();
            TBrainCoponent customBrainComponent = scientistNpc.gameObject.AddComponent<TBrainCoponent>();

            CopySerializableFields(scientistNpc, customNpcComponent);
            CopySerializableFields(scientistBrain, customBrainComponent);

            DestroyObject(scientistNpc);
            DestroyObject(scientistBrain);

            TnpcController customNpc = InitializeNpc<TnpcController>(customNpcComponent, configuration);
            TBrainController brain = InitializeBrain<TBrainController>(customNpc, customBrainComponent);

            return CustomNpc_Manager.CreateAndStartEntity(customNpcComponent, customNpc, brain);
        }

        private static void LogInstanceNpc(Vector3 position)
        {
            Interface.Oxide.LogInfo($"[CustomNpc] InstanceNpc at {position.ToString()} position ...");
        }

        private static void CopySerializableFields<T>(T src, T dst)
        {
            FieldInfo[] srcFields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo field in srcFields)
            {
                object value = field.GetValue(src);
                field.SetValue(dst, value);
            }
        }

        private static ScientistNPC CreateScientistNpc(Vector3 position)
        {
            ScientistNPC scientistNpc = GameManager.server.CreateEntity("assets/rust.ai/agents/npcplayer/humannpc/scientist/scientistnpc_heavy.prefab", position, Quaternion.identity, false) as ScientistNPC;
            if (scientistNpc == null)
            {
                Interface.Oxide.LogInfo("[CustomNpc] Scientist npc is null");
            }
            return scientistNpc;
        }

        private static ScientistBrain GetScientistBrain(ScientistNPC scientistNpc)
        {
            ScientistBrain scientistBrain = scientistNpc.GetComponent<ScientistBrain>();
            if (scientistBrain == null)
            {
                Interface.Oxide.LogInfo("[CustomNpc] Scientist npc brain is null");
            }
            return scientistBrain;
        }

        private static void DestroyObject(UnityEngine.Object obj)
        {
            UnityEngine.Object.DestroyImmediate(obj, true);
        }

        private static TController InitializeNpc<TController>(CustomNpc_Component component, CustomNpc_Configuration configuration) where TController : CustomNpc_Controller, new()
        {
            TController controller = new TController();
            controller.Initialize(component, configuration);
            return controller;
        }

        private static TController InitializeBrain<TController>(CustomNpc_Controller npc, CustomNpcBrain_Component component) where TController : CustomNpcBrain_Controller, new()
        {
            TController controller = new TController();
            controller.Initialize(npc, component);
            return controller;
        }
    }
}
