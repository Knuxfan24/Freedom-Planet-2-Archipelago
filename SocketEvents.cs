using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Packets;
using Freedom_Planet_2_Archipelago.Patchers;

namespace Freedom_Planet_2_Archipelago
{
    internal class SocketEvents
    {
        /// <summary>
        /// Event handler for when we receive an item from the multiworld.
        /// </summary>
        public static void Socket_ReceiveItem(Archipelago.MultiClient.Net.Helpers.ReceivedItemsHelper helper)
        {
            // Get the item the multiworld sent us and handle adding it to our queue.
            SetUpQueue(helper.PeekItem());

            // Dequeue this item.
            helper.DequeueItem();
        }

        /// <summary>
        /// Sets up the queue of items to be received.
        /// </summary>
        /// <param name="item">The item we're handling.</param>
        public static void SetUpQueue(ItemInfo item)
        {
            // Set up a queued item entry for this item.
            ArchipelagoItem queuedItem = new(item.ItemName, item.Player.Name);

            // Set up a boolean to check if we've got an item of the same type and source in the queue already.
            bool foundExistingInQueue = false;

            // Loop through each queued item.
            foreach (ArchipelagoItem itemInQueue in Plugin.itemQueue.Keys)
            {
                // Check if this item's name and source matches this dictionary entry.
                if (itemInQueue.ItemName == queuedItem.ItemName && itemInQueue.Source == queuedItem.Source)
                {
                    // Increment this dictionary entry's count.
                    Plugin.itemQueue[itemInQueue]++;

                    // Set our flag.
                    foundExistingInQueue = true;

                    // Stop the rest of the foreach loop, as its a pointless waste of time now.
                    break;
                }
            }

            // If we haven't found a version of this item in the queue already, then add one.
            if (!foundExistingInQueue)
                Plugin.itemQueue.Add(queuedItem, 1);
        }

        /// <summary>
        /// Event handler for when we receive a DeathLink from the multiworld.
        /// </summary>
        public static void Socket_ReceiveDeathLink(DeathLink deathLink)
        {
            // Set up the message showing our DeathLink source.
            string notifyMessage = $"DeathLink received from {deathLink.Source}";

            // Present the cause and source of the DeathLink.
            if (deathLink.Cause != null)
                notifyMessage = $"{deathLink.Cause}";

            // Set the player's DeathLink flags accordingly.
            FPPlayerPatcher.canSendDeathLink = false;
            FPPlayerPatcher.hasBufferedDeathLink = true;

            // Add the message to our sent queue so it'll take priority for the message label.
            Plugin.sentMessageQueue.Add(notifyMessage);
        }

