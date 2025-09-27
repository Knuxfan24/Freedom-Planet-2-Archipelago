namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class PlayerBFF2000Patcher
    {
        /// <summary>
        /// Holds a reference to the player's object.
        /// </summary>
        public static PlayerBFF2000 player;

        /// <summary>
        /// Stupid hack boolean to trigger a loop in the HealthUpdate function.
        /// </summary>
        private static bool StupidHack;

        /// <summary>
        /// Stores the PlayerBFF2000 object so that the regular DeathLink receiver can check for it.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerBFF2000), "Start")]
        private static void Setup(PlayerBFF2000 __instance) => player = __instance;

        /// <summary>
        /// Handles receiving DeathLinks.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerBFF2000), "HealthUpdate")]
        private static void ReceiveDeathLink(ref float ___flinchHealth, ref float ___healthBuffer)
        {
            // Check for a DeathLink.
            if (FPPlayerPatcher.hasBufferedDeathLink)
            {
                // Remove the player's shield.
                player.targetPlayer.shieldHealth = 0;

                // Turn off our can send flag so we don't send one in return.
                FPPlayerPatcher.canSendDeathLink = false;

                // Force the player's health down to -1 for the upcoming maths
                player.targetPlayer.health = -1;

                //Remove the buffered DeathLink.
                FPPlayerPatcher.hasBufferedDeathLink = false;

                // Drop the weakpoint health down.
                player.weakPoint.health = 450;

                // Set the flinch health to -1.
                ___flinchHealth = -1;

                // Enable my stupid hack.
                StupidHack = true;
            }

            // Repeatedly subtract the healthBuffer.
            // I don't know why this works, but this seems to cause the maths to add up in the right way to fire the death state.
            if (StupidHack)
                ___healthBuffer--;
        }

        /// <summary>
        /// Handles sending DeathLinks and resetting some flags.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerBFF2000), "State_Death")]
        private static void SendDeathlink()
        {
            // Reset the buffered DeathLink and stupid hack flags.
            FPPlayerPatcher.hasBufferedDeathLink = false;
            StupidHack = false;

            // Send a DeathLink.
            FPPlayerPatcher.SendDeathLink($"{Helpers.GetPlayer()} failed to be a mech pilot. [{Plugin.session.Players.GetPlayerName(Plugin.session.ConnectionInfo.Slot)}]", false);
        }
    }
}
