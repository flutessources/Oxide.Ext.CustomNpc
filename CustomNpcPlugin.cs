using Oxide.Core;
using Oxide.Core.Plugins;
using Oxide.Ext.CustomNpc.Gameplay.Components;
using Oxide.Ext.CustomNpc.Gameplay.Configurations;
using Oxide.Ext.CustomNpc.Gameplay.Managers;
using Oxide.Ext.CustomNpc.Gameplay.NpcCreator;
using Oxide.Ext.CustomNpc.PluginExntesions;
using Oxide.Plugins;
using System.Collections.Generic;

namespace Oxide.Ext.CustomNpc
{
    [Info("CustomNpcPlugin", "Flutes", "0.1.0")]
    [Description("test")]
    public class CustomNpcPlugin : RustPlugin
    {
        private void Init()
        {
            new PluginsExtensionsManager();
            CustomNpc_Manager.Setup();

            Unsubscribe("OnInventoryNetworkUpdate");
        }

        private void Unload()
        {
            CustomNpc_Manager.DestroyAllNpcs();
        }

        private void SpawnTest(BasePlayer player)
        {
            CustomNpc_Configuration npcConfig = new CustomNpc_Configuration()
            {
                DamageScale = 1,
                DisableRadio = true,
                AgentTypeID = -1372625422,
                MemoryDuration = 10.0f,
                AreaMask = 1,
                AttackRangeMultiplier = 1.0f,
                WearItems = new List<CustomNpc_WearItem> { new CustomNpc_WearItem { ShortName = "attire.egg.suit", SkinId = 0 } },
                BeltItems = new List<CustomNpc_BeltItem>
                            {
                                new CustomNpc_BeltItem { ShortName = "rifle.lr300", Amount = 1, SkinId = 0, Mods = new List<string> { "weapon.mod.holosight", "weapon.mod.flashlight" } },
                                new CustomNpc_BeltItem { ShortName = "syringe.medical", Amount = 10, SkinId = 0, Mods = new List<string>() },
                                new CustomNpc_BeltItem { ShortName = "grenade.f1", Amount = 10, SkinId = 0, Mods = new List<string>() },
                                new CustomNpc_BeltItem { ShortName = "grenade.smoke", Amount = 10, SkinId = 0, Mods = new List<string>() },
                                new CustomNpc_BeltItem { ShortName = "explosive.timed", Amount = 10, SkinId = 0, Mods = new List<string>() },
                                new CustomNpc_BeltItem { ShortName = "rocket.launcher", Amount = 1, SkinId = 0, Mods = new List<string>() }
                            },
                CanTargetOtherNpc = false,
                ChaseRange = 100.0f,
                CheckVisionCone = false,
                ListenRange = 10.0f,
                MaxHealth = 200.0f,
                StartHealth = 200.0f,
                RoamRange = 10f,
                Name = "Test",
                SenseRange = 50f,
                Speed = 7.5f,
                VisionCone = 15.0f,
                States = new List<string>() { "DefaultIdleState", "DefaultRoamState", "DefaultChaseState", "DefaultCombatState", "DefaultHealState" }
            };

            NpcInstantiationFactory.InstanceNpcDefault(player.ServerPosition, npcConfig);
        }

        #region Oxide Hooks
        private void OnEntityKill(CustomNpc_Component customNpc)
        {
            CustomNpc_Manager.OnNpcDestroyed(customNpc);

            if (NpcCreator_Manager.IsStarted)
            {
                NpcCreator_Manager.OnEntityKill(customNpc);
            }
            //NpcCreator_ManagerFactory.OnEntityKill(customNpc);
        }

        object OnInventoryNetworkUpdate(PlayerInventory inventory, ItemContainer container, ProtoBuf.UpdateItemContainer updateItemContainer, PlayerInventory.Type type, bool broadcast)
        {
            if (inventory.baseEntity == null)
                return null;

            if (NpcCreator_Manager.Controllers.ContainsKey(inventory.baseEntity.userID) == false)
                return null;

            var controller = NpcCreator_Manager.Controllers[inventory.baseEntity.userID];
            controller.CopyWearToSelectedNpc();

            return null;
        }
        #endregion

        [ChatCommand("customnpc_spawn")]
        private void CustomNpcSpawnChatCommand(BasePlayer player, string command, string[] args)
        {
            if (player.IsAdmin == false)
                return;

            SpawnTest(player);
        }

        [ChatCommand("customnpc_creator_start")]
        private void CustomNpcCreatorStartChatCommand(BasePlayer player, string command, string[] args)
        {
            if (player.IsAdmin == false)
                return;

            if (args.Length == 0)
                return;

            string pluginName = args[0];
            var plugin = Interface.Oxide.RootPluginManager.GetPlugin(pluginName);

            if (plugin == null)
            {
                player.ChatMessage($"Plugin {pluginName} not found");
                return;
            }

            if (NpcCreator_Manager.IsStarted == false)
                Subscribe("OnInventoryNetworkUpdate");

            NpcCreator_Manager.StartCreator(player, plugin);
            player.ChatMessage($"Success");
        }


