using System.Collections.Generic;


namespace Oxide.Ext.CustomNpc.Gameplay.Data
{
    public static class CoversData
    {
        public static readonly HashSet<string> Barricades = new HashSet<string>
            {
                "barricade.cover.wood",
                "barricade.sandbags",
                "barricade.concrete",
                "barricade.stone"
            };
    }
}
