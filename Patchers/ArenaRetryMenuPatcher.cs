namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class ArenaRetryMenuPatcher
    {
        /// <summary>
        /// Disables the Rail Trap if its active.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ArenaRetryMenu), "Start")]
        static void MenuSetup()
        {
            if (Plugin.RailTrap)
                Plugin.RailTrap = false;
        }
    }
}
