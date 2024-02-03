using MenuAPI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;

namespace Red.InteractionMenu.Client.Menus
{
    public class SceneManagement : BaseScript
    {
        #region Variables
        protected static int zoneRadius = 50;
        protected static float zoneSpeed = 30;
        protected static Prop visualizedProp;
        protected static SceneProp visualizedSceneProp;
        protected readonly List<Speedzone> speedzones = new();

        protected readonly static IReadOnlyList<SceneProp> sceneProps = new List<SceneProp>()
        {
            new SceneProp { Name = "Police Barrier", Model = "prop_barrier_work05" },
            new SceneProp { Name = "Roadwork Ahead Barrier", Model = "prop_mp_barrier_02" },
            new SceneProp { Name = "Type III Barrier", Model = "prop_mp_barrier_02b" },
            new SceneProp { Name = "Small Cone", Model = "prop_roadcone02b" },
            new SceneProp { Name = "Large Cone", Model = "prop_roadcone01a" },
            new SceneProp { Name = "Drum Cone", Model = "prop_barrier_wat_03b" },
            new SceneProp { Name = "Tent", Model = "prop_gazebo_02" },
            new SceneProp { Name = "Scene Lights", Model = "prop_worklight_03b", Heading = 180f }
        };

        protected readonly static List<int> speedzoneRadiuses = new()
        {
            25, 50, 75, 100
        };

        protected readonly static List<int> speedzoneSpeeds = new()
        {
            0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60
        };
        #endregion

        #region Constructor
        public SceneManagement() => TriggerServerEvent("Menu:Server:getSpeedzones");
        #endregion

        #region Methods

        public static Menu GetMenu()
        {
            Menu menu = new("Red Menu", "~b~Scene Management");

            List<string> propNames = sceneProps.Select((prop, index) => $"{prop.Name} ({index + 1}/{sceneProps.Count})").ToList();
            menu.AddMenuItem(new MenuListItem("Spawn Prop", propNames, 0));

            menu.AddMenuItem(new MenuListItem("Speedzone Radius", speedzoneRadiuses.ConvertAll(x => $"{x}m"), 1));
            menu.AddMenuItem(new MenuListItem("Speedzone Speed", speedzoneSpeeds.ConvertAll(x => $"{x}mph"), 6));

            menu.AddMenuItem(new("Create Speedzone", "Create a speed zone with the radius and speed selected above."));
            menu.AddMenuItem(new("Delete Closest Speedzone", "Delete the closest speed zone."));

            menu.AddMenuItem(new("Delete Closest Prop"));

            menu.AddMenuItem(new("~o~Back"));
            menu.AddMenuItem(new("~r~Close"));

            menu.OnItemSelect += SceneManagement_OnItemSelect;
            menu.OnMenuOpen += SceneManagement_OnMenuOpen;
            menu.OnListItemSelect += SceneManagement_OnListItemSelect;
            menu.OnIndexChange += SceneManagement_OnIndexChange;
            menu.OnListIndexChange += SceneManagement_OnListIndexChange;
            menu.OnMenuClose += SceneManagement_OnMenuClose;

            return menu;
        }

        private static void SceneManagement_OnListIndexChange(Menu menu, MenuListItem listItem, int oldSelectionIndex, int newSelectionIndex, int itemIndex)
        {
            string item = listItem.Text;

            if (item == "Spawn Prop")
            {
                visualizedSceneProp = sceneProps[newSelectionIndex];
            }
            else if (item == "Speedzone Radius")
            {
                zoneRadius = speedzoneRadiuses[newSelectionIndex];
            }
            else if (item == "Speedzone Speed")
            {
                zoneSpeed = speedzoneSpeeds[newSelectionIndex] / 2.236936f;
            }
        }

        private static void SceneManagement_OnIndexChange(Menu menu, MenuItem oldItem, dynamic newItem, int oldIndex, int newIndex)
        {
            if (newIndex == 0)
            {
                int listIndex = ((MenuListItem)newItem).ListIndex;
                visualizedSceneProp = sceneProps[listIndex];
            }
            else
            {
                visualizedSceneProp = null;
            }
        }

        private static void SceneManagement_OnMenuClose(Menu menu) => visualizedSceneProp = null;

        private static void SceneManagement_OnMenuOpen(Menu menu)
        {
            if (menu.CurrentIndex == 0)
            {
                dynamic scenePropItem = menu.GetCurrentMenuItem();
                visualizedSceneProp = sceneProps[scenePropItem.ListIndex];
            }
        }

        private static async void SceneManagement_OnListItemSelect(Menu menu, MenuListItem listItem, int selectedIndex, int itemIndex)
        {
            if (listItem.Text == "Spawn Prop")
            {
                bool spawned = await SpawnProp(selectedIndex);

                Screen.ShowNotification(spawned ? $"~g~~h~Success~h~~s~: Spawned {sceneProps[selectedIndex].Name}." : "~r~~h~Error~h~~s~: Couldn't spawn prop, try again.", true);
            }
        }

