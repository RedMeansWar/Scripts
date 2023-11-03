using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuAPI;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.InteractionMenu.Client.Variables;

namespace Red.InteractionMenu.Client.Menus
{
    public class VehicleMenu
    {
        public static Menu GetMenu()
        {
            ReadConfigFile();
            Menu menu = new(menuName, subMenuVehicleName);

            return menu;
        }
    }
}
