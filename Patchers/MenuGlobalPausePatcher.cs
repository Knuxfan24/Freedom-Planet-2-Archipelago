namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class MenuGlobalPausePatcher
    {
        /// <summary>
        /// Redirects the game quit to the Title Screen rather than the Main Menu and disconnects us from the server.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuGlobalPause), "State_Transition")]
        static void ReturnToTitle()
        {
            FPSaveManager.menuToLoad = 4;
            Plugin.session.Socket.Disconnect();
        }
    }
}
