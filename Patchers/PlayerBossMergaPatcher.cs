namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class PlayerBossMergaPatcher
    {
        /// <summary>
        /// Empties out the cutscene arrays for Merga to stop the ending from playing when defeating her if our goal is Weapon's Core.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerBossMerga), "Start")]
        static void DisableEnding(PlayerBossMerga __instance)
        {
            if ((long)Plugin.slotData["goal"] == 1)
            {
                __instance.cutsceneOnVictory = [null];
                __instance.adventureCutscene = [];
            }
        }
    }
}
