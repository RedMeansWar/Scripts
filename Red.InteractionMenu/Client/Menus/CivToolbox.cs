using System.Collections.Generic;
using CitizenFX.Core;
using MenuAPI;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;
using static Red.Common.Client.Hud.HUD;

namespace Red.InteractionMenu.Client.Menus
{
    internal class CivToolbox : BaseScript
    {
        public static Menu GetMenu()
        {
            Menu menu = new("Red Menu", "~b~Civilian Toolbox");

            MenuItem propButton = new("Prop Spawner");
            menu.AddMenuItem(propButton);
            MenuController.BindMenuItem(menu, PropMenu.GetMenu(), propButton);
            
            menu.AddMenuItem(new MenuListItem("Restrainment", new List<string> { "Rear Ziptie", "Front Ziptie" }, 0));
            menu.AddMenuItem(new("Drop Weapon"));

            menu.AddMenuItem(new("Hands Up"));
            menu.AddMenuItem(new("Hands Up & Knees"));
            menu.AddMenuItem(new("Hands On Head"));

            menu.AddMenuItem(new("~o~Back"));
            menu.AddMenuItem(new("~r~Close"));

            menu.OnItemSelect += Menu_OnItemSelect;
            menu.OnListItemSelect += Menu_OnListItemSelect;

            return menu;
        }

        private static void Menu_OnItemSelect(Menu menu, MenuItem menuItem, int itemIndex)
        {
            string item = menuItem.Text;

            if (item == "Drop Weapon")
            {
                if (PlayerPed.Weapons.Current != WeaponHash.Unarmed)
                {
                    SuccessNotification($"You've dropped your {PlayerPed.Weapons.Current.LocalizedName}");
                    PlayerPed.Weapons.Drop();
                }
                else
                {
                    ErrorNotification("You must equip the weapon you wish to drop.");
                }
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

            if (item == "Restrainment")
            {
                switch (selectedIndex)
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
}
