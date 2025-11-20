namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class MenuJukeboxPatcher
    {
        // Both of these functions are just for removing bits of the code that locks vinyls.
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(MenuJukebox), "UpdateMenuPosition")]
        static IEnumerable<CodeInstruction> ShowJukeboxIcons(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (int codeIndex = 113; codeIndex <= 135; codeIndex++)
                codes[codeIndex].opcode = OpCodes.Nop;
            for (int codeIndex = 152; codeIndex <= 160; codeIndex++)
                codes[codeIndex].opcode = OpCodes.Nop;

            return codes.AsEnumerable();
        }
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(MenuJukebox), "State_Main")]
        static IEnumerable<CodeInstruction> AllowPlayingAllSongs(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (int codeIndex = 254; codeIndex <= 262; codeIndex++)
                codes[codeIndex].opcode = OpCodes.Nop;
            for (int codeIndex = 274; codeIndex <= 283; codeIndex++)
                codes[codeIndex].opcode = OpCodes.Nop;
            for (int codeIndex = 318; codeIndex <= 326; codeIndex++)
                codes[codeIndex].opcode = OpCodes.Nop;

            return codes.AsEnumerable();
        }
    }
}
