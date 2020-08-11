using Newtonsoft.Json;

namespace PlayerCompanion
{
    /// <summary>
    /// The configuration of the mod.
    /// </summary>
    public class InventoryConfiguration
    {
        /// <summary>
        /// If the LemonUI Menu should be used to navigate the inventory items.
        /// </summary>
        [JsonProperty("use_menu")]
        public bool UseMenu { get; set; } = true;
        /// <summary>
        /// If the inventory should be shared between peds.
        /// </summary>
        [JsonProperty("shared_inventory")]
        public bool SharedInventory { get; set; } = false;
    }
}
