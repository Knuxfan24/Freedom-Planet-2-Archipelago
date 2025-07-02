using Newtonsoft.Json;

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
        /// The amount of Mirror Traps we've gotten from the server.
        /// </summary>
        public int MirrorTrapCount { get; set; }

        /// <summary>
        /// The amount of PowerPoint Traps we've gotten from the server.
        /// </summary>
        public int PowerPointTrapCount { get; set; }

        /// <summary>
        /// The amount of Zoom Traps we've gotten from the server.
        /// </summary>
        public int ZoomTrapCount { get; set; }

        /// <summary>
        /// The amount of Pie Traps we've gotten from the server.
        /// </summary>
        public int PieTrapCount { get; set; }

        /// <summary>
        /// The amount of Spring Traps we've gotten from the server.
        /// </summary>
        public int SpringTrapCount { get; set; }

        /// <summary>
        /// The amount of Aaa Traps we've gotten from the server.
        /// </summary>
        public int AaaTrapCount { get; set; }

        /// <summary>
        /// The amount of Spike Ball Traps we've gotten from the server.
        /// </summary>
        public int SpikeBallTrapCount { get; set; }

        /// <summary>
        /// The amount of Pixellation Traps we've gotten from the server.
        /// </summary>
        public int PixellationTrapCount { get; set; }

        /// <summary>
        /// The amount of Rail Traps we've gotten from the server.
        /// </summary>
        public int RailTrapCount { get; set; }

        /// <summary>
        /// The amount of Gold Gems we've gotten from the server.
        /// </summary>
        public int GoldGemCount { get; set; }

        /// <summary>
        /// The amount of Crystals we've gotten from the server.
        /// </summary>
        public int CrystalCount { get; set; }

        /// <summary>
        /// The amount of Extra Lives we've gotten from the server.
        /// </summary>
        public int ExtraLifeCount { get; set; }

        /// <summary>
        /// The amount of Invincibilities we've gotten from the server.
        /// </summary>
        public int InvincibilityCount { get; set; }

        /// <summary>
        /// The amount of Shields we've gotten from the server.
        /// </summary>
        public int ShieldCount { get; set; }

        /// <summary>
        /// The amount of Powerups we've gotten from the server.
        /// </summary>
        public int PowerupCount { get; set; }
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
