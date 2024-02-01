using MenuAPI;

namespace Red.InteractionMenu.Client.Menus
{
    internal class SettingsMenu
    {
        public static Menu GetMenu()
        {
            Menu menu = new("Red Menu", "~b~Menu Settings");

            menu.AddMenuItem(new MenuCheckboxItem("Right-Align Menu", Constants.RightAlignMenu));
            menu.AddMenuItem(new("~o~Back"));
            menu.AddMenuItem(new("~r~Close"));

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

            if (item == "Right-Align Menu")
            {
                Constants.RightAlignMenu = newCheckedState;
                MenuController.MenuAlignment = newCheckedState ? MenuController.MenuAlignmentOption.Right : MenuController.MenuAlignmentOption.Left;
            }
        }
    }
}