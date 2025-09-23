namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class PlayerShipPatcher
    {
        /// <summary>
        /// Holds a reference to the player's object.
        /// </summary>
        public static PlayerShip player;

        /// <summary>
        /// Stores the PlayerShip object so that the regular DeathLink receiver can check for it.
        /// We specifically check for the one called PlayerZaoAirship, as Bakunawa Chase has two objects with this script, one doesn't actually reference the player.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerShip), "Start")]
        private static void Setup(PlayerShip __instance)
        {
            if (__instance.name == "PlayerZaoAirship")
                player = __instance;
        }

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

        /// <summary>
        /// Handles receiving DeathLinks.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerShip), "HealthUpdate")]
        private static void ReceiveDeathLink()
        {
            // Check for a DeathLink.
            if (FPPlayerPatcher.hasBufferedDeathLink)
            {
                // Turn off our can send flag so we don't send one in return.
                FPPlayerPatcher.canSendDeathLink = false;

                // Force the player's health down to -1 to fire the State_Death change.
                player.targetPlayer.health = -1;

                // Remove the buffered DeathLink.
                FPPlayerPatcher.hasBufferedDeathLink = false;
            }
        }
    }
}
