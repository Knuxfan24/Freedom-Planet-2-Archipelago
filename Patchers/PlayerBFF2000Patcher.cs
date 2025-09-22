using System.Reflection;

namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class PlayerBFF2000Patcher
    {
        /// <summary>
        /// Holds a reference to the player's object.
        /// </summary>
        public static PlayerBFF2000 player;

        // Holds references to the Death state, as it's private.
        public static readonly MethodInfo DeathState = typeof(PlayerBFF2000).GetMethod("State_Death", BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>
        /// Stores the PlayerBFF2000 object so that the regular DeathLink receiver can check for it.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerBFF2000), "Start")]
        private static void Setup(PlayerBFF2000 __instance) => player = __instance;

        /// <summary>
        /// Handles DeathLinks.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerBFF2000), "State_Death")]
        private static void DeathLinkHandle()
        {
            // Only remove the buffered DeathLink flag after the entire death state has played out, as there's something weird going on with it.
            if (player.genericTimer >= 350f)
                FPPlayerPatcher.hasBufferedDeathLink = false;

            // Send a DeathLink.
            FPPlayerPatcher.SendDeathLink($"{Helpers.GetPlayer()} failed to be a mech pilot. [{Plugin.session.Players.GetPlayerName(Plugin.session.ConnectionInfo.Slot)}]", false);
        }
    }
}
