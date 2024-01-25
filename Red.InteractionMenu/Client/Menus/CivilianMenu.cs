using System.Collections.Generic;
using MenuAPI;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using Red.InteractionMenu.Client.Menus.SubMenus;

namespace Red.InteractionMenu.Client.Menus
{
    internal class CivilianMenu : BaseScript
    {
        public static Menu GetMenu()
        {
            Menu menu = new("Red Menu", "~b~Civilian Menu");

            MenuItem sceneManage = new("Scene Management") { Label = Constants.forwardArrow };
            MenuController.BindMenuItem(GetMenu(), SceneManagement.GetMenu(), sceneManage);
            menu.AddMenuItem(sceneManage);

            menu.AddMenuItem(new MenuListItem("Restrainment", new List<string> { "Rear Ziptie", "Front Ziptie" }, 0));
            menu.AddMenuItem(new MenuListItem("Step Out Of Vehicle", new List<string> { "Driver", "Passanger", "Rear Driver", "Rear Passanger" }, 0));
            
            menu.AddMenuItem(new("Drop Weapon"));
            menu.AddMenuItem(new("Hands Up"));
            menu.AddMenuItem(new("Hands Up & Knees"));

            menu.AddMenuItem(new("Hands On Head"));
            menu.AddMenuItem(new("Set BAC"));

            menu.AddMenuItem(new("~o~Back", "Go back one menu."));
            menu.AddMenuItem(new("~r~Close", "Closes all menus."));

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
                ExecuteCommand("hu");
            }
            else if (item == "Hands Up & Knees")
            {
                ExecuteCommand("huk");
            }
            else if (item == "Hands On Head")
            {
                ExecuteCommand("hoh");
            }
            else if (item == "Set BAC")
            {
                ExecuteCommand("setbac");
            }
        }

        private static void Menu_OnListItemSelect(Menu menu, MenuListItem listItem, int selectedIndex, int itemIndex)
        {
            string item = listItem.Text;

            if (item == "Restrainment")
            {
                switch (selectedIndex)
                {
                    case 0:
                        ExecuteCommand("ziptie");
                        break;

                    case 1:
                        ExecuteCommand("frontziptie");
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
