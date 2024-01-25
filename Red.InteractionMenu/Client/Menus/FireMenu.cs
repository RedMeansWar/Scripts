using System.Collections.Generic;
using MenuAPI;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.InteractionMenu.Client.Menus
{
    internal class FireMenu
    {
        public static Menu GetMenu()
        {
            Menu menu = new("Red Menu", "~b~Fire Menu");

            

            return menu;
        }
    }
}
