using SharpConfig;
using static CitizenFX.Core.Native.API;

namespace Red.InteractionMenu.Client
{
    internal class Constants
    {
        public static string communityName = "Red";
        public static string forwardArrow = "→→→";
        public static string backwardArrow = "←←←";

        public Constants() => ReadConfigFile();

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
