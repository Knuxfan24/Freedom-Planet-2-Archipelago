namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class MenuExchangePatcher
    {
        /// <summary>
        /// Replaces the crystal shard and core requirements for a Gold Gem exchange with our slot data's values.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuExchange), "Start")]
        static void ReplaceCosts(ref int[] ___itemRequirements)
        {
            ___itemRequirements[0] = (int)(long)Plugin.slotData["gold_gem_crystal_cost"];
            ___itemRequirements[1] = (int)(long)Plugin.slotData["gold_gem_core_cost"];
        }
    }
}
