namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class MenuWorldMapConfirmPatcher
    {
        /// <summary>
        /// Forces the Battlesphere to go to the Arena Menu.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuWorldMapConfirm), "Start")]
        static void RedirectArena(MenuWorldMapConfirm __instance) => __instance.arenaSceneClassicIncomplete = __instance.arenaSceneClassic;
    }
}
