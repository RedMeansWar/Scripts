using System;
using MenuAPI;

namespace Red.InteractionMenu.Client.Menus
{
    internal class SettingsMenu
    {
        public static Menu GetMenu()
        {
            Menu menu = new("Red Menu", "~b~Settings Menu");
            
            menu.AddMenuItem(new MenuCheckboxItem("Right-Align Menu"));
            menu.AddMenuItem(new("~o~Back", "Go back one menu."));
            menu.AddMenuItem(new("~r~Close", "Close all menus."));

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
            if (newCheckedState == true)
            {
                MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;
            }
        }
    }
}
