using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuAPI;
using static CitizenFX.Core.Native.API;

namespace Red.InteractionMenu.Client.Menus.SubMenus
{
    internal class SceneManagement
    {
        public static Menu GetMenu()
        {
            Menu menu = new( $"{ClientMain.communityName}", "Scene Menu");

            return menu;
        }

    }
}
