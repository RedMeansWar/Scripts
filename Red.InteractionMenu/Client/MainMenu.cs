using MenuAPI;
using CitizenFX.Core;
using Red.InteractionMenu.Client.Menus;

namespace Red.InteractionMenu.Client
{
    public class MainMenu : BaseScript
    {
        protected readonly Menu menu = new("Red Menu", "~b~Main Menu");

        public MainMenu()
        {
            MenuController.MenuToggleKey = Control.InteractionMenu;
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Left; 
            MenuController.AddMenu(menu);

            MenuItem policeButton = new("Police Toolbox");
            menu.AddMenuItem(policeButton);
            MenuController.BindMenuItem(menu, LEToolbox.GetMenu(), policeButton);

            MenuItem fireButton = new("Fire Toolbox");
            menu.AddMenuItem(fireButton);

            MenuItem civButton = new("Civilian Toolbox");
            menu.AddMenuItem(civButton);

            MenuItem vehButton = new("Vehicle Menu");
            menu.AddMenuItem(vehButton);
            MenuController.BindMenuItem(menu, VehicleMenu.GetMenu(), vehButton);

            MenuItem setButton = new("Settings Menu");
            menu.AddMenuItem(setButton);
            MenuController.BindMenuItem(menu, SettingsMenu.GetMenu(), setButton);

            menu.AddMenuItem(new("~r~Close", "Close all menus"));

            menu.OnItemSelect += Menu_OnItemSelect;
        }

        private void Menu_OnItemSelect(Menu menu, MenuItem menuItem, int itemIndex)
        {
            string item = menuItem.Text;

            if (item == "~r~Close")
            {
                MenuController.CloseAllMenus();
            }
        }
    }
}