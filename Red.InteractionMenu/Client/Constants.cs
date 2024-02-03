using System.Collections.Generic;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;

namespace Red.InteractionMenu.Client
{
    internal class Constants
    {
        #region Menu
        public static string forwardArrow = "→→→";

        public static List<int> zoneSpeeds = new()
        {
            0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60
        };

        public static List<SceneProp> propList = new()
        {
            new() { Name = "Police Barrier", Model = "prop_barrier_work05" },
            new() { Name = "Roadwork Ahead Barrier", Model = "prop_mp_barrier_02" },
            new() { Name = "Type III Barrier", Model = "prop_mp_barrier_02b" },
            new() { Name = "Small Cone", Model = "prop_roadcone02b" },
            new() { Name = "Big Cone", Model = "prop_roadcone01a" },
            new() { Name = "Drum Cone", Model = "prop_Barrier_wat_03b" },
            new() { Name = "Tent", Model = "prop_gazebo_02" },
            new() { Name = "Scene Lights", Model = "prop_worklight_03b", Heading = 180f },
        };
        #endregion

        #region Methods
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
        #endregion
    }
}
