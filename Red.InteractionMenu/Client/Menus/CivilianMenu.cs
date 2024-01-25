using System.Collections.Generic;
using MenuAPI;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.InteractionMenu.Client.Menus
{
    internal class CivilianMenu : BaseScript
    {
        public static Menu GetMenu()
        {
            Menu menu = new($"{Constants.communityName} Menu", "~b~Civilian Menu");

            menu.AddMenuItem(new MenuListItem("Restrainment", new List<string> { "Rear Ziptie", "Front Ziptie" }, 0));
            
            menu.AddMenuItem(new("Drop Weapon"));
            menu.AddMenuItem(new("Hands Up"));
            menu.AddMenuItem(new("Hands Up & Knees"));
            
            menu.AddMenuItem(new("Set BAC"));
            menu.AddMenuItem(new(""));

            menu.AddMenuItem(new("~o~Back", "Go back one menu."));
            menu.AddMenuItem(new("~r~Close", "Closes all menus."));

            return menu;
        }
    }
}
