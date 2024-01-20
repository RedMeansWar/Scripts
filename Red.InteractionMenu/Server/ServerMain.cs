using System;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace Red.InteractionMenu.Server
{
    public class ServerMain : BaseScript
    {
        public static Menu GetMenu()
        {
            Menu menu = new($"{Constants.communityName} Menu", "~b~Civilian Menu");



            return menu;
        }
    }
}