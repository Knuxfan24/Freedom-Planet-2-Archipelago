namespace Freedom_Planet_2_Archipelago.CustomData
{
    public class ArchipelagoItem
    {
        /// <summary>
        /// The name of the item being received.
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Who sent the item.
        /// </summary>
        public string Source { get; set; }

        public ArchipelagoItem() { }
        public ArchipelagoItem(string itemName, string source)
        {
            ItemName = itemName;
            Source = source;
        }
    }
}
