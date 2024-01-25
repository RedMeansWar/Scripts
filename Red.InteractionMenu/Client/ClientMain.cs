using MenuAPI;
using CitizenFX.Core;
using Red.InteractionMenu.Client.Menus;

namespace Red.InteractionMenu.Client
{
    public class ClientMain : BaseScript
    {
        protected Menu menu = new("Red Menu", "~b~Main Menu");

        public ClientMain()
        {
            MenuController.MenuToggleKey = Control.InteractionMenu;
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Left;

            MenuItem policeBtn = new("Police Menu", "Go to the police menu") { Label = Constants.forwardArrow };
            MenuController.BindMenuItem(menu, PoliceMenu.GetMenu(), policeBtn);
            menu.AddMenuItem(policeBtn);

            MenuItem fireBtn = new("Fire Menu", "Go to the fire menu") { Label = Constants.forwardArrow };
            MenuController.BindMenuItem(menu, FireMenu.GetMenu(), fireBtn);
            menu.AddMenuItem(policeBtn);

            MenuItem civBtn = new("Civilian Menu", "Go to the police menu") { Label = Constants.forwardArrow };
            MenuController.BindMenuItem(menu, CivilianMenu.GetMenu(), civBtn);
            menu.AddMenuItem(policeBtn);

            MenuItem vehBtn = new("Vehicle Menu", "Go to the fire menu") { Label = Constants.forwardArrow };
            MenuController.BindMenuItem(menu, VehicleMenu.GetMenu(), vehBtn);
            menu.AddMenuItem(policeBtn);

            MenuItem setBtn = new("Settings Menu", "Go to the fire menu") { Label = Constants.forwardArrow };
            MenuController.BindMenuItem(menu, SettingsMenu.GetMenu(), setBtn);
            menu.AddMenuItem(policeBtn);

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