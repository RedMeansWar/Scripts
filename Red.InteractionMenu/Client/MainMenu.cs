using System;
using System.Threading.Tasks;
using MenuAPI;
using Red.InteractionMenu.Client.Menus;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.InteractionMenu.Client.Variables;

namespace Red.InteractionMenu.Client
{
    public class MainMenu : BaseScript
    {
        public MainMenu()
        {
            ReadConfigFile();
            Menu menu = new(menuName, "Main Menu");

            MenuController.EnableMenuToggleKeyOnController = false;
            MenuController.MenuToggleKey = (Control)openKey;
            MenuController.AddMenu(menu);

            MenuItem policeBindButton = new(subMenuPoliceName) { Label = ForwardArrow, Description = $"Opens the {subMenuPoliceName}." };
            MenuItem fireBindButton = new(subMenuFireName) { Label = ForwardArrow, Description = $"Opens the {subMenuFireName}." };
            MenuItem civBindButton = new(subMenuCivilianName) { Label = ForwardArrow, Description = $"Opens the {subMenuCivilianName}." };
            MenuItem vehBindButton = new(subMenuVehicleName) { Label = ForwardArrow, Description = $"Opens the {subMenuVehicleName}." };
            MenuItem settingsBindButton = new("Settings") { Label = ForwardArrow, Description = $"Opens the {menuName}'s Settings menu." };

            menu.AddMenuItem(policeBindButton);
            menu.AddMenuItem(fireBindButton);
            menu.AddMenuItem(civBindButton);
            menu.AddMenuItem(vehBindButton);
            menu.AddMenuItem(settingsBindButton);
            menu.AddMenuItem(new("~r~Close Menu") { Description = $"Closes all {menuName} menus." });

            MenuController.BindMenuItem(menu, PoliceMenu.GetMenu(), policeBindButton);
            MenuController.BindMenuItem(menu, FireMenu.GetMenu(), fireBindButton);
            MenuController.BindMenuItem(menu, CivilianMenu.GetMenu(), civBindButton);
            MenuController.BindMenuItem(menu, VehicleMenu.GetMenu(), vehBindButton);
            MenuController.BindMenuItem(menu, SettingsMenu.GetMenu(), settingsBindButton);

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