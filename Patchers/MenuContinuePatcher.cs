namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class MenuContinuePatcher
    {
        /// <summary>
        /// Disables the Rail Trap if its active.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuContinue), "Start")]
        static void MenuSetup(MenuContinue)
        {
            if (Plugin.RailTrap)
                Plugin.RailTrap = false;
        }
    }
}
