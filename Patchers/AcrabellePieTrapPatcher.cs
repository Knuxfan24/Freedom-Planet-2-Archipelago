// TODO: This managed to crash the game when receiving one on stage clear?
namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class AcrabellePieTrapPatcher
    {
        /// <summary>
        /// Snaps the pies spawned by the Pie Trap to the player's location.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AcrabellePieTrap), "State_Default")]
        static void ForceSnapToPlayer(AcrabellePieTrap __instance)
        {
            // Check that this pie was spawned by us and not by Acrabelle herself.
            if (__instance.name.StartsWith("APPieTrap"))
            {
                // If the player doesn't exist, then destroy this pie.
                if (FPPlayerPatcher.player == null)
                    FPStage.DestroyStageObject(__instance);

                // Snap this pie to the player's position.
                __instance.position = FPPlayerPatcher.player.position;
            }
        }
    }
}
