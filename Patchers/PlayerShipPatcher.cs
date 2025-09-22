namespace Freedom_Planet_2_Archipelago.Patchers
{
    // TODO: This doesn't seem to work right???????
    internal class PlayerShipPatcher
    {
        /// <summary>
        /// Holds a reference to the player's object.
        /// </summary>
        public static PlayerShip player;

        /// <summary>
        /// Stores the PlayerShip object so that the regular DeathLink receiver can check for it.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerShip), "Start")]
        private static void Setup(PlayerShip __instance) => player = __instance;

        /// <summary>
        /// Resets the DeathLink flag upon revival.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerShip), "State_Revive")]
        private static void Revive() => FPPlayerPatcher.canSendDeathLink = true;

        /// <summary>
        /// Sends out a DeathLink.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerShip), "State_Death")]
        private static void SendDeathlink() => FPPlayerPatcher.SendDeathLink($"{Helpers.GetPlayer()} got shot down. [{Plugin.session.Players.GetPlayerName(Plugin.session.ConnectionInfo.Slot)}]", false);
    }
}
