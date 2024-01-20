using CitizenFX.Core;
using MenuAPI;

namespace Red.InteractionMenu.Client.Menus
{
    internal class CivilianMenu : BaseScript
    {
        public static Menu GetMenu()
        {
            Menu menu = new($"{Constants.communityName} Menu", "~b~Civilian Menu");



            return menu;
        }
    }
}
