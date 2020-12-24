using GTA;
using GTA.Native;
using GTA.UI;
using LemonUI;
using LemonUI.Elements;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Control = GTA.Control;
using Screen = LemonUI.Screen;

namespace PlayerCompanion
{
    /// <summary>
    /// Main class for managing the player information.
    /// </summary>
    public class Companion : Script
    {
        #region Fields

        private static Model lastModel = default;
        private static int nextWeaponUpdate = 0;
        private static int nextWeaponSave = 0;

        private static readonly ScaledText moneyText = new ScaledText(PointF.Empty, "$0", 0.65f, GTA.UI.Font.Pricedown)
        {
            Alignment = Alignment.Right,
            Outline = true

        };

        internal static readonly ObjectPool pool = new ObjectPool();
        internal static readonly InventoryMenu menu = new InventoryMenu();

        internal static string location = Path.Combine(new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)).LocalPath, "PlayerCompanion");
        internal static Configuration config = null; 

        #endregion

        #region Properties

        /// <summary>
        /// If the PlayerCompanion features are ready to be used.
        /// </summary>
        public static bool IsReady { get; private set; } = false;
        /// <summary>
        /// The manager for the Player Money.
        /// </summary>
        public static Wallet Wallet { get; private set; } = new Wallet();
        /// <summary>
        /// Manages the colors in the HUD and Radar.
        /// </summary>
        public static ColorManager Colors { get; private set; } = new ColorManager();
        /// <summary>
        /// Makages the Inventories of the Players.
        /// </summary>
        public static InventoryManager Inventories { get; private set; } = new InventoryManager();
        /// <summary>
        /// Manages your Weapons between peds.
        /// </summary>
        public static WeaponManager Weapons { get; private set; } = new WeaponManager();

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new PlayerCompanion Script.
        /// Please note that this can only be done by SHVDN.
        /// </summary>
        public Companion()
        {
            // Get the assembly that called and it's name
            Assembly assembly = Assembly.GetCallingAssembly();
            AssemblyName name = assembly.GetName();
            // If the name is not ScriptHookVDotNet, raise an exception
            if (name.Name != "ScriptHookVDotNet")
            {
                throw new InvalidOperationException($"PlayerCompanion can only be started by ScriptHookVDotNet (it was called from '{name.Name}').");
            }

            // If there is a configuration file, load it
            string path = Path.Combine(location, "Config.json");
            if (File.Exists(path))
            {
                config = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(path));
            }
            // Otherwise, create a new one and save it
            else
            {
                config = new Configuration();
                Directory.CreateDirectory(location);
                File.WriteAllText(Path.Combine(location, path), JsonConvert.SerializeObject(config));
            }

            // Add the items to the pool
            pool.Add(menu);
            // Finally, add the events that we need
            Tick += Companion_Tick;
            Aborted += Companion_Aborted;
            KeyDown += Companion_KeyDown;
            Inventories.ItemAdded += Inventories_ItemAdded;
            Inventories.ItemRemoved += Inventories_ItemRemoved;
        }

        #endregion

        #region Functions

        /// <summary>
        /// Updates the Text showing the user Money.
        /// </summary>
        private void UpdateMoneyText()
        {
            Screen.SetElementAlignment(GFXAlignment.Right, GFXAlignment.Top);
            PointF position = Screen.GetRealPosition(0, 0);
            Screen.ResetElementAlignment();

            if (Hud.IsComponentActive(HudComponent.WantedStars))
            {
                position.Y += 35;
            }

            moneyText.Position = position;
            moneyText.Text = $"${Wallet.Money}";
        }

        #endregion

        #region Events

        private void Companion_Tick(object sender, EventArgs e)
        {
            // If PlayerCompanion is not ready to work, perform the initialization
            if (!IsReady)
            {
                Inventories.PopulateItems();
                IsReady = true;
            }

            // Process the menu pool
            pool.Process();

            // If the player entered the inventory cheat, toggle it
            if (Game.WasCheatStringJustEntered("inventory"))
            {
                menu.Visible = !menu.Visible;
            }

            // if the notification feed is paused and the user has the character wheel button pressed
            // Draw the current money that the user has
            if (Function.Call<bool>(Hash.THEFEED_IS_PAUSED) && Game.IsControlPressed(Control.CharacterWheel))
            {
                Hud.HideComponentThisFrame(HudComponent.Cash);
                Hud.HideComponentThisFrame(HudComponent.CashChange);
                UpdateMoneyText();
                moneyText.Draw();
            }

            // If the Player Ped Model has been changed, make the required updates
            if (Game.Player.Character.Model != lastModel)
            {
                if (Colors.HasCustomColor(Game.Player.Character.Model))
                {
                    Colors.Apply(Colors.Current);
                }
                else
                {
                    switch ((PedHash)Game.Player.Character.Model)
                    {
                        case PedHash.Franklin:
                        case PedHash.Michael:
                        case PedHash.Trevor:
                            Colors.RestoreDefault();
                            break;
                        default:
                            Notification.Show($"~o~Warning~s~: Ped Model {Game.Player.Character.Model.Hash} does not has a Color set!");
                            Colors.Apply(Color.LightGray);
                            break;
                    }
                }
                if (lastModel != default)
                {
                    Weapons[lastModel]?.Save(lastModel);
                }
                Weapons.Current?.Apply();
                Inventories.Load(Game.Player.Character.Model);

                menu.ReloadItems();

                lastModel = Game.Player.Character.Model;
                nextWeaponUpdate = Game.GameTime + (1000 * 5);
            }

            // If is time to update the weapons that the player has
            if (nextWeaponUpdate <= Game.GameTime)
            {
                Weapons.Current?.Populate();
                nextWeaponUpdate = Game.GameTime + (1000 * 5);
            }

            // If is time to save the weapons
            if (nextWeaponSave <= Game.GameTime)
            {
                Weapons.Current?.Save(Game.Player.Character.Model);
                nextWeaponSave = Game.GameTime + (1000 * 30);
            }
        }

        private void Companion_Aborted(object sender, EventArgs e)
        {
            // Do the required cleanup tasks
            Colors.RestoreDefault();
            // And save the user weapons
            Weapons.Current?.Populate();
            Weapons.Current?.Save(Game.Player.Character.Model);
        }

        private void Companion_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.I:
                    menu.Visible = !menu.Visible;
                    break;
            }
        }

        private void Inventories_ItemAdded(object sender, ItemChangedEventArgs e)
        {
            menu.Add(new InventoryItem(e.Item));
        }

        private void Inventories_ItemRemoved(object sender, ItemChangedEventArgs e)
        {
            menu.Remove(item => item is InventoryItem ii && ii.Item == e.Item);
        }

        #endregion
    }
}
