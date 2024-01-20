using MenuAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Red.InteractionMenu.Client.Menus
{
    internal class PoliceMenu
    {
        public static Menu GetMenu()
        {
            Menu menu = new($"{Constants.communityName} Menu", "~b~Civilian Menu");



            return menu;
        }
    }
}
