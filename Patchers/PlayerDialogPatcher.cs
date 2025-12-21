namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class PlayerDialogPatcher
    {
        /// <summary>
        /// Removes the lines that silence the player character and lowers the music volume when a text box is up.
        /// </summary>
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(PlayerDialog), "State_Open")]
        static IEnumerable<CodeInstruction> RemoveQuieting(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            // Remove the lines that cause the player to be marked as talking and remove the music quieting.
            for (int codeIndex = 95; codeIndex <= 105; codeIndex++)
                codes[codeIndex].opcode = OpCodes.Nop;

            return codes.AsEnumerable();
        }
    }
}