        /// <summary>
        /// Event handler for when we receive a packet from the Multiworld, only used for RingLink handling.
        /// </summary>
        public static void Socket_LinkPackets(ArchipelagoPacketBase packet)
        {
            switch (packet)
            {
                case BouncedPacket bouncedPacket when bouncedPacket.Tags.Contains("RingLink"):
                    // Ignore the packet if we're the one who sent it or RingLink is disabled.
                    if (bouncedPacket.Data["source"].ToObject<int>() == Plugin.session.ConnectionInfo.Slot || ((long)Plugin.slotData["ring_link"] == 0))
                        return;

                    // Get the value from the RingLink.
                    int ringLinkValue = bouncedPacket.Data["amount"].ToObject<int>();

                    // Check if the player exists.
                    if (FPPlayerPatcher.player != null)
                    {
                        // Disable our send flag so we don't send the RingLink value back.
                        FPSaveManagerPatcher.DisableRingLinkSend = true;

                        // If the RingLink value is positive, then give us that amount of crystals.
                        if (ringLinkValue > 0)
                        {
                            for (int ringIndex = 0; ringIndex < ringLinkValue; ringIndex++)
                                FPSaveManager.AddCrystal(FPPlayerPatcher.player);
                        }

                        // If the RingLink value is negative, then remove that amount.
                        if (ringLinkValue < 0)
                        {
                            FPPlayerPatcher.player.crystals -= ringLinkValue;
                            FPPlayerPatcher.player.totalCrystals += ringLinkValue;

                            // Clamp the values to 0 and the extra life cost respectively.
                            if (FPPlayerPatcher.player.totalCrystals < 0)
                                FPPlayerPatcher.player.totalCrystals = 0;
                            if (FPPlayerPatcher.player.crystals > FPPlayerPatcher.player.extraLifeCost)
                                FPPlayerPatcher.player.crystals = FPPlayerPatcher.player.extraLifeCost;

                            // Check if the player has a shield.
                            if (FPPlayerPatcher.player.shieldHealth > 0)
                            {
                                // Play the approriate sound effect for the shield.
                                if (FPPlayerPatcher.player.shieldHealth > 1)
                                    FPAudio.PlaySfx(FPPlayerPatcher.player.sfxShieldHit);
                                else
                                    FPAudio.PlaySfx(FPPlayerPatcher.player.sfxShieldBreak);

                                // Reduce the player's shield health.
                                FPPlayerPatcher.player.shieldHealth -= 1;

                                // Create the shield hit flash.
                                ShieldHit shieldHit2 = (ShieldHit)FPStage.CreateStageObject(ShieldHit.classID, FPPlayerPatcher.player.position.x, FPPlayerPatcher.player.position.y);
                                shieldHit2.SetParentObject(FPPlayerPatcher.player);
                                shieldHit2.remainingDuration = 60f - Mathf.Min((float)FPPlayerPatcher.player.shieldHealth * 3f, 30f);
                            }

                            // Check if the player has health to lose (a RingLink should never kill).
                            else if (FPPlayerPatcher.player.health > 0)
                            {
                                // Play the damage sound effect.
                                FPAudio.PlaySfx(FPPlayerPatcher.player.sfxHurt);

                                // Either remove a health petal, or floor the health down to 0.
                                if (FPPlayerPatcher.player.health > 1f)
                                    FPPlayerPatcher.player.health -= 1f;
                                else
                                    FPPlayerPatcher.player.health = 0;
                            }
                        }

                        // Reenable our send flag.
                        FPSaveManagerPatcher.DisableRingLinkSend = false;
                    }

                    // If the player doesn't exist, then apply the RingLink straight to the save.
                    else
                    {
                        FPSaveManager.totalCrystals += ringLinkValue;

                        // Clamp the value to 0.
                        if (FPSaveManager.totalCrystals < 0)
                            FPSaveManager.totalCrystals = 0;
                    }

                    break;

                case BouncedPacket bouncedPacket when bouncedPacket.Tags.Contains("TrapLink"):
                    // Ignore the packet if we're the one who sent it or TrapLink is disabled.
                    if (bouncedPacket.Data["source"].ToObject<string>() == Plugin.session.Players.GetPlayerName(Plugin.session.ConnectionInfo.Slot) || ((long)Plugin.slotData["trap_link"] == 0))
                        return;

                    // Whether or not we've been in a position to receive the linked trap.
                    bool received = false;

                    // Handle the traps and what they should link to.
                    // TODO: Trap Brave Stones as well? A couple of traps could map better to those.
                    switch (bouncedPacket.Data["trap_name"].ToObject<string>())
                    {
                        case "Swap Trap":
                            if (FPPlayerPatcher.player != null && FPStage.objectsRegistered)
                                AddTrap(bouncedPacket, "Swap Trap");
                            break;

                        case "Mirror Trap":
                        case "Confusion Trap": // Sonic Adventure 2
                        case "Reverse Trap": // Super Mario World, Sonic Adventure DX and Sonic Adventure 2
                            AddTrap(bouncedPacket, "Mirror Trap");
                            break;

                        case "Pie Trap":
                        case "Chaos Control Trap": // Sonic Adventure 2
                        case "Stun Trap": // Super Mario World
                            if (FPPlayerPatcher.player != null && FPStage.objectsRegistered)
                                AddTrap(bouncedPacket, "Pie Trap");
                            break;

                        case "Spring Trap":
                            if (FPPlayerPatcher.player != null && FPStage.objectsRegistered)
                                AddTrap(bouncedPacket, "Spring Trap");
                            break;

                        case "PowerPoint Trap":
                            AddTrap(bouncedPacket, "PowerPoint Trap");
                            break;

                        case "Zoom Trap":
                            AddTrap(bouncedPacket, "Zoom Trap");
                            break;

                        case "Aaa Trap":
                        case "OmoTrap": // Sonic Adventure 2
                        case "Exposition Trap": // Sonic Adventure 2
                        case "Cutscene Trap": // Sonic Adventure 2
                        case "Literature Trap": // Super Mario World and Sonic Adventure 2
                            AddTrap(bouncedPacket, "Aaa Trap");
                            break;

                        case "Spike Ball Trap":
                        case "Thwimp Trap": // Super Mario World
                        case "Police Trap": // Sonic Adventure DX
                        case "Buyon Trap": // Sonic Adventure DX
                            if (FPPlayerPatcher.player != null && FPStage.objectsRegistered)
                                AddTrap(bouncedPacket, "Spike Ball Trap");
                            break;

                        case "Pixellation Trap":
                            AddTrap(bouncedPacket, "Pixellation Trap");
                            break;

                        case "Rail Trap":
                        case "Ice Trap": // Super Mario World, Sonic Adventure DX and Sonic Adventure 2
                            AddTrap(bouncedPacket, "Rail Trap");
                            break;
                    }

                    if (received)
                        Plugin.sentMessageQueue.Add($"{bouncedPacket.Data["source"]} linked a {bouncedPacket.Data["trap_name"]}!");

                    void AddTrap(BouncedPacket bouncedPacket, string trapName)
                    {
                        Plugin.TrapLinks.Add(new ArchipelagoItem(trapName, bouncedPacket.Data["source"].ToObject<string>()));
                        received = true;
                    }
                    break;
            }  

        }
    }
}
