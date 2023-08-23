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

        public float Speed;
        public float MemoryDuration;

		public bool DisableRadio;
		public string Kit;
		public bool CanTargetOtherNpc;

		public List<string> States = new List<string>();
        public List<CustomNpc_BeltItem> BeltItems = new List<CustomNpc_BeltItem>();
        public List<CustomNpc_WearItem> WearItems = new List<CustomNpc_WearItem>();

        public CustomNpc_Ranges Ranges = new CustomNpc_Ranges();
        public LootTableConfiguration LootTable = new LootTableConfiguration();

        public static CustomNpc_Configuration Default()
        {
            CustomNpc_Configuration npcConfig = new CustomNpc_Configuration()
            {
                DamageScale = 1,
                DisableRadio = true,
                AgentTypeID = -1372625422,
                MemoryDuration = 10.0f,
                AreaMask = 1,
                Ranges = new CustomNpc_Ranges()
                {
                    AttackRangeMultiplier = 1.0f,
                    ChaseRange = 100.0f,
                    CheckVisionCone = false,
                    ListenRange = 10.0f,
                    RoamRange = 10f,
                    SenseRange = 50f,
                    VisionCone = 15.0f,
                },
                WearItems = new List<CustomNpc_WearItem> { new CustomNpc_WearItem { ShortName = "attire.egg.suit", SkinId = 0 } },
                BeltItems = new List<CustomNpc_BeltItem>
                {
                    new CustomNpc_BeltItem { ShortName = "rifle.lr300", Amount = 1, SkinId = 0, Mods = new List<string> { "weapon.mod.holosight", "weapon.mod.flashlight" } },
                },

                CanTargetOtherNpc = false,
                MaxHealth = 200.0f,
                StartHealth = 200.0f,
                Name = "Custom Npc",
                Speed = 7.5f,
                States = new List<string>() { "DefaultIdleState", "DefaultRoamState", "DefaultChaseState", "DefaultCombatState", "ChangeWeaponState" },
                LootTable = null
            };

            return npcConfig;
        }
    }
}
