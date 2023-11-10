using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuAPI;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.InteractionMenu.Client.MenuHelper;

namespace Red.InteractionMenu.Client.Submenus
{
    public class SceneManagementMenu : BaseScript
    {
        public static Menu GetMenu()
        {
            ReadConfigFile();

            Menu menu = new(menuName, "Scene Management");

            return menu;
        }
    }
}
