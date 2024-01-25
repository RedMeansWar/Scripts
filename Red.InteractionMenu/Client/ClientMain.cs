using SharpConfig;
using static CitizenFX.Core.Native.API;

namespace Red.InteractionMenu.Client
{
    internal class ClientMain
    {
        public static string communityName = "Red";

        public ClientMain()
        {
            ReadConfigFile();
            AddTextEntry("WEAPON_BEANBAGSHOTGUN", "BEANBAGSHOTGUN");
        }

        private void ReadConfigFile()
        {
            var data = LoadResourceFile(GetCurrentResourceName(), "config.ini");

            if (Configuration.LoadFromString(data).Contains("Menu", "CommunityName") == true)
            {
                Configuration loaded = Configuration.LoadFromString(data);
                communityName = loaded["Menu"]["CommunityName"].StringValue;
            }
            else
            {
                // do nothing
            }
        }
    }
}
