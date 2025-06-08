using Archipelago.MultiClient.Net.Models;
using System.Linq;
using System.Reflection.Emit;

namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class MenuArenaChallengeSelectPatcher
    {
        // Set up an array of sprites for the challenge rewards.
        public static Sprite[] Sprites;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuArenaChallengeSelect), "Start")]
        static void GetChallengeRewardSprites()
        {
            // Only do this if we haven't already gotten the sprites.
            if (Sprites != null)
                return;

            // Set up a dictionary to hold our scouted locations.
            Dictionary<long, ScoutedItemInfo> _ScoutedLocationInfo = [];

            // Set up a list of location indices.
            List<long> locationIDs =
            [
                Plugin.session.Locations.GetLocationIdFromName("Manual_FreedomPlanet2_Knuxfan24", "Beginner's Gauntlet"),
                Plugin.session.Locations.GetLocationIdFromName("Manual_FreedomPlanet2_Knuxfan24", "Battlebot Battle Royale"),
                Plugin.session.Locations.GetLocationIdFromName("Manual_FreedomPlanet2_Knuxfan24", "Hero Battle Royale"),
                Plugin.session.Locations.GetLocationIdFromName("Manual_FreedomPlanet2_Knuxfan24", "Kalaw's Challenge"),
                Plugin.session.Locations.GetLocationIdFromName("Manual_FreedomPlanet2_Knuxfan24", "Army of One"),
                Plugin.session.Locations.GetLocationIdFromName("Manual_FreedomPlanet2_Knuxfan24", "Ring-Out Challenge"),
                Plugin.session.Locations.GetLocationIdFromName("Manual_FreedomPlanet2_Knuxfan24", "Flip Fire Gauntlet"),
                Plugin.session.Locations.GetLocationIdFromName("Manual_FreedomPlanet2_Knuxfan24", "Vanishing Maze"),
                Plugin.session.Locations.GetLocationIdFromName("Manual_FreedomPlanet2_Knuxfan24", "Mondo Condo"),
                Plugin.session.Locations.GetLocationIdFromName("Manual_FreedomPlanet2_Knuxfan24", "Birds of Prey"),
                Plugin.session.Locations.GetLocationIdFromName("Manual_FreedomPlanet2_Knuxfan24", "Battlebot Revenge"),
                Plugin.session.Locations.GetLocationIdFromName("Manual_FreedomPlanet2_Knuxfan24", "Mach Speed Melee"),
                Plugin.session.Locations.GetLocationIdFromName("Manual_FreedomPlanet2_Knuxfan24", "Galactic Rumble"),
                Plugin.session.Locations.GetLocationIdFromName("Manual_FreedomPlanet2_Knuxfan24", "Stop and Go"),
                Plugin.session.Locations.GetLocationIdFromName("Manual_FreedomPlanet2_Knuxfan24", "Mecha Madness"),
                Plugin.session.Locations.GetLocationIdFromName("Manual_FreedomPlanet2_Knuxfan24", "Rolling Thunder"),
                Plugin.session.Locations.GetLocationIdFromName("Manual_FreedomPlanet2_Knuxfan24", "Blast from the Past"),
                Plugin.session.Locations.GetLocationIdFromName("Manual_FreedomPlanet2_Knuxfan24", "Bubble Battle"),
            ];

            // Scout the locations for the Battlesphere.
            Plugin.session.Locations.ScoutLocationsAsync(HandleScoutInfo, [.. locationIDs]);

            // Wait for the scout to finish before continuing.
            while (_ScoutedLocationInfo.Count < 18)
                System.Threading.Thread.Sleep(1);

            // Get the sprites for the items in the Battlesphere.
            List<Sprite> sprites = [];
            for (int i = 0; i < 18; i++)
                sprites.Add(Helpers.GetItemSprite(_ScoutedLocationInfo.ElementAt(i).Value, true));
            Sprites = [.. sprites];

            void HandleScoutInfo(Dictionary<long, ScoutedItemInfo> scoutedLocationInfo) => _ScoutedLocationInfo = scoutedLocationInfo;
        }

        /// <summary>
        /// Sets up the challenge list.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuArenaChallengeSelect), "Start")]
        static void SetupChallengeList(MenuArenaChallengeSelect __instance)
        {
            // Force the key count down to 18 if its somehow higher than that.
            if (Plugin.save.BattlesphereKeyCount > 18)
                Plugin.save.BattlesphereKeyCount = 18;

            // Remove the two boss rush challenges from the reward list to remove them from the visual challenge list.
            // This list in the OG makes no sense (as it has an extra entry in it?), but this seems to work.
            List<int> trimmedRewards = [.. __instance.challengeRewards];
            trimmedRewards.RemoveAt(20);
            trimmedRewards.RemoveAt(19);
            trimmedRewards.RemoveAt(18);
            __instance.challengeRewards = [.. trimmedRewards];

            // Set every challenge unlock requirement to 39.
            for (int challengeIndex = 0; challengeIndex < __instance.challengeUnlockRequirement.Length; challengeIndex++)
                __instance.challengeUnlockRequirement[challengeIndex] = 39;

            // Loop through based on the number of Battlesphere Keys we have and unlock the corrosponding challenge.
            for (int keyIndex = 0; keyIndex < Plugin.save.BattlesphereKeyCount; keyIndex++)
                __instance.challengeUnlockRequirement[keyIndex] = -1;

            // Replace the Return label with Exit.
            __instance.gameObject.transform.GetChild(1).GetChild(7).GetChild(0).GetComponent<TextMesh>().text = "Exit";
        }

        /// <summary>
        /// Unlocks challenges in case a key comes in while on the menu.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuArenaChallengeSelect), "Update")]
        static void UnlockChallenges(ref MenuText[] ___challengeList, ref bool[] ___challengeUnlocked, ref int ___challengeIDOffset)
        {
            // Force the key count down to 18 if its somehow higher than that.
            if (Plugin.save.BattlesphereKeyCount > 18)
                Plugin.save.BattlesphereKeyCount = 18;
            
            // Loop through the keys we have.
            for (int keyIndex = 0; keyIndex < Plugin.save.BattlesphereKeyCount; keyIndex++)
            {
                // Unlock this key's challenge.
                ___challengeUnlocked[keyIndex] = true;

                // Get the visual components of this challenge's banner.
                TextMesh challengeTextMesh = ___challengeList[keyIndex].GetComponent<TextMesh>();
                SuperTextMesh challengeSuperTextMesh = ___challengeList[keyIndex].GetComponent<SuperTextMesh>();
                FPHudDigit challengeBanner = ___challengeList[keyIndex].GetComponentInChildren<FPHudDigit>();

                // Set the two text meshes to this challenge's name.
                if (challengeTextMesh != null)
                    challengeTextMesh.text = FPSaveManager.GetChallengeName(keyIndex + ___challengeIDOffset - 1);
                if (challengeSuperTextMesh != null)
                    challengeSuperTextMesh.text = FPSaveManager.GetChallengeName(keyIndex + ___challengeIDOffset - 1);

                // Set this challenge banner's sprite.
                if (challengeBanner != null)
                    challengeBanner.SetDigitValue(keyIndex);
            }
        }

        /// <summary>
        /// Sets the sprite showing the item that is locked by the selected challenge.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuArenaChallengeSelect), "State_Challenge")]
        static void SetSprites(ref bool[] ___challengeUnlocked, ref int[] ___slotID, ref int ___challengeSelection, MenuArenaChallengeSelect __instance, ref int ___challengeIDOffset, ref GameObject ___rewardCheckmark)
        {
            // Check if the currently selected challenge is unlocked and set its sprite accordingly.
            // If it isn't, then just remove the sprite.
            if (___challengeUnlocked[___slotID[___challengeSelection]])
                __instance.rewardItem.sprite = Sprites[___challengeSelection];
            else
                __instance.rewardItem.sprite = null;


            // Check if the currently selected challenge has been cleared and activate the checkmark if needed.
            if (FPSaveManager.challengeRecord[___slotID[___challengeSelection] + ___challengeIDOffset] <= 0)
                ___rewardCheckmark.SetActive(false);
            else
                ___rewardCheckmark.SetActive(true);
        }

        /// <summary>
        /// Removes the lines that causes the menu to lower when the return button is selected.
        /// </summary>
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(MenuArenaChallengeSelect), "State_Options")]
        static IEnumerable<CodeInstruction> RemoveMenuDropStateOptions(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 122; i <= 126; i++)
                codes[i].opcode = OpCodes.Nop;

            return codes.AsEnumerable();
        }
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(MenuArenaChallengeSelect), "State_Challenge")]
        static IEnumerable<CodeInstruction> RemoveMenuDropStateChallenge(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 748; i <= 752; i++)
                codes[i].opcode = OpCodes.Nop;

            return codes.AsEnumerable();
        }

        /// <summary>
        /// Exits the menu.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuArenaChallengeSelect), "State_Exit")]
        static void Exit(MenuArenaChallengeSelect __instance)
        {
            // Find the menu's screen transition object.
            FPScreenTransition transition = GameObject.Find("Screen Transition").GetComponent<FPScreenTransition>();

            // Set the transition's type to wipe.
            transition.transitionType = FPTransitionTypes.WIPE;

            // Set the speed of the transition.
            transition.transitionSpeed = 48f;

            // Load the Classic Menu.
            transition.sceneToLoad = "ClassicMenu";

            // Set the transition to pure black.
            transition.SetTransitionColor(0f, 0f, 0f);

            // Start the transition.
            transition.BeginTransition();

            // Stop the music.
            FPAudio.StopMusic();

            // Play the menu wipe sound.
            FPAudio.PlayMenuSfx(3);

            // Swap to the nothing state.
            __instance.state = State_Nothing;
        }

        /// <summary>
        /// Dummy state so the menu does nothing.
        /// </summary>
        static void State_Nothing() { }
    }
}
