﻿using System;
using System.Collections.Generic;
using MenuAPI;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.InteractionMenu.Client.Constants;

namespace Red.InteractionMenu.Client.Menus
{
    internal class VehicleMenu
    {
        public static Menu GetMenu()
        {
            Menu menu = new("Red Menu", "~b~Vehicle Menu");
            Menu confirmDeleteMenu = new("Confirm Action", "Delete Vehicle, are you sure?");
             

            menu.AddMenuItem(new MenuListItem("Roll Up / Down Windows", new List<string> { "Roll Up Windows", "Roll Down Windows" }, 0));
            menu.AddMenuItem(new("Toggle Engine", "Toggle the current vehicles engines"));
            menu.AddMenuItem(new("Toggle Trunk", "Opens and closes the trunk of the current vehicle."));

            menu.AddMenuItem(new("Toggle Hood", "Opens and closes the hood of the current vehicle."));
            menu.AddMenuItem(new MenuListItem("Open / Close Door", new List<string> { "Driver", "Passanger", "Rear Driver", "Rear Passanger" }, 0));
            menu.AddMenuItem(new("Shuffle Seats", "Shuffle to a different seat."));

            menu.AddMenuItem(new("Flip Vehicle", "Flips a vehicle that is upside down back upright"));
            menu.AddMenuItem(new MenuListItem("Door Locks", new List<string> { "Unlock Doors", "Lock Doors" }, 0));
            menu.AddMenuItem(new("Toggle Seatbelt"));

            MenuItem deleteBtn = new("~r~Delete Vehicle") { LeftIcon = MenuItem.Icon.WARNING, Label = Constants.forwardArrow };
            menu.AddMenuItem(deleteBtn);

            MenuController.AddSubmenu(menu, confirmDeleteMenu);
            MenuItem deleteNoBtn = new("NO, Cancel", "NO, do NOT delete my vehicle and go back!");
            MenuItem deleteYesBtn = new("~r~YES, DELETE", "Yes, I'm sure. Delete my vehicle, please. I understand this cannot be undone.") { LeftIcon = MenuItem.Icon.WARNING };

            confirmDeleteMenu.AddMenuItem(deleteNoBtn);
            confirmDeleteMenu.AddMenuItem(deleteYesBtn);

            confirmDeleteMenu.OnItemSelect += (sender, item, select) =>
            {
                if (item != null)
                {
                    confirmDeleteMenu.GoBack();
                }
                else
                {
                    ExecuteCommand("dv");
                }

                confirmDeleteMenu.GoBack();
            };

            MenuController.BindMenuItem(menu, confirmDeleteMenu, deleteBtn);

            menu.AddMenuItem(new("~o~Back", "Go back one menu."));
            menu.AddMenuItem(new("~r~Close", "Close all menus."));

            menu.OnItemSelect += Menu_OnItemSelect;
            menu.OnListItemSelect += Menu_OnListItemSelect;

            return menu;
        }

        private static void Menu_OnItemSelect(Menu menu, MenuItem menuItem, int itemIndex)
        {
            string item = menuItem.Text;

            if (item == "Toggle Engine")
            {
                ExecuteCommand("eng");
            }
            else if (item == "Toggle Trunk")
            {
                ExecuteCommand("trunk");
            }
            else if (item == "Toggle Hood")
            {
                ExecuteCommand("hood");
            }
            else if (item == "Shuffle Seats")
            {
                ExecuteCommand("shuffle");
            }
            else if (item == "Flip Vehicle")
            {
                ExecuteCommand("flip");
            }
            else if (item == "Toggle Seatbelt")
            {
                if (!PlayerPed.IsInVehicle())
                {
                    ErrorNotification("You need to be in vehicle to use this!", true);
                }
                else
                {
                }
            }
            else if (item == "~o~Go Back")
            {
                menu.GoBack();
            }
            else if (item == "~r~Close")
            {
                MenuController.CloseAllMenus();
            }
        }

        private static void Menu_OnListItemSelect(Menu menu, MenuListItem listItem, int selectedIndex, int itemIndex)
        {
            string item = listItem.Text;
            Vehicle vehicle = Game.PlayerPed.CurrentVehicle;

            if (item == "Roll Up / Down Windows")
            {
                switch (selectedIndex)
                {
                    case 0:
                        vehicle.Windows.RollUpAllWindows();
                        break;
                    case 1:
                        vehicle.Windows.RollDownAllWindows();
                        break;
                    default:
                        break;
                }
            }
            else if (item == "Open / Close Door")
            {
                switch (selectedIndex)
                {
                    case 0:
                        ExecuteCommand("door 1");
                        break;

                    case 1:
                        ExecuteCommand("door 2");
                        break;

                    case 2:
                        ExecuteCommand("door 3");
                        break;

                    case 3:
                        ExecuteCommand("door 4");
                        break;

                    default:
                        break;
                }
            }
            else if (item == "Vehicle Locks")
            {
                switch (selectedIndex)
                {
                    case 0:
                        break;

                    case 1:
                        break;

                    default:
                        break;
                }
            }
        }
        
    }
}