        [ChatCommand("customnpc_creator_stop")]
        private void CustomNpcCreatorStopChatCommand(BasePlayer player, string command, string[] args)
        {
            if (player.IsAdmin == false)
                return;

            NpcCreator_Manager.StopCreator(player);

            if (NpcCreator_Manager.IsStarted == false)
                Unsubscribe("OnInventoryNetworkUpdate");
        }

        [ChatCommand("customnpc_creator_stop_all")]
        private void CustomNpcCreatorStopAllChatCommand(BasePlayer player, string command, string[] args)
        {
            if (player.IsAdmin == false)
                return;

            NpcCreator_Manager.Stop();

            Unsubscribe("OnInventoryNetworkUpdate");

            player.ChatMessage($"Success");
        }

        [ChatCommand("customnpc_creator_edit")]
        private void CustomNpcCreatorEditChatCommand(BasePlayer player, string command, string[] args)
        {
            if (player.IsAdmin == false)
                return;

            if (NpcCreator_Manager.Controllers.ContainsKey(player.userID) == false)
                return;

            var controller = NpcCreator_Manager.Controllers[player.userID];
            controller.InstanceNpcCommand(args);

            player.ChatMessage($"Success");
        }

        [ChatCommand("customnpc_creator_select")]
        private void CustomNpcCreatorSelectChatCommand(BasePlayer player, string command, string[] args)
        {
            if (player.IsAdmin == false)
                return;

            if (NpcCreator_Manager.Controllers.ContainsKey(player.userID) == false)
                return;

            var controller = NpcCreator_Manager.Controllers[player.userID];
            controller.SelectNpcCommand();

            player.ChatMessage($"Success");
        }

        [ChatCommand("customnpc_creator_unselect")]
        private void CustomNpcCreatorUnselectChatCommand(BasePlayer player, string command, string[] args)
        {
            if (player.IsAdmin == false)
                return;

            if (NpcCreator_Manager.Controllers.ContainsKey(player.userID) == false)
                return;

            var controller = NpcCreator_Manager.Controllers[player.userID];
            controller.UnselectNpcCommand();

            player.ChatMessage($"Success");
        }

        [ChatCommand("customnpc_creator_save_all")]
        private void CustomNpcCreatorSaveAllChatCommand(BasePlayer player, string command, string[] args)
        {
            if (player.IsAdmin == false)
                return;

            if (NpcCreator_Manager.Controllers.ContainsKey(player.userID) == false)
                return;

            var controller = NpcCreator_Manager.Controllers[player.userID];
            controller.SaveAllNpcConfig();

            player.ChatMessage($"Success");
        }

        [ChatCommand("customnpc_creator_save_selected")]
        private void CustomNpcCreatorSaveSelectedChatCommand(BasePlayer player, string command, string[] args)
        {
            if (player.IsAdmin == false)
                return;

            if (NpcCreator_Manager.Controllers.ContainsKey(player.userID) == false)
                return;

            var controller = NpcCreator_Manager.Controllers[player.userID];
            controller.SaveSelectNpcConfig();

            player.ChatMessage($"Success");
        }

        [ChatCommand("customnpc_creator_reload_all")]
        private void CustomNpcCreatoReloadAllChatCommand(BasePlayer player, string command, string[] args)
        {
            if (player.IsAdmin == false)
                return;

            if (NpcCreator_Manager.Controllers.ContainsKey(player.userID) == false)
                return;

            var controller = NpcCreator_Manager.Controllers[player.userID];
            controller.ReloadAllNpcConfig();

            player.ChatMessage($"Success");
        }

        [ChatCommand("customnpc_creator_reload_selected")]
        private void CustomNpcCreatoReloadSelectedChatCommand(BasePlayer player, string command, string[] args)
        {
            if (player.IsAdmin == false)
                return;

            if (NpcCreator_Manager.Controllers.ContainsKey(player.userID) == false)
                return;

            var controller = NpcCreator_Manager.Controllers[player.userID];
            controller.ReloadSelectedNpcConfig();

            player.ChatMessage($"Success");
        }

        [ChatCommand("customnpc_creator_test_selected")]
        private void CustomNpcCreatorTestSelectedChatCommand(BasePlayer player, string command, string[] args)
        {
            if (player.IsAdmin == false)
                return;

            if (NpcCreator_Manager.Controllers.ContainsKey(player.userID) == false)
                return;

            var controller = NpcCreator_Manager.Controllers[player.userID];
            controller.TestSelect();

            player.ChatMessage($"Success");
        }

        [ChatCommand("customnpc_creator_test_all")]
        private void CustomNpcCreatorTestAllChatCommand(BasePlayer player, string command, string[] args)
        {
            if (player.IsAdmin == false)
                return;


            if (NpcCreator_Manager.Controllers.ContainsKey(player.userID) == false)
                return;

            var controller = NpcCreator_Manager.Controllers[player.userID];
            controller.TestAll();

            player.ChatMessage($"Success");
        }

        [ChatCommand("customnpc_creator_test_stop")]
        private void CustomNpcCreatorTestEndChatCommand(BasePlayer player, string command, string[] args)
        {
            if (player.IsAdmin == false)
                return;

            if (NpcCreator_Manager.Controllers.ContainsKey(player.userID) == false)
                return;

            var controller = NpcCreator_Manager.Controllers[player.userID];
            controller.StopTest();

            player.ChatMessage($"Success");
        }
    }
}
