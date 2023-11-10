using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuAPI;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.InteractionMenu.Client.MenuHelper;

namespace Red.InteractionMenu.Client.Menus
{
    public class SettingsMenu
    {
        public static Menu GetMenu()
        {
            ReadConfigFile();
            Menu menu = new("Interaction Menu", "Prop Menu");

            menu.AddMenuItem(new MenuCheckboxItem("Right-Align Menu"));
            menu.AddMenuItem(new MenuCheckboxItem("Lock Minimap Rotation"));
            menu.AddMenuItem(new("~o~Back") { Label = BackArrow, Description = "Goes back one menu." });
            menu.AddMenuItem(new("~r~Close") { Description = $"Closes all {menuName} menus." });

            menu.OnItemSelect += Menu_OnItemSelect;
            menu.OnCheckboxChange += Menu_OnCheckboxChange;

            return menu;
        }

        private static void Menu_OnItemSelect(Menu menu, MenuItem menuItem, int itemIndex)
        {
            string item = menuItem.Text;

            if (item == "~o~Back")
            {
                menu.GoBack();
            }
            else if (item == "~r~Close")
            {
                MenuController.CloseAllMenus();
            }
        }

        private static void Menu_OnCheckboxChange(Menu menu, MenuCheckboxItem menuItem, int itemIndex, bool newCheckedState)
        {
            string item = menuItem.Text;
            bool state = newCheckedState;

            if (state is true && item == "Right-Align Menu")
            {
                MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;
            }
            else if (state is false && item == "Right-Align Menu")
            {
                MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Left;
            }
            else if (state is true && item == "Lock Minimap Rotation")
            {
                // lock minimap logic.
            }
            else if (state is false && item == "Lock Minimap Rotation")
            {
                // unlock minimap logic.
            }
        }
    }
}
