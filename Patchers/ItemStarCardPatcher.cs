using Archipelago.MultiClient.Net.Models;
using System.Linq;
using UnityEngine.SceneManagement;

namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class ItemStarCardPatcher
    {
        /// <summary>
        /// Replaces the sprite of the Star Card depending on the item that the location holds.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemStarCard), "Start")]
        static void ReplaceSprite(ItemStarCard __instance)
        {
            // Set up a value to store the stage name.
            string stageName = string.Empty;

            // Get the stage name depending on the active scene.
            switch (SceneManager.GetActiveScene().name)
            {
                case "AirshipSigwada": stageName = "Airship Sigwada"; break;
                case "AncestralForge": stageName = "Ancestral Forge"; break;
                case "Auditorium": stageName = "Auditorium"; break;
                case "AvianMuseum": stageName = "Avian Museum"; break;
                case "Bakunawa1": stageName = "Bakunawa Rush"; break;
                case "Bakunawa1Boss": stageName = "Refinery Room"; break;
                case "Bakunawa2": stageName = "Clockwork Arboretum"; break;
                case "Bakunawa3": stageName = "Inversion Dynamo"; break;
                case "Bakunawa4": stageName = "Lunar Cannon"; break;
                case "DiamondPoint": stageName = "Diamond Point"; break;
                case "DragonValley": stageName = "Dragon Valley"; break;
                case "GlobeOpera1": stageName = "Globe Opera 1"; break;
                case "GlobeOpera2": stageName = "Globe Opera 2"; break;
                case "GravityBubble": stageName = "Gravity Bubble"; break;
                case "LightningTower": stageName = "Lightning Tower"; break;
                case "MagmaStarscape": stageName = "Magma Starscape"; break;
                case "NalaoLake": stageName = "Nalao Lake"; break;
                case "PhoenixHighway": stageName = "Phoenix Highway"; break;
                case "RobotGraveyard": stageName = "Robot Graveyard"; break;
                case "ShadeArmory": stageName = "Shade Armory"; break;
                case "ShenlinPark": stageName = "Shenlin Park"; break;
                case "SkyBridge": stageName = "Sky Bridge"; break;
                case "Snowfields": stageName = "Snowfields"; break;
                case "TidalGate": stageName = "Tidal Gate"; break;
                case "TigerFalls": stageName = "Tiger Falls"; break;
                case "ZaoLand": stageName = "Zao Land"; break;
                case "ZulonJungle": stageName = "Zulon Jungle"; break;
            }

            // If we haven't found a stage name, then abort.
            if (stageName == string.Empty)
                return;

            // Scout the clear location for this stage.
            ScoutedItemInfo _scoutedLocationInfo = null;
            Plugin.session.Locations.ScoutLocationsAsync(HandleScoutInfo, [Plugin.session.Locations.GetLocationIdFromName("Manual_FreedomPlanet2_Knuxfan24", $"{stageName} - Clear")]);

            // Pause operation until the location is scouted.
            while (_scoutedLocationInfo == null)
                System.Threading.Thread.Sleep(1);

            // If this location has a Star Card, then abort.
            if (_scoutedLocationInfo.ItemName == "Star Card")
                return;

            // Disable the Star Card's animator.
            __instance.GetComponent<Animator>().enabled = false;

            // Set the Star Card's scale to 2.
            __instance.scale = new(2f, 2f, 2f);

            // Replace the Star Card's sprite with the approriate one for our item.
            __instance.GetComponent<SpriteRenderer>().sprite = Helpers.GetItemSprite(_scoutedLocationInfo, true);

            void HandleScoutInfo(Dictionary<long, ScoutedItemInfo> scoutedLocationInfo) => _scoutedLocationInfo = scoutedLocationInfo.First().Value;
        }
    }
}
