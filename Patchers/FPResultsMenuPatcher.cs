using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Packets;
using System.Linq;
using UnityEngine.SceneManagement;

namespace Freedom_Planet_2_Archipelago.Patchers
{
    internal class FPResultsMenuPatcher
    {
        /// <summary>
        /// Sends a location check upon clearing a stage.
        /// </summary>
        /// <param name="___challengeID">The challenge ID this results menu has, used for Battlesphere locations.</param>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPResultsMenu), "Start")]
        static void SendLocationCheck(ref int ___challengeID)
        {
            // Set up a value to hold the location name.
            string locationName = string.Empty;

            // Determine the location name based on the Stage ID.
            switch (FPStage.currentStage.stageID)
            {
                case 1: locationName = "Dragon Valley - Clear"; break;
                case 2: locationName = "Shenlin Park - Clear"; break;
                case 3: locationName = "Avian Museum - Clear"; break;
                case 5: locationName = "Tiger Falls - Clear"; break;
                case 6: locationName = "Robot Graveyard - Clear"; break;
                case 7: locationName = "Shade Armory - Clear"; break;
                case 8: locationName = "Snowfields - Clear"; break;
                case 9: locationName = "Phoenix Highway - Clear"; break;
                case 10: locationName = "Zao Land - Clear"; break;
                case 11: locationName = "Globe Opera 1 - Clear"; break;
                case 12: locationName = "Globe Opera 2 - Clear"; break;
                case 13: locationName = "Auditorium - Clear"; break;
                case 14: locationName = "Palace Courtyard - Clear"; break;
                case 15: locationName = "Tidal Gate - Clear"; break;
                case 16: locationName = "Zulon Jungle - Clear"; break;
                case 17: locationName = "Nalao Lake - Clear"; break;
                case 18: locationName = "Sky Bridge - Clear"; break;
                case 19: locationName = "Lightning Tower - Clear"; break;
                case 20: locationName = "Ancestral Forge - Clear"; break;
                case 21: locationName = "Magma Starscape - Clear"; break;
                case 22: locationName = "Diamond Point - Clear"; break;
                case 23: locationName = "Gravity Bubble - Clear"; break;
                case 24: locationName = "Bakunawa Rush - Clear"; break;
                case 25: locationName = "Refinery Room - Clear"; break;
                case 26: locationName = "Clockwork Arboretum - Clear"; break;
                case 27: locationName = "Inversion Dynamo - Clear"; break;
                case 28: locationName = "Lunar Cannon - Clear"; break;
                case 29: locationName = "Merga - Clear"; break;
                case 30: locationName = "Weapon's Core - Clear"; break;
                case 32: locationName = "Bakunawa Chase - Clear"; break;

                // The Battlesphere and the Sigwada share the same ID (as the Battlesphere is TECHNICALLY a HUB).
                case 4:
                    if (SceneManager.GetActiveScene().name == "AirshipSigwada")
                    {
                        locationName = "Airship Sigwada - Clear";
                    }
                    break;
                case 31:
                    switch (___challengeID)
                    {
                        case 1: locationName = "Beginner's Gauntlet"; break;
                        case 2: locationName = "Battlebot Battle Royale"; break;
                        case 3: locationName = "Hero Battle Royale"; break;
                        case 4: locationName = "Kalaw's Challenge"; break;
                        case 5: locationName = "Army of One"; break;
                        case 6: locationName = "Ring-Out Challenge"; break;
                        case 7: locationName = "Flip Fire Gauntlet"; break;
                        case 8: locationName = "Vanishing Maze"; break;
                        case 9: locationName = "Mondo Condo"; break;
                        case 10: locationName = "Birds of Prey"; break;
                        case 11: locationName = "Battlebot Revenge"; break;
                        case 12: locationName = "Mach Speed Melee"; break;
                        case 13: locationName = "Galactic Rumble"; break;
                        case 14: locationName = "Stop and Go"; break;
                        case 15: locationName = "Mecha Madness"; break;
                        case 16: locationName = "Rolling Thunder"; break;
                        case 17: locationName = "Blast from the Past"; break;
                        case 18: locationName = "Bubble Battle"; break;

                        // Throw an error if we have an unhandled challenge ID.
                        default: Plugin.consoleLog.LogError($"Challenge ID {___challengeID} on {SceneManager.GetActiveScene().name} not yet handled!"); break;
                    }

                    break;

                // Throw an error if we have an unhandled stage ID.
                default: Plugin.consoleLog.LogError($"No stage clear handling is present for stage ID {FPStage.currentStage.stageID} ({FPStage.currentStage.stageName})!"); break;
            }

            // Check if we actually got a location name before continuing.
            if (locationName == string.Empty)
                return;

            // Get the index of the location for this stage clear.
            long locationIndex = Plugin.session.Locations.GetLocationIdFromName("Freedom Planet 2", locationName);

            // If this location exists, then complete the check of it.
            if (Helpers.CheckLocationExists(locationIndex))
            {
                // Complete the location check for this index.
                Plugin.session.Locations.CompleteLocationChecks(locationIndex);

                // Scout the location we just completed.
                ScoutedItemInfo _scoutedLocationInfo = null;
                Plugin.session.Locations.ScoutLocationsAsync(HandleScoutInfo, [locationIndex]);

                // Pause operation until the location is scouted.
                while (_scoutedLocationInfo == null)
                    System.Threading.Thread.Sleep(1);

                // Add a message to the queue if this item is for someone else.
                if (_scoutedLocationInfo.Player.Name != Plugin.session.Players.GetPlayerName(Plugin.session.ConnectionInfo.Slot))
                    Plugin.sentMessageQueue.Add($"Found {_scoutedLocationInfo.Player.Name}'s {_scoutedLocationInfo.ItemName}.");

                void HandleScoutInfo(Dictionary<long, ScoutedItemInfo> scoutedLocationInfo) => _scoutedLocationInfo = scoutedLocationInfo.First().Value;
            }

            // If the location was Weapon's Core's clear, then send a goal packet to the server too.
            if (locationName == "Weapon's Core - Clear")
            {
                StatusUpdatePacket goalPacket = new() { Status = ArchipelagoClientState.ClientGoal };
                Plugin.session.Socket.SendPacketAsync(goalPacket);
            }
        }
    }
}
