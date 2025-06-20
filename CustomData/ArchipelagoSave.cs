﻿using Newtonsoft.Json;

namespace Freedom_Planet_2_Archipelago.CustomData
{
    public class ArchipelagoSave
    {
        /// <summary>
        /// The slot we're saving FP2's actual save into.
        /// </summary>
        public int SaveSlot { get; set; }

        #region Item Quantities
        /// <summary>
        /// The amount of Star Cards we've gotten from the server.
        /// </summary>
        [JsonIgnore]
        public int StarCardCount { get; set; }

        /// <summary>
        /// The amount of Time Capsules we've gotten from the server.
        /// </summary>
        [JsonIgnore]
        public int TimeCapsuleCount { get; set; }

        /// <summary>
        /// The amount of Battlesphere Keys we've gotten from the server.
        /// </summary>
        [JsonIgnore]
        public int BattlesphereKeyCount { get; set; }

        /// <summary>
        /// The amount of Gold Gems we've gotten from the server.
        /// </summary>
        public int GoldGemCount { get; set; }

        /// <summary>
        /// The amount of Mirror Traps we've gotten from the server.
        /// </summary>
        public int MirrorTrapCount { get; set; }
        #endregion

        /// <summary>
        /// The Chapter Unlocks we've received from the server.
        /// </summary>
        [JsonIgnore]
        public bool[] ChapterUnlocks { get; set; } = new bool[8];

        /// <summary>
        /// The Chest Tracers we've received from the server.
        /// </summary>
        [JsonIgnore]
        public bool[] ChestTracers { get; set; } = new bool[24];

        /// <summary>
        /// The Brave Stones we've received from the server.
        /// </summary>
        [JsonIgnore]
        public bool[] BraveStones { get; set; } = new bool[29];

        /// <summary>
        /// The Potions we've received from the server.
        /// </summary>
        [JsonIgnore]
        public bool[] Potions { get; set; } = new bool[9];

        public ArchipelagoSave() { }
        public ArchipelagoSave(int slot)
        {
            SaveSlot = slot;
        }
    }
}
