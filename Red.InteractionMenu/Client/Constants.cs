using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.InteractionMenu.Client
{
    internal class Constants
    {
        public static string forwardArrow = "→→→";
        public static string backwardArrow = "←←←";

        public static bool RightAlignMenu
        {
            get => GetSettings("redMenuAlignRight");
            set => SetSavedSettings("redMenuAlignRight", value);
        }

        private static bool GetSettings(string kvpString)
        {
            string savedVal = GetResourceKvpString($"red_menu_{kvpString}");
            bool savedValExists = !string.IsNullOrEmpty(savedVal);

            if (!savedValExists)
            {
                SetSavedSettings(kvpString, false);
                return false;
            }
            else
            {
                return GetResourceKvpString($"red_menu_{kvpString}").ToLower() == "true";
            }
        }

        private static void SetSavedSettings(string kvpString, bool newSavedValue) => SetResourceKvp("red_menu_" + kvpString, newSavedValue.ToString());
    }
}
