using System.Linq;
using System.Reflection.Emit;

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

            for (int i = 113; i <= 135; i++)
                codes[i].opcode = OpCodes.Nop;
            for (int i = 152; i <= 160; i++)
                codes[i].opcode = OpCodes.Nop;

            return codes.AsEnumerable();
        }
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(MenuJukebox), "State_Main")]
        static IEnumerable<CodeInstruction> AllowPlayingAllSongs(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 254; i <= 262; i++)
                codes[i].opcode = OpCodes.Nop;
            for (int i = 274; i <= 283; i++)
                codes[i].opcode = OpCodes.Nop;
            for (int i = 318; i <= 326; i++)
                codes[i].opcode = OpCodes.Nop;

            return codes.AsEnumerable();
        }
    }
}
