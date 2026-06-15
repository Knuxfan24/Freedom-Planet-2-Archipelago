using BepInEx.Bootstrap;

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
            // Load our data from the save.
            __instance.amulets = Plugin.save.BraveStones;
            __instance.potions = Plugin.save.Potions;

            // Expand the amulets list to match the length caused by FP2Lib's custom items.
            Array.Resize(ref __instance.amulets, __instance.amuletList.Length);

            // Check for the Sonic mod.
            if (Chainloader.PluginInfos.ContainsKey("K24_FP2_Sonic"))
            {
                // Create a value to hold the index of the Chaos Emeralds item.
                int emeraldsIndex = -1;

                // Loop backwards through the list to find the Chaos Emeralds.
                for (int itemIndex = __instance.amuletList.Length - 1; itemIndex >= 0; itemIndex--)
                {
                    if ((int)__instance.amuletList[itemIndex] == FP2Lib.Item.ItemHandler.GetItemDataByUid("k24.sonic.chaosemeralds").itemID)
                    {
                        emeraldsIndex = itemIndex;
                        break;
                    }
                }

                // Check that we've actually found the Emeralds.
                if (emeraldsIndex == -1)
                    return;

                // Check that we have all seven Emerald items.
                foreach (bool emerald in Plugin.save.ChaosEmeralds)
                    if (!emerald)
                        return;
                
                // Unlock the Chaos Emeralds item.
                __instance.amulets[emeraldsIndex] = true;
            }
        }

        /// <summary>
        /// Removes the code that recreates the inventory from the game's own save.
        /// </summary>
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(MenuItemSelect), "Start")]
        static IEnumerable<CodeInstruction> RemoveInventoryCreation(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (int codeIndex = 56; codeIndex <= 117; codeIndex++)
                codes[codeIndex].opcode = OpCodes.Nop;

            return codes.AsEnumerable();
        }
    }
}
