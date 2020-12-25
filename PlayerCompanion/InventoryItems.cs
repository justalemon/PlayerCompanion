using GTA.UI;
using LemonUI.Elements;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace PlayerCompanion
{
    /// <summary>
    /// Serializes and Deserializes inventory items.
    /// </summary>
    public class ItemConverter : JsonConverter<Item>
    {
        /// <summary>
        /// Reads an <see cref="Item"/>.
        /// </summary>
        /// <returns>The restored <see cref="Item"/>.</returns>
        public override Item ReadJson(JsonReader reader, Type objectType, Item existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Load the entire thing into a JSON Object
            JObject @object = JObject.Load(reader);

            // If there is no Type in the object, raise an exception
            if (!@object.ContainsKey("type"))
            {
                throw new KeyNotFoundException("There was no Type specified in the item.");
            }

            // Get the total number of items
            int count = @object.ContainsKey("count") ? (int)@object["count"] : 1;

            // Try to get the type of the object
            Type type = Type.GetType((string)@object["type"], false);
            // If the type was not found, return an invalid type
            if (type == null)
            {
                Notification.Show($"~o~Warning~s~: Item Type {(string)@object["type"]} was not found. Make sure that all of the required mods are installed and reload PlayerCompanion.");
                return new InvalidItem((string)@object["type"], count);
            }

            // Create the item with the specified class
            Item item = (Item)Activator.CreateInstance(type);

            // If the item inherits from the stackable class, store the number of items
            if (item is StackableItem stackableItem)
            {
                stackableItem.Count = count;
            }

            // Finally, return the item that we just created
            return item;
        }
        /// <summary>
        /// Writes an <see cref="Item"/>.
        /// </summary>
        public override void WriteJson(JsonWriter writer, Item value, JsonSerializer serializer)
        {
            // Create a new JObject to store the values
            JObject @object = new JObject();

            // If this is an invalid item, save the type in the property
            if (value is InvalidItem invalid)
            {
                @object["type"] = invalid.Type;
            }
            // Otherwise, save the real Type
            else
            {
                @object["type"] = value.GetType().AssemblyQualifiedName;
            }

            // If the item type inherits from stackable, save the count
            if (value is StackableItem stackable)
            {
                @object["count"] = stackable.Count;
            }

            // And save it
            @object.WriteTo(writer);
        }
    }

    /// <summary>
    /// Represents an inventory item that can be stored in stacks (like Minecraft).
    /// </summary>
    public abstract class StackableItem : Item
    {
        #region Fields

        private int count = 0;

        #endregion

        #region Properties

        /// <summary>
        /// The total number of items in this Stack.
        /// </summary>
        public int Count
        {
            get => count;
            set
            {
                count = value;
                CountChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// The maximum number of items that can be stored in a Stack.
        /// </summary>
        public virtual int Maximum { get; } = int.MaxValue;

        #endregion

        #region Events

        /// <summary>
        /// Event triggered when the Count of items changes.
        /// </summary>
        public event EventHandler CountChanged;

        #endregion
    }

    /// <summary>
    /// Represents a single Inventory Item.
    /// </summary>
    [JsonConverter(typeof(ItemConverter))]
    public abstract class Item
    {
        #region Fields

        internal PedInventory inventory = null;

        #endregion

        #region Properties

        /// <summary>
        /// The name of the item shown on specific inventory interfaces.
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// The description of the name shown on specific inventory interfaces.
        /// </summary>
        public virtual string Description => "No Description Available.";
        /// <summary>
        /// A custom white Sprite used as an icon for the item.
        /// </summary>
        public abstract ScaledTexture Icon { get; }
        /// <summary>
        /// The Monetary value of this item.
        /// </summary>
        public virtual int Value => 0;
        /// <summary>
        /// The inventory that this Item is part of.
        /// </summary>
        public PedInventory Inventory => inventory;

        #endregion

        #region Events

        /// <summary>
        /// Event triggered when the Item has been used.
        /// </summary>
        public event EventHandler Used;

        #endregion

        #region Functions

        /// <summary>
        /// Removes the Item from it's existing inventory.
        /// </summary>
        public void Remove()
        {
            if (inventory == null)
            {
                return;
            }

            inventory.Remove(this);
            inventory = null;
        }
        /// <summary>
        /// Uses the item.
        /// </summary>
        public void Use() => Used?.Invoke(this, EventArgs.Empty);

        #endregion
    }

    /// <summary>
    /// Represents an Item that could not be restored when the script was loaded.
    /// </summary>
    public sealed class InvalidItem : StackableItem
    {
        #region Properties

        /// <summary>
        /// The Name of this Invalid Item.
        /// </summary>
        public override string Name { get; }
        /// <summary>
        /// The Price of this Invalid Item.
        /// </summary>
        public override int Value { get; } = 0;
        /// <summary>
        /// The Type that could not be created during initialization.
        /// </summary>
        public string Type { get; }
        /// <summary>
        /// The icon of the invalid item.
        /// </summary>
        public override ScaledTexture Icon { get; } = new ScaledTexture("timerbar_icons", "pickup_random");

        #endregion

        #region Constructors

        internal InvalidItem(string type, int count)
        {
            Name = $"Invalid ({type})";
            Type = type;
            Count = count;
        }

        #endregion
    }
}