        private static void SceneManagement_OnItemSelect(Menu menu, MenuItem menuItem, int itemIndex)
        {
            string item = menuItem.Text;

            if (item == "Create Speedzone")
            {
                TriggerServerEvent("Menu:Server:createSpeedzone", PlayerPed.Position, zoneRadius, zoneSpeed);
            }
            else if (item == "Delete Closest Speedzone")
            {
                TriggerServerEvent("Menu:Server:deleteSpeedzone", PlayerPed.Position);
            }
            else if (item == "Delete Closest Prop")
            {
                bool deleted = DeleteClosestSceneProp();

                Screen.ShowNotification(deleted ? $"~g~~h~Success~h~~s~: Deleted prop." : "~r~~h~Error~h~~s~: Couldn't delete prop, try again.", true);
            }
            else if (item == "~o~Back")
            {
                menu.GoBack();
            }
            else if (item == "~r~Close")
            {
                MenuController.CloseAllMenus();
            }
        }

        private static bool DeleteClosestSceneProp()
        {
            Vector3 plyPos = PlayerPed.Position;

            foreach (SceneProp sceneProp in sceneProps)
            {
                uint modelHash = Game.GenerateHashASCII(sceneProp.Model);

                Prop prop = World.GetAllProps()
                    .Where(p => p.Model == modelHash)
                    .OrderBy(p => Vector3.DistanceSquared(plyPos, p.Position))
                    .FirstOrDefault();

                if (prop is not null && Vector3.DistanceSquared(plyPos, prop.Position) < 3f)
                {
                    if (NetworkGetEntityOwner(prop.Handle) == Game.Player.Handle)
                    {
                        prop?.Delete();
                    }
                    else
                    {
                        TriggerServerEvent("Menu:Server:deleteProp", prop?.NetworkId);
                    }

                    return true;
                }
            }

            return false;
        }

        private static async Task<bool> SpawnProp(int propIndex)
        {
            if (PlayerPed.IsInVehicle())
            {
                return false;
            }

            SceneProp sceneProp = sceneProps[propIndex];

            Prop spawnedProp = await World.CreateProp(new(sceneProp.Model), PlayerPed.Position, false, true);
            spawnedProp.Heading = RotateProp(PlayerPed.Heading, sceneProp.Heading);
            spawnedProp.IsPersistent = true;
            spawnedProp.IsPositionFrozen = true;

            return true;
        }

        private static float RotateProp(float rawHeading, float offsetDegrees)
        {
            float orientation = (rawHeading + offsetDegrees) % 360;

            if (orientation < 0)
            {
                orientation += 360;
            }

            return orientation;
        }

        #endregion

        #region Ticks

        [Tick]
        private async Task VisualizePropTick()
        {
            if (visualizedProp is not null && visualizedProp.Exists())
            {
                visualizedProp.Delete();
                visualizedProp = null;
            }

            if (visualizedSceneProp is null || PlayerPed.IsInVehicle())
            {
                visualizedProp = null;
                await Delay(1000);
                return;
            }

            visualizedProp = await World.CreateProp(new(visualizedSceneProp.Model), PlayerPed.GetOffsetPosition(new(0, 1f, 0f)), false, true);
            visualizedProp.Heading = RotateProp(PlayerPed.Heading, visualizedSceneProp.Heading);
            visualizedProp.IsCollisionEnabled = false;
            SetEntityAlpha(visualizedProp.Handle, 100, 0);
        }

        #endregion

        #region Event Handlers
        [EventHandler("Menu:Client:updateSpeedzones")]
        internal void OnSpeedZonesUpdated(string json)
        {
            List<Speedzone> updatedZones = Json.Parse<List<Speedzone>>(json);

            foreach (Speedzone speedzone in speedzones)
            {
                if (!updatedZones.Any(uz => uz == speedzone))
                {
                    if (speedzone.Blip > 0 && DoesBlipExist(speedzone.Blip))
                    {
                        int blip = speedzone.Blip;
                        RemoveBlip(ref blip);
                    }

                    if (speedzone.Zone > 0)
                    {
                        RemoveSpeedZone(speedzone.Zone);
                    }
                }
            }

            foreach (Speedzone zone in updatedZones)
            {
                Debug.WriteLine($"Blip was found at Vector: ({new Vector3(zone.Position.X, zone.Position.Y, zone.Position.Z)}) with a radius of {zone.Radius}.");

                zone.Blip = AddBlipForRadius(zone.Position.X, zone.Position.Y, zone.Position.Z, zone.Radius);
                SetBlipSprite(zone.Blip, 9);
                SetBlipColour(zone.Blip, 3);
                SetBlipAlpha(zone.Blip, 80);

                zone.Zone = AddSpeedZoneForCoord(zone.Position.X, zone.Position.Y, zone.Position.Z, zone.Radius, zone.Speed, false);

                speedzones.Add(zone);
            }
        }

        [EventHandler("Menu:Client:showClientNotification")]
        private void OnShowClientNotification(string message) => Screen.ShowNotification(message, true);
        #endregion

        #region Classes
        public class SceneProp
        {
            public string Name { get; set; }
            public string Model { get; set; }
            public float Heading { get; set; }
        }
        #endregion
    }
}