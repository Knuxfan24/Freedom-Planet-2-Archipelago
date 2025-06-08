namespace Freedom_Planet_2_Archipelago.CustomData
{
    internal class ItemDescriptor
    {
        /// <summary>
        /// The item names that this description and sprite is used for.
        /// </summary>
        public string[] ItemNames { get; set; }

        /// <summary>
        /// The description to be shown for this item in the shop.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The name of the sprite to be loaded for this item.
        /// </summary>
        public string SpriteName { get; set; }
    }
}
