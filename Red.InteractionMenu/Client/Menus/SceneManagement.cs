using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuAPI;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;
using static Red.Common.Client.Hud.HUD;

namespace Red.InteractionMenu.Client.Menus
{
    internal class SceneManagement
    {
        public static Menu GetMenu()
        {
            Menu menu = new("Red Menu", "~b~Law Enforcement Toolbox");

            menu.AddMenuItem(new MenuListItem("Prop Spawner", new List<string> { "Grab", "Seat", "Unseat" }, 0));
            menu.AddMenuItem(new MenuListItem("Zone Radius", new List<string> { "", "" }, 0));
            menu.AddMenuItem(new MenuListItem("Zone Speed", new List<string> { "", "" }, 0));

            menu.AddMenuItem(new("Create Speed Zone"));
            menu.AddMenuItem(new("Delete Closest Zone"));

            menu.AddMenuItem(new("~o~Back"));
            menu.AddMenuItem(new("~r~Close"));

            menu.OnItemSelect += Menu_OnItemSelect;
            menu.OnListItemSelect += Menu_OnListItemSelect;

            return menu;
        }

        private static void Menu_OnItemSelect(Menu menu, MenuItem menuItem, int itemIndex)
        {

        }

        private static void Menu_OnListItemSelect(Menu menu, MenuListItem listItem, int selectedIndex, int itemIndex)
        {
        }
    }
}
