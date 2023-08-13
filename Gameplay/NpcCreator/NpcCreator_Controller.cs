using Oxide.Core;
using Oxide.Ext.CustomNpc.Gameplay.Components;
using Oxide.Ext.CustomNpc.Gameplay.Configurations;
using Oxide.Ext.CustomNpc.Gameplay.Entities;
using Oxide.Ext.CustomNpc.Gameplay.Managers;
using Rust;
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Oxide.Ext.CustomNpc.Gameplay.NpcCreator
{
    public class NpcCreator_Controller
    {
        public const float DISTANCE_SELECT_MAX = 15.0f;

        private Dictionary<ulong, NpcCreator_NpcController> m_npcs = new Dictionary<ulong, NpcCreator_NpcController>();

        public NpcCreator_NpcController SelectedNpc { get; private set; }
        public readonly BasePlayer BasePlayer;
        public Action<NpcCreator_NpcController> onAddNpc;
        public Action<string> onRemoveNpc;

        private bool m_isTest;
        private bool m_isSelectedTest;

        private bool m_onStopping;

        public NpcCreator_Controller(BasePlayer player)
        {
            BasePlayer = player;
        }

        public void Stop()
        {
            m_onStopping = true;

            foreach(var npc in m_npcs.Values)
            {
                KillNpc(npc);
            }

            m_onStopping = false;
        }

        public void KillNpc(NpcCreator_NpcController npc)
        {
            npc.Npc.Controller.Component.Kill();
        }

        public bool InstanceNpc(string name, Vector3 position, out NpcCreator_NpcController npc, bool select = false)
        {
            npc = null;

            if (NpcCreator_Manager.EditesNpcs.ContainsKey(name))
            {
                return false;
            }

            CustomNpc_Entity entity = null;
            CustomNpc_Configuration config = null;

            if (m_npcs.Values != null)
            {
                var alreadyExist = m_npcs.Values.FirstOrDefault(x => x.Name == name);
                if (alreadyExist != null)
                {
                    return false;
                }
            }

            if (NpcCreator_Manager.NpcConfigurations.ContainsKey(name))
            {
                config = NpcCreator_Manager.NpcConfigurations[name];
                Interface.Oxide.LogInfo($"[CustomNpc] Instance npc {name} with configuration");
            }
            else
            {
                config = CustomNpc_Configuration.Default();
                config.Name = name;
            }

            entity = NpcInstantiationFactory.InstanceNpcWithCustomBrainComponent<NpcCreator_BrainComponent>(position, config);
            NpcCreator_NpcController npcCreator = new NpcCreator_NpcController(BasePlayer, name, entity);
            m_npcs.Add(entity.Controller.Component.net.ID.Value, npcCreator);

            if (select)
            {
                SelectNpc(npcCreator);
            }

            npcCreator.onKill += OnNpcKilled;

            npc = npcCreator;
            onAddNpc?.Invoke(npcCreator);
            return true;
        }

        private void OnNpcKilled(NpcCreator_NpcController npcController)
        {
            if (npcController == null)
                return;

            bool selected = false;

            if (SelectedNpc == npcController)
            {
                selected = true;
            }

            m_npcs.Remove(npcController.Npc.Controller.Component.net.ID.Value);
            OnRemoveNpc(npcController);

            if (m_onStopping == false)
            {
                NpcCreator_NpcController newNpc = null;

                if (InstanceNpc(npcController.Name, npcController.StartPosition, out newNpc, selected))
                {
                    if ((selected && m_isSelectedTest) || m_isTest)
                    {
                        newNpc.Npc.Controller.Component.Invoke(newNpc.CreatorBrain.StartTest, 2.0f);
                    }
                }
                else
                {
                    Interface.Oxide.LogInfo("[CustomNpc] Fail to spawn npc");
                }
            }
        }

        private void OnRemoveNpc(NpcCreator_NpcController npcController)
        {
            npcController.onKill -= OnNpcKilled;

            onRemoveNpc?.Invoke(npcController.Name);
        }

        public void InstanceNpcCommand(string[] args)
        {
            if (m_isTest)
                return;

            if (args.Length != 1)
            {
                BasePlayer.ChatMessage("Need 1 arg (name)");
                return;
            }
            
            string name = args[0];
            NpcCreator_NpcController newNpc = null;
            if (InstanceNpc(name, BasePlayer.ServerPosition, out newNpc, true) == false)
            {
                BasePlayer.ChatMessage($"npc {name} already instantiated for edition");
                return;
            }
        }
        
        //public bool RemoveNpcCommand()
        //{
        //    if (m_isTest)
        //        return false;

        //    if (SelectedNpc == null)
        //    {
        //        BasePlayer.ChatMessage("Not npc selected");
        //        return false;
        //    }

        //    SelectedNpc.Npc.Controller.Component.Kill();
        //    onRemoveNpc?.Invoke(SelectedNpc.Name);

        //    return true;
        //}

        public void SelectNpcCommand()
        {
            if (m_isTest)
                return;

            NpcCreator_NpcController npc = null;
            if (TrySelectNpc(out npc))
            {
                SelectNpc(npc);
                BasePlayer.ChatMessage($"Npc {npc.Npc.Controller.Component.displayName} selected");
            }
            else
            {
                BasePlayer.ChatMessage("Not npc founded");
            }
        }

        public void UnselectNpcCommand()
        {
            if (m_isTest)
                return;

            UnSelectNpc();
        }

        public void CopyWearToSelectedNpc()
        {
            if (m_isTest)
                return;

            if (SelectedNpc == null) return;

            SelectedNpc.MovePlayerWearItemsToNpc(BasePlayer.inventory);
            SelectedNpc.MovePlayerBeltItemsToNpc(BasePlayer.inventory);
        }

        public bool TrySelectNpc(out NpcCreator_NpcController selectedNpc)
        {
            selectedNpc = null;
            var ray = BasePlayer.eyes.HeadRay();
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, DISTANCE_SELECT_MAX, (1 << (int)Layer.AI) | (1 << (int)Layer.Player_Server)))
            {
                if (hit.collider == null)
                    return false;

                if (hit.collider.gameObject == null)
                    return false;

                var component = hit.collider.gameObject.GetComponent<CustomNpc_Component>();
                if (component == null)
                    return false;

                if (m_npcs.ContainsKey(component.net.ID.Value) == false)
                    return false;

                selectedNpc = m_npcs[component.net.ID.Value];
                return true;
            }

            return false;
        }

        private void SelectNpc(NpcCreator_NpcController npc)
        {
            if (SelectedNpc != null)
                SelectedNpc.OnUnselect();

            SelectedNpc = npc;
            npc.MoveNpcWearItemsToPlayer(BasePlayer.inventory);
            npc.MoveNpcBeltItemsToPlayer(BasePlayer.inventory);

            SelectedNpc.OnSelect();
        }

        private void UnSelectNpc()
        {
            SelectedNpc = null;
        }

        public void SaveAllNpcConfig()
        {
            foreach (var npc in m_npcs.Values)
            {
                SaveNpcConfig(npc);
            }
        }

        public void SaveSelectNpcConfig()
        {
            SaveNpcConfig(SelectedNpc);
        }

        public void SaveNpcConfig(NpcCreator_NpcController npc)
        {
            var config = npc.Npc.Controller.Configuration;
            var fileName = NpcCreator_Manager.NPC_FILE_BASE + config.Name;

            SaveWearConfiguration(npc);
            SaveBeltConfiguration(npc);

            Interface.Oxide.DataFileSystem.WriteObject($"{NpcCreator_Manager.Plugin.Name}/{fileName}", config);
        }

        private void SaveWearConfiguration(NpcCreator_NpcController npc)
        {
            var config = npc.Npc.Controller.Configuration;
            config.WearItems = new List<CustomNpc_WearItem>();

            foreach (var item in npc.Npc.Controller.Component.inventory.containerWear.itemList)
            {
                config.WearItems.Add(new CustomNpc_WearItem() { ShortName = item.info.shortname, SkinId = item.skin });
            }
        }

        private void SaveBeltConfiguration(NpcCreator_NpcController npc)
        {
            var config = npc.Npc.Controller.Configuration;
            config.BeltItems = new List<CustomNpc_BeltItem>();

            foreach (var item in npc.Npc.Controller.Component.inventory.containerBelt.itemList)
            {
                var beltItem = new CustomNpc_BeltItem() { ShortName = item.info.shortname, SkinId = item.skin, Amount = item.amount };
                config.BeltItems.Add(beltItem);

                var heldentity = item.GetHeldEntity();
                if (heldentity == null)
                    continue;

                BaseProjectile baseProjectile = heldentity as BaseProjectile;
                if (baseProjectile == null)
                    continue;

                var mods = item.contents;
                if (mods != null && mods.itemList != null)
                {
                    foreach(var mod in mods.itemList)
                    {
                        beltItem.Mods.Add(mod.info.shortname);
                    }
                }

                if (baseProjectile.primaryMagazine != null)
                {
                    var ammo = baseProjectile.primaryMagazine.ammoType.shortname;
                    beltItem.Ammo = ammo;
                }
            }
        }

        public void ReloaddNpcConfig(NpcCreator_NpcController npc, bool select = false)
        {
            if (SelectedNpc == null)
                return;

            var config = npc.Npc.Controller.Configuration;

            NpcCreator_Manager.ReloadConfig(config.Name);

            ulong id = npc.Npc.Controller.Component.net.ID.Value;
            Vector3 position = npc.Npc.Controller.Component.ServerPosition;
            m_npcs.Remove(id);
            KillNpc(npc);
            NpcCreator_NpcController newNpc = null;
            InstanceNpc(config.Name, position, out newNpc, select);
        }

        public void ReloadSelectedNpcConfig()
        {
            ReloaddNpcConfig(SelectedNpc, true);
        }

        public void ReloadAllNpcConfig()
        {
            for (int i = 0; i < m_npcs.Count; i++)
            {
                ReloaddNpcConfig(m_npcs.ElementAt(i).Value);
                i--;
            }
        }

        public void TestSelect()
        {
            if (m_isTest)
                return;

            m_isSelectedTest = true;
            m_isTest = true;
            SelectedNpc.CreatorBrain.StartTest();
        }
        
        public void TestAll()
        {
            if (m_isTest)
                return;

            m_isSelectedTest = false;
            m_isTest = true;

            foreach (var npc in m_npcs)
            {
                npc.Value.CreatorBrain.StartTest();
            }
        }

        public void StopTest()
        {
            if (m_isTest == false)
                return;

            if (m_isSelectedTest == false)
            {
                foreach(var npc in m_npcs)
                {
                    npc.Value.CreatorBrain.StopTest();
                }
            }
            else
            {
                SelectedNpc.CreatorBrain.StopTest();
            }

            m_isTest = false;
        }
    }
}
