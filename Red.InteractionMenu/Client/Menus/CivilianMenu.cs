using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuAPI;
using Red.InteractionMenu.Client.Submenus;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.InteractionMenu.Client.Variables;

namespace Red.InteractionMenu.Client.Menus
{
    public class CivilianMenu : BaseScript
    {
        protected static List<string> advertisementList = new()
        {
            "Ammu-Nation", "Cluckin 'Bell", "FlyUS"
        };

        public static Menu GetMenu()
        {
            ReadConfigFile();
            Menu menu = new(menuName, subMenuCivilianName);

            MenuItem button = new("Prop Menu") { Label = ForwardArrow };

            menu.AddMenuItem(button);
            menu.AddMenuItem(new MenuListItem("Ziptie", new List<string> { "Ziptie", "Front Ziptie" }, 0));
            menu.AddMenuItem(new("Drop Weapon"));
            menu.AddMenuItem(new("Hands Up"));
            menu.AddMenuItem(new("Hands On Head"));
            menu.AddMenuItem(new("Hands Up Knees"));
            menu.AddMenuItem(new MenuListItem("Conduct Advertisement", advertisementList, 0));
            menu.AddMenuItem(new("~o~Back") { Label = BackArrow, Description = "Goes back one menu." });
            menu.AddMenuItem(new("~r~Close") { Description = $"Closes all {menuName} menus." });

            MenuController.BindMenuItem(menu, PropSpawnerMenu.GetMenu(), button);
            MenuController.AddSubmenu(menu, PropSpawnerMenu.GetMenu());

            menu.OnItemSelect += Menu_OnItemSelect;
            menu.OnListItemSelect += Menu_OnListItemSelect;

            return menu;
        }

        private static void Menu_OnItemSelect(Menu menu, MenuItem menuItem, int itemIndex)
        {
            string item = menuItem.Text;

            if (item == "Drop Weapon")
            {
                ExecuteCommand("dropweapon");
            }
            else if (item == "Hands Up")
            {
                ExecuteCommand("handsup");
            }
            else if (item == "Hands On Head")
            {
                ExecuteCommand("handsonhead");
            }
            else if (item == "Heads Up Knees")
            {
                ExecuteCommand("handsupknees");
            }
            else if (item == "~o~Back")
            {
                menu.GoBack();
            }
            else if (item == "~r~Close")
            {
                MenuController.CloseAllMenus();
            }
        }

        private static void Menu_OnListItemSelect(Menu menu, MenuListItem listItem, int selectedIndex, int itemIndex)
        {
            string item = listItem.Text;

            switch (itemIndex)
            {
                case 0:
                    TriggerEvent("Cuff:Client:cuffClosestPlayer", false, true);
                    break;

                case 1:
                    TriggerEvent("Cuff:Client:cuffClosestPlayer", true, true);
                    break;

                default:
                    break;
            }
        }
    }
}
