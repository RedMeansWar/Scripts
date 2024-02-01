using System.Collections.Generic;
using MenuAPI;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;
using static Red.Common.Client.Hud.HUD;

namespace Red.InteractionMenu.Client.Menus
{
    internal class VehicleMenu
    {
        protected static Menu DeleteConfirmMenu { get; private set; }

        public static Menu GetMenu()
        {
            Menu vehicleMenu = new("Red Menu", "~b~Vehicle Menu");

            DeleteConfirmMenu = new Menu("Confirm Action", "Delete vehicle, are you sure?");

            vehicleMenu.AddMenuItem(new("Toggle Engine"));
            vehicleMenu.AddMenuItem(new("Toggle Hood"));
            vehicleMenu.AddMenuItem(new("Toggle Trunk"));
            vehicleMenu.AddMenuItem(new("Shuffle Seats"));
            vehicleMenu.AddMenuItem(new MenuListItem("Vehicle Locks", new List<string> { "Unlock Doors", "Lock Doors" }, 0));

            MenuItem deleteBtn = new("~r~Delete Vehicle")
            {
                LeftIcon = MenuItem.Icon.WARNING,
                Label = "→→→"
            };
            vehicleMenu.AddMenuItem(deleteBtn);

            vehicleMenu.AddMenuItem(new("~o~Back"));
            vehicleMenu.AddMenuItem(new("~r~Close"));

            MenuController.AddSubmenu(vehicleMenu, DeleteConfirmMenu);
            MenuItem deleteNoBtn = new("NO, CANCEL", "NO, do NOT delete my vehicle and go back!");
            MenuItem deleteYesBtn = new("~r~YES, DELETE", "Yes, I'm sure. Delete my vehicle, please. I understand this cannot be undone.")
            {
                LeftIcon = MenuItem.Icon.WARNING
            };

            DeleteConfirmMenu.AddMenuItem(deleteNoBtn);
            DeleteConfirmMenu.AddMenuItem(deleteYesBtn);

            DeleteConfirmMenu.OnItemSelect += (sender, item, select) =>
            {
                if (item == deleteNoBtn)
                {
                    DeleteConfirmMenu.GoBack();
                }
                else
                {
                    ExecuteCommand("dv");
                }

                DeleteConfirmMenu.GoBack();
            };

            MenuController.BindMenuItem(vehicleMenu, DeleteConfirmMenu, deleteBtn);

            vehicleMenu.OnItemSelect += VehicleMenu_OnItemSelect;
            vehicleMenu.OnListItemSelect += VehicleMenu_OnListItemSelect;

            return vehicleMenu;
        }

        private static void VehicleMenu_OnItemSelect(Menu menu, MenuItem menuItem, int itemIndex)
        {
            string item = menuItem.Text;

            if (item == "Toggle Engine")
            {
                ExecuteCommand("eng");
            }
            else if (item == "Toggle Hood")
            {
                ExecuteCommand("hood");
            }
            else if (item == "Toggle Trunk")
            {
                ExecuteCommand("trunk");
            }
            else if (item == "Shuffle Seats")
            {
                ExecuteCommand("shuffle");
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

        private static void VehicleMenu_OnListItemSelect(Menu menu, MenuListItem listItem, int selectedIndex, int itemIndex)
        {
            string item = listItem.Text;

            if (item == "Vehicle Locks")
            {
                switch (selectedIndex)
                {
                    case 0:
                        VehicleLockHandler(true);
                        break;
                    case 1:
                        VehicleLockHandler();
                        break;
                    default:
                        break;
                }
            }
        }

        private static void VehicleLockHandler(bool unlock = false)
        {
            if (PlayerPed.CurrentVehicle is null)
            {
                ErrorNotification(" You must be in a vehicle.", true);
                return;
            }

            if (PlayerPed.CurrentVehicle.Driver != PlayerPed)
            {
                ErrorNotification("You must be the driver.", true);
                return;
            }

            PlayerPed.CurrentVehicle.LockStatus = unlock ? VehicleLockStatus.Unlocked : VehicleLockStatus.Locked;
            SuccessNotification($"{(unlock ? "Unlocked" : "Locked")}.", true);
        }
    }
}
