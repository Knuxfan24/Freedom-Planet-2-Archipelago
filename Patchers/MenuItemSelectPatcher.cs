using System.Linq;
using System.Reflection.Emit;

namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class MenuItemSelectPatcher
    {
        /// <summary>
        /// Overwrites the default inventories with the ones from our save.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuItemSelect), "Start")]
        static void SetInventory(MenuItemSelect __instance)
        {
            __instance.amulets = Plugin.save.BraveStones;
            __instance.potions = Plugin.save.Potions;
        }

        /// <summary>
        /// Removes the code that recreates the inventory from the game's own save.
        /// </summary>
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(MenuItemSelect), "Start")]
        static IEnumerable<CodeInstruction> RemoveInventoryCreation(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 56; i <= 117; i++)
                codes[i].opcode = OpCodes.Nop;

            return codes.AsEnumerable();
        }
    }
}
