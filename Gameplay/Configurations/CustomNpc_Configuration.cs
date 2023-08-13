using System.Collections.Generic;


namespace Oxide.Ext.CustomNpc.Gameplay.Configurations
{
    public class CustomNpc_Configuration
    {
        public string Name;

        public int AreaMask;
        public int AgentTypeID;

        public float StartHealth;
        public float MaxHealth;

        public float DamageScale;
        public float AttackRangeMultiplier;

        public float Speed;
        public float MemoryDuration;
        public float SenseRange;
        public bool CheckVisionCone;
        public float ListenRange;
        public float VisionCone;

        public bool CanRunAwayWater;

        public float ChaseRange;
        public float RoamRange;

        public List<string> States = new List<string>();

        public bool DisableRadio;

        public string Kit;
        public List<CustomNpc_BeltItem> BeltItems = new List<CustomNpc_BeltItem>();
        public List<CustomNpc_WearItem> WearItems = new List<CustomNpc_WearItem>();

        public bool CanTargetOtherNpc;

        public static CustomNpc_Configuration Default()
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
                States = new List<string>() { "DefaultIdleState", "DefaultRoamState", "DefaultChaseState", "DefaultCombatState", "ChangeWeaponState" }
            };

            return npcConfig;
        }
    }
}
