using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuAPI;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.InteractionMenu.Client.MenuHelper;


namespace Red.InteractionMenu.Client.Menus
{
    public class FireMenu
    {
        public static Menu GetMenu()
        {
            Menu menu = new(menuName, subMenuFireName);

            return menu;
        }
    }
}
