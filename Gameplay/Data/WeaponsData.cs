using System.Collections.Generic;


namespace Oxide.Ext.CustomNpc.Gameplay.Data
{
    public static class WeaponsData
    {
        public class Range { public float EffectiveRange; public float AttackLengthMin; public float AttackLengthMax; }

        public static readonly Dictionary<string, Range> WeaponsRange = new Dictionary<string, Range>
        {
            ["snowballgun"] = new Range { EffectiveRange = 5f, AttackLengthMin = 2f, AttackLengthMax = 2f },
            ["shotgun.double"] = new Range { EffectiveRange = 10f, AttackLengthMin = 0.3f, AttackLengthMax = 1f },
            ["pistol.eoka"] = new Range { EffectiveRange = 10f, AttackLengthMin = -1f, AttackLengthMax = -1f },
            ["shotgun.waterpipe"] = new Range { EffectiveRange = 10f, AttackLengthMin = -1f, AttackLengthMax = -1f },
            ["speargun"] = new Range { EffectiveRange = 15f, AttackLengthMin = -1f, AttackLengthMax = -1f },
            ["bow.compound"] = new Range { EffectiveRange = 15f, AttackLengthMin = -1f, AttackLengthMax = -1f },
            ["crossbow"] = new Range { EffectiveRange = 15f, AttackLengthMin = -1f, AttackLengthMax = -1f },
            ["bow.hunting"] = new Range { EffectiveRange = 15f, AttackLengthMin = -1f, AttackLengthMax = -1f },
            ["pistol.nailgun"] = new Range { EffectiveRange = 15f, AttackLengthMin = 0f, AttackLengthMax = 0.46f },
            ["pistol.python"] = new Range { EffectiveRange = 15f, AttackLengthMin = 0.175f, AttackLengthMax = 0.525f },
            ["pistol.semiauto"] = new Range { EffectiveRange = 15f, AttackLengthMin = 0f, AttackLengthMax = 0.46f },
            ["pistol.prototype17"] = new Range { EffectiveRange = 15f, AttackLengthMin = 0f, AttackLengthMax = 0.46f },
            ["multiplegrenadelauncher"] = new Range { EffectiveRange = 20f, AttackLengthMin = -1f, AttackLengthMax = -1f },
            ["smg.2"] = new Range { EffectiveRange = 20f, AttackLengthMin = 0.4f, AttackLengthMax = 0.4f },
            ["smg.thompson"] = new Range { EffectiveRange = 20f, AttackLengthMin = 0.4f, AttackLengthMax = 0.4f },
            ["rifle.bolt"] = new Range { EffectiveRange = 150f, AttackLengthMin = -1f, AttackLengthMax = -1f },
            ["rifle.l96"] = new Range { EffectiveRange = 150f, AttackLengthMin = -1f, AttackLengthMax = -1f }
        };

        public static readonly HashSet<string> MeleeWeapons = new HashSet<string>
        {
            "bone.club",
            "knife.bone",
            "knife.butcher",
            "candycaneclub",
            "knife.combat",
            "longsword",
            "mace",
            "machete",
            "paddle",
            "pitchfork",
            "salvaged.cleaver",
            "salvaged.sword",
            "spear.stone",
            "spear.wooden",
            "chainsaw",
            "hatchet",
            "jackhammer",
            "pickaxe",
            "axe.salvaged",
            "hammer.salvaged",
            "icepick.salvaged",
            "stonehatchet",
            "stone.pickaxe",
            "torch",
            "sickle",
            "rock",
            "snowball",
            "mace.baseballbat"
        };

        public static readonly HashSet<string> FirstDistanceWeapons = new HashSet<string>
        {
            "speargun",
            "bow.compound",
            "crossbow",
            "bow.hunting",
            "shotgun.double",
            "pistol.eoka",
            "flamethrower",
            "pistol.m92",
            "pistol.nailgun",
            "multiplegrenadelauncher",
            "shotgun.pump",
            "pistol.python",
            "pistol.revolver",
            "pistol.semiauto",
            "pistol.prototype17",
            "snowballgun",
            "shotgun.spas12",
            "shotgun.waterpipe"
        };

        public static readonly HashSet<string> SecondDistanceWeapons = new HashSet<string>
        {
            "smg.2",
            "smg.mp5",
            "rifle.semiauto",
            "smg.thompson"
        };

        public static readonly HashSet<string> ThirdDistanceWeapons = new HashSet<string>
        {
            "rifle.ak",
            "rifle.lr300",
            "lmg.m249",
            "rifle.m39",
            "hmlmg",
            "rifle.ak.ice"
        };

        public static readonly HashSet<string> FourthDistanceWeapons = new HashSet<string>
        {
            "rifle.bolt",
            "rifle.l96"
        };
    }
}
