namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class FPHudMasterPatcher
    {
        /// <summary>
        /// Kills the player when the Time Limit Brave Stone's timer elapses.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPHudMaster), "LateUpdate")]
        static void KillOnTimeLimit(FPHudMaster __instance, ref GameObject ___hudTimeLimitBar)
        {
            // Check that the player actually exists and that the Dangerous Time Limit option is enabled.
            if (FPPlayerPatcher.player == null && (long)Plugin.slotData["dangerous_time_limit"] != 0)
                return;

            // Check that the time limit bar exists, the player has the Time Limit Brave Stone and that the bar is actually being shown.
            if (___hudTimeLimitBar != null && FPPlayerPatcher.player.IsPowerupActive(FPPowerup.TIME_LIMIT) && !__instance.onlyShowHealth)
            {
                // Check if the time limit has elapsed.
                if (FPSaveManager.GetStageParTime(FPStage.currentStage.stageID) - (FPStage.currentStage.minutes * 6000 + FPStage.currentStage.seconds * 100 + FPStage.currentStage.milliSeconds) < 0)
                {
                    // Disable the player's DeathLink flag so we don't send a "became a pancake" message.
                    FPPlayerPatcher.canSendDeathLink = false;

                    // Forcibly run the player's crush action to blow them up.
                    FPPlayerPatcher.player.Action_Crush();

                    // Send a DeathLink with the message "Character ran out of time."
                    Plugin.DeathLink.SendDeathLink(new Archipelago.MultiClient.Net.BounceFeatures.DeathLink.DeathLink(Plugin.session.Players.GetPlayerName(Plugin.session.ConnectionInfo.Slot), $"{Helpers.GetPlayer()} ran out of time."));
                }
            }
        }
    }
}
