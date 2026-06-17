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

            // Expand the amulets list to match the length caused by any custom items.
            Array.Resize(ref __instance.amulets, __instance.amuletList.Length);
            Array.Resize(ref __instance.potions, __instance.potionList.Length);

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

                // Unlock the Chaos Emeralds item if we've got them all and the item actually exists.
                if (Plugin.save.SonicChaosEmeralds[0]
                    && Plugin.save.SonicChaosEmeralds[1]
                    && Plugin.save.SonicChaosEmeralds[2]
                    && Plugin.save.SonicChaosEmeralds[3]
                    && Plugin.save.SonicChaosEmeralds[4]
                    && Plugin.save.SonicChaosEmeralds[5]
                    && Plugin.save.SonicChaosEmeralds[6]
                    && emeraldsIndex != -1)
                    __instance.amulets[emeraldsIndex] = true;
            }

            // Check for the Potion Seller mod.
            if (Chainloader.PluginInfos.ContainsKey("com.eps.plugin.fp2.potion-seller"))
            {
                if (Plugin.save.PotionSellerPotions[0]) UnlockPotionSellerItem(80, __instance.potionList, __instance.potions);
                if (Plugin.save.PotionSellerPotions[1]) UnlockPotionSellerItem(81, __instance.potionList, __instance.potions);
                if (Plugin.save.PotionSellerPotions[2]) UnlockPotionSellerItem(82, __instance.potionList, __instance.potions);

                if (Plugin.save.PotionSellerBraveStones[0]) UnlockPotionSellerItem(83, __instance.amuletList, __instance.amulets);
                if (Plugin.save.PotionSellerBraveStones[1]) UnlockPotionSellerItem(84, __instance.amuletList, __instance.amulets);
                if (Plugin.save.PotionSellerBraveStones[2]) UnlockPotionSellerItem(85, __instance.amuletList, __instance.amulets);
                if (Plugin.save.PotionSellerBraveStones[3]) UnlockPotionSellerItem(86, __instance.amuletList, __instance.amulets);
                if (Plugin.save.PotionSellerBraveStones[4]) UnlockPotionSellerItem(87, __instance.amuletList, __instance.amulets);
                if (Plugin.save.PotionSellerBraveStones[5]) UnlockPotionSellerItem(88, __instance.amuletList, __instance.amulets);
                if (Plugin.save.PotionSellerBraveStones[6]) UnlockPotionSellerItem(89, __instance.amuletList, __instance.amulets);
                if (Plugin.save.PotionSellerBraveStones[7]) UnlockPotionSellerItem(90, __instance.amuletList, __instance.amulets);
                if (Plugin.save.PotionSellerBraveStones[8]) UnlockPotionSellerItem(91, __instance.amuletList, __instance.amulets);
                if (Plugin.save.PotionSellerBraveStones[9]) UnlockPotionSellerItem(92, __instance.amuletList, __instance.amulets);
                if (Plugin.save.PotionSellerBraveStones[10]) UnlockPotionSellerItem(93, __instance.amuletList, __instance.amulets);
                if (Plugin.save.PotionSellerBraveStones[11]) UnlockPotionSellerItem(94, __instance.amuletList, __instance.amulets);
                if (Plugin.save.PotionSellerBraveStones[12]) UnlockPotionSellerItem(95, __instance.amuletList, __instance.amulets);
                if (Plugin.save.PotionSellerBraveStones[13]) UnlockPotionSellerItem(96, __instance.amuletList, __instance.amulets);
                if (Plugin.save.PotionSellerBraveStones[14]) UnlockPotionSellerItem(97, __instance.amuletList, __instance.amulets);

                void UnlockPotionSellerItem(int targetID, FPPowerup[] itemList, bool[] activeArray)
                {
                    for (int itemIndex = itemList.Length - 1; itemIndex >= 0; itemIndex--)
                    {
                        if ((int)itemList[itemIndex] == targetID)
                        {
                            activeArray[itemIndex] = true;
                            return;
                        }
                    }
                }
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
