namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class ItemBoxPatcher
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemBox), "Start")]
        static void AddToFallBacks(ItemBox __instance) => FPStagePatcher.ItemBoxFallbackPositions.Add(__instance.gameObject, new(__instance.transform.position.x, __instance.transform.position.y));

        /// <summary>
        /// Sends a location check out upon breaking an item box.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemBox), "BoxHit")]
        static void SendLocationCheck(ItemBox __instance)
        {
            // TODO: Skip all of this if this item box is a Crate, as we're completely ignoring those.

            // Get the index of this item box's location.
            long locationIndex = GetLocationIndex(__instance);

            // If this location exists, then complete the check of it.
            if (Helpers.CheckLocationExists(locationIndex) && !Plugin.session.Locations.AllLocationsChecked.Contains(locationIndex))
            {
                // Complete the location check for this index.
                Plugin.EnqueueLocation(locationIndex);

                var item = Plugin.items[locationIndex];

                // Add a message to the queue if this item is for someone else.
                if (item.Player.Name != Plugin.session.Players.GetPlayerName(Plugin.session.ConnectionInfo.Slot))
                    Plugin.sentMessageQueue.Add($"Found {item.Player.Name}'s {item.ItemName}.");

                // Reset the chest tracers to remove this item box from it.
                FPPlayerPatcher.CreateChestTracers();
            }
        }

        /// <summary>
        /// Gets the index this item box's location.
        /// </summary>
        static long GetLocationIndex(ItemBox __instance)
        {
            // Set up a value to hold the item box position read from our hardcoded lists.
            KeyValuePair<string, Vector2> itemBoxPosition = new("", Vector2.zero);

            // Look up this item box's position based on our hardcoded list of item box positions.
            switch (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
            {
                case "DragonValley": itemBoxPosition = ItemBoxLists.DragonValley.FirstOrDefault(x => x.Value == __instance.position); break;
                case "ShenlinPark": itemBoxPosition = ItemBoxLists.ShenlinPark.FirstOrDefault(x => x.Value == __instance.position); break;
                case "TigerFalls": itemBoxPosition = ItemBoxLists.TigerFalls.FirstOrDefault(x => x.Value == __instance.position); break;
                case "RobotGraveyard": itemBoxPosition = ItemBoxLists.RobotGraveyard.FirstOrDefault(x => x.Value == __instance.position); break;
                case "ShadeArmory": itemBoxPosition = ItemBoxLists.ShadeArmory.FirstOrDefault(x => x.Value == __instance.position); break;
                case "Snowfields": itemBoxPosition = ItemBoxLists.Snowfields.FirstOrDefault(x => x.Value == __instance.position); break;
                case "AvianMuseum": itemBoxPosition = ItemBoxLists.AvianMuseum.FirstOrDefault(x => x.Value == __instance.position); break;
                case "AirshipSigwada": itemBoxPosition = ItemBoxLists.AirshipSigwada.FirstOrDefault(x => x.Value == __instance.position); break;
                case "PhoenixHighway": itemBoxPosition = ItemBoxLists.PhoenixHighway.FirstOrDefault(x => x.Value == __instance.position); break;
                case "ZaoLand": itemBoxPosition = ItemBoxLists.ZaoLand.FirstOrDefault(x => x.Value == __instance.position); break;
                case "GlobeOpera1": itemBoxPosition = ItemBoxLists.GlobeOpera1.FirstOrDefault(x => x.Value == __instance.position); break;
                case "GlobeOpera2": itemBoxPosition = ItemBoxLists.GlobeOpera2.FirstOrDefault(x => x.Value == __instance.position); break;
                case "PalaceCourtyard": itemBoxPosition = ItemBoxLists.PalaceCourtyard.FirstOrDefault(x => x.Value == __instance.position); break;
                case "TidalGate": itemBoxPosition = ItemBoxLists.TidalGate.FirstOrDefault(x => x.Value == __instance.position); break;
                case "SkyBridge": itemBoxPosition = ItemBoxLists.SkyBridge.FirstOrDefault(x => x.Value == __instance.position); break;
                case "LightningTower": itemBoxPosition = ItemBoxLists.LightningTower.FirstOrDefault(x => x.Value == __instance.position); break;
                case "ZulonJungle": itemBoxPosition = ItemBoxLists.ZulonJungle.FirstOrDefault(x => x.Value == __instance.position); break;
                case "NalaoLake": itemBoxPosition = ItemBoxLists.NalaoLake.FirstOrDefault(x => x.Value == __instance.position); break;
                case "AncestralForge": itemBoxPosition = ItemBoxLists.AncestralForge.FirstOrDefault(x => x.Value == __instance.position); break;
                case "MagmaStarscape": itemBoxPosition = ItemBoxLists.MagmaStarscape.FirstOrDefault(x => x.Value == __instance.position); break;
                case "GravityBubble": itemBoxPosition = ItemBoxLists.GravityBubble.FirstOrDefault(x => x.Value == __instance.position); break;
                case "Bakunawa1": itemBoxPosition = ItemBoxLists.BakunawaRush.FirstOrDefault(x => x.Value == __instance.position); break;
                case "Bakunawa2": itemBoxPosition = ItemBoxLists.ClockworkArboretum.FirstOrDefault(x => x.Value == __instance.position); break;
                case "Bakunawa3": itemBoxPosition = ItemBoxLists.InversionDynamo.FirstOrDefault(x => x.Value == __instance.position); break;
                case "Bakunawa4": itemBoxPosition = ItemBoxLists.LunarCannon.FirstOrDefault(x => x.Value == __instance.position); break;
                case "Battlesphere_Course1": itemBoxPosition = ItemBoxLists.Battlesphere1.FirstOrDefault(x => x.Value == __instance.position); break;
                case "Battlesphere_Course2": itemBoxPosition = ItemBoxLists.Battlesphere2.FirstOrDefault(x => x.Value == __instance.position); break;
                case "Battlesphere_Course3": itemBoxPosition = ItemBoxLists.Battlesphere3.FirstOrDefault(x => x.Value == __instance.position); break;
                case "Battlesphere_Course4": itemBoxPosition = ItemBoxLists.Battlesphere4.FirstOrDefault(x => x.Value == __instance.position); break;
                case "Battlesphere_Course5": itemBoxPosition = ItemBoxLists.Battlesphere5.FirstOrDefault(x => x.Value == __instance.position); break;
                case "Battlesphere_Course6": itemBoxPosition = ItemBoxLists.Battlesphere6.FirstOrDefault(x => x.Value == __instance.position); break;
                case "Battlesphere_Course7": itemBoxPosition = ItemBoxLists.Battlesphere7.FirstOrDefault(x => x.Value == __instance.position); break;
                case "Battlesphere_Course8": itemBoxPosition = ItemBoxLists.Battlesphere8.FirstOrDefault(x => x.Value == __instance.position); break;
                case "Battlesphere_Arena": itemBoxPosition = ItemBoxLists.BattlesphereArena.FirstOrDefault(x => x.Value == __instance.position); break;

                default:
                    Plugin.consoleLog.LogError($"No item box handling present for stage ID {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}!");
                    return -1;
            }

            // Check that we actually have a key and position. If not, then check for a fall back position (to account for Item Boxes that are parented to moving objects).
            if (itemBoxPosition.Key == "" || itemBoxPosition.Value == Vector2.zero)
            {
                // Look for a fallback position matching this Item Box's game object.
                Vector2 fallbackPosition = FPStagePatcher.ItemBoxFallbackPositions[__instance.gameObject];

                // Check that we've actually found a fallback position.
                if (fallbackPosition != null)
                {
                    // Look up this item box's fallback position based on our hardcoded list of item box positions.
                    switch (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
                    {
                        case "DragonValley": itemBoxPosition = ItemBoxLists.DragonValley.FirstOrDefault(x => x.Value == fallbackPosition); break;
                        case "ShenlinPark": itemBoxPosition = ItemBoxLists.ShenlinPark.FirstOrDefault(x => x.Value == fallbackPosition); break;
                        case "TigerFalls": itemBoxPosition = ItemBoxLists.TigerFalls.FirstOrDefault(x => x.Value == fallbackPosition); break;
                        case "RobotGraveyard": itemBoxPosition = ItemBoxLists.RobotGraveyard.FirstOrDefault(x => x.Value == fallbackPosition); break;
                        case "ShadeArmory": itemBoxPosition = ItemBoxLists.ShadeArmory.FirstOrDefault(x => x.Value == fallbackPosition); break;
                        case "Snowfields": itemBoxPosition = ItemBoxLists.Snowfields.FirstOrDefault(x => x.Value == fallbackPosition); break;
                        case "AvianMuseum": itemBoxPosition = ItemBoxLists.AvianMuseum.FirstOrDefault(x => x.Value == fallbackPosition); break;
                        case "AirshipSigwada": itemBoxPosition = ItemBoxLists.AirshipSigwada.FirstOrDefault(x => x.Value == fallbackPosition); break;
                        case "PhoenixHighway": itemBoxPosition = ItemBoxLists.PhoenixHighway.FirstOrDefault(x => x.Value == fallbackPosition); break;
                        case "ZaoLand": itemBoxPosition = ItemBoxLists.ZaoLand.FirstOrDefault(x => x.Value == fallbackPosition); break;
                        case "GlobeOpera1": itemBoxPosition = ItemBoxLists.GlobeOpera1.FirstOrDefault(x => x.Value == fallbackPosition); break;
                        case "GlobeOpera2": itemBoxPosition = ItemBoxLists.GlobeOpera2.FirstOrDefault(x => x.Value == fallbackPosition); break;
                        case "PalaceCourtyard": itemBoxPosition = ItemBoxLists.PalaceCourtyard.FirstOrDefault(x => x.Value == fallbackPosition); break;
                        case "TidalGate": itemBoxPosition = ItemBoxLists.TidalGate.FirstOrDefault(x => x.Value == fallbackPosition); break;
                        case "SkyBridge": itemBoxPosition = ItemBoxLists.SkyBridge.FirstOrDefault(x => x.Value == fallbackPosition); break;
                        case "LightningTower": itemBoxPosition = ItemBoxLists.LightningTower.FirstOrDefault(x => x.Value == fallbackPosition); break;
                        case "ZulonJungle": itemBoxPosition = ItemBoxLists.ZulonJungle.FirstOrDefault(x => x.Value == fallbackPosition); break;
                        case "NalaoLake": itemBoxPosition = ItemBoxLists.NalaoLake.FirstOrDefault(x => x.Value == fallbackPosition); break;
                        case "AncestralForge": itemBoxPosition = ItemBoxLists.AncestralForge.FirstOrDefault(x => x.Value == fallbackPosition); break;
                        case "MagmaStarscape": itemBoxPosition = ItemBoxLists.MagmaStarscape.FirstOrDefault(x => x.Value == fallbackPosition); break;
                        case "GravityBubble": itemBoxPosition = ItemBoxLists.GravityBubble.FirstOrDefault(x => x.Value == fallbackPosition); break;
                        case "Bakunawa1": itemBoxPosition = ItemBoxLists.BakunawaRush.FirstOrDefault(x => x.Value == fallbackPosition); break;
                        case "Bakunawa2": itemBoxPosition = ItemBoxLists.ClockworkArboretum.FirstOrDefault(x => x.Value == fallbackPosition); break;
                        case "Bakunawa3": itemBoxPosition = ItemBoxLists.InversionDynamo.FirstOrDefault(x => x.Value == fallbackPosition); break;
                        case "Bakunawa4": itemBoxPosition = ItemBoxLists.LunarCannon.FirstOrDefault(x => x.Value == fallbackPosition); break;
                        case "Battlesphere_Course1": itemBoxPosition = ItemBoxLists.Battlesphere1.FirstOrDefault(x => x.Value == fallbackPosition); break;
                        case "Battlesphere_Course2": itemBoxPosition = ItemBoxLists.Battlesphere2.FirstOrDefault(x => x.Value == fallbackPosition); break;
                        case "Battlesphere_Course3": itemBoxPosition = ItemBoxLists.Battlesphere3.FirstOrDefault(x => x.Value == fallbackPosition); break;
                        case "Battlesphere_Course4": itemBoxPosition = ItemBoxLists.Battlesphere4.FirstOrDefault(x => x.Value == fallbackPosition); break;
                        case "Battlesphere_Course5": itemBoxPosition = ItemBoxLists.Battlesphere5.FirstOrDefault(x => x.Value == fallbackPosition); break;
                        case "Battlesphere_Course6": itemBoxPosition = ItemBoxLists.Battlesphere6.FirstOrDefault(x => x.Value == fallbackPosition); break;
                        case "Battlesphere_Course7": itemBoxPosition = ItemBoxLists.Battlesphere7.FirstOrDefault(x => x.Value == fallbackPosition); break;
                        case "Battlesphere_Course8": itemBoxPosition = ItemBoxLists.Battlesphere8.FirstOrDefault(x => x.Value == fallbackPosition); break;
                        case "Battlesphere_Arena": itemBoxPosition = ItemBoxLists.BattlesphereArena.FirstOrDefault(x => x.Value == fallbackPosition); break;
                    }

                    // If we've still come up empty, then throw an error.
                    // TODO: Check if this item box is a bomb, as we're ignoring those. We have to do that check here to account for the Items to Bombs Brave Stone.
                    if (itemBoxPosition.Key == "" || itemBoxPosition.Value == Vector2.zero)
                    {
                        Plugin.consoleLog.LogError($"No item box found for position {__instance.position} in stage ID {FPStage.currentStage.stageID} ({FPStage.currentStage.stageName})!");
                        return -1;
                    }

                    // Return the index of this chest's location.
                    return Plugin.session.Locations.GetLocationIdFromName("Freedom Planet 2", itemBoxPosition.Key);
                }

                // TODO: Check if this item box is a bomb, as we're ignoring those. We have to do that check here to account for the Items to Bombs Brave Stone.
                // Throw an error if we didn't find a fallback position.
                Plugin.consoleLog.LogError($"No item box found for position {__instance.position} in stage ID {FPStage.currentStage.stageID} ({FPStage.currentStage.stageName})!");
                return -1;
            }

            // Return the index of this chest's location.
            return Plugin.session.Locations.GetLocationIdFromName("Freedom Planet 2", itemBoxPosition.Key);
        }
    }
}
