using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;

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

    internal class SceneProp
    {
        public int NetworkId { get; set; } = 1;
        public float[] Position { get; set; }
        public Status PropStatus { get; set; }

        public static async void CreateVirtualProp(string modelName, int networkId)
        {
            Prop createdProp = await World.CreateProp(modelName, PlayerPed.Position, true, true);

            SetEntityAlpha(createdProp.Handle, 127, 0);
        }
    }

    internal enum Status
    {
        NotPlaced,
        Placed
    }
}
