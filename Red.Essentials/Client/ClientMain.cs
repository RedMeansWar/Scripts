using System.Collections.Generic;
using System.Threading.Tasks;
using Red.Common.Client;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Hud.HUD;
using static Red.Common.Client.Client;

namespace Red.Essentials.Client
{ 
    internal class ClientMain : BaseScript
    {
        #region Variables
        protected float densityMultiplier = 1f;
        protected uint player = Game.GenerateHashASCII("PLAYER");
        protected bool handsUp, handsOnHead, handsUpKnees, usingCamera, selfieCamera;
        protected bool noShuffle = true;
        protected float steeringAngle;
        protected Ped PlayerPed = Game.PlayerPed;

        protected readonly IReadOnlyList<string> scenarioTypes = new List<string>
        {
            "WORLD_VEHICLE_MILITARY_PLANES_SMALL", "WORLD_VEHICLE_MILITARY_PLANES_BIG", "WORLD_VEHICLE_AMBULANCE", "WORLD_VEHICLE_POLICE_NEXT_TO_CAR", "WORLD_VEHICLE_POLICE_CAR", 
            "WORLD_VEHICLE_POLICE_BIKE", "WORLD_VEHICLE_DRIVE_PASSENGERS_LIMITED"
        };

        protected readonly IReadOnlyList<string> scenarioGroups = new List<string>
        {
            "MP_POLICE", "ARMY_HELI", "POLICE_POUND1", "POLICE_POUND2", "POLICE_POUND3", "POLICE_POUND4", "POLICE_POUND5", "SANDY_PLANES", "ALAMO_PLANES", 
            "GRAPESEED_PLANES", "LSA_PLANES", "NG_PLANES"
        };

        protected readonly IReadOnlyList<string> relationshipGroups = new List<string>
        {
            "AMBIENT_GANG_HILLBILLY", "AMBIENT_GANG_BALLAS", "AMBIENT_GANG_MEXICAN", "AMBIENT_GANG_FAMILY", "AMBIENT_GANG_MARABUNTE", "AMBIENT_GANG_SALVA", 
            "GANG_1", "GANG_2", "GANG_9", "GANG_10", "FIREMAN", "MEDIC", "COP"
        };

        protected readonly IReadOnlyList<string> suppressedModels = new List<string>
        {
            "police", "police2", "police3", "police4", "policeb", "policeold1", "policeold2", "policet", "polmav", "pranger", "sheriff", "sheriff2", "stockade3", "buffalo3", "fbi",
            "fbi2", "firetruk", "jester2", "lguard", "ambulance", "riot", "SHAMAL", "LUXOR", "LUXOR2", "JET", "LAZER", "TITAN", "BARRACKS", "BARRACKS2", "CRUSADER", "RHINO", "AIRTUG",
            "RIPLEY", "docktrailer", "trflat", "trailersmall", "boattrailer", "cargobob", "cargobob2", "cargobob3", "cargobob4", "volatus", "buzzard", "buzzard2", "besra",
        };

        protected readonly IReadOnlyList<Control> controlsToDisable = new List<Control>
        {
            Control.Aim, Control.Attack, Control.Attack2, Control.Cover, Control.Jump, Control.MeleeAttack1, Control.MeleeAttack2, Control.MeleeAttackAlternate, 
            Control.MeleeAttackHeavy, Control.MeleeAttackLight, Control.MeleeBlock, Control.Reload
        };
        #endregion

        #region Constructor
        public ClientMain()
        {
            SetWeaponsNoAutoreload(true);
            SetWeaponsNoAutoswap(true);
            NetworkSetFriendlyFireOption(true);
            SetPlayerHealthRechargeMultiplier(Game.Player.Handle, 0f);
            SetWeaponDamageModifier((uint)WeaponHash.Nightstick, 0.1f);
            SetWeaponDamageModifier((uint)WeaponHash.Unarmed, 0.1f);
            SetFlashLightKeepOnWhileMoving(true);

            SetRandomTrains(false);
            SetRandomBoats(false);

            SetAudioFlag("PoliceScannerDisabled", true);

            foreach (string scenarioType in scenarioTypes)
            {
                SetScenarioTypeEnabled(scenarioType, false);
            }

            foreach (string scenarioGroup in scenarioGroups)
            {
                SetScenarioGroupEnabled(scenarioGroup, false);
            }

            foreach (string relationshipGroup in relationshipGroups)
            {
                SetRelationshipBetweenGroups(1, Game.GenerateHashASCII(relationshipGroup), player);
            }

            for (int i = 0; i < 15; i++)
            {
                EnableDispatchService(i, false);
            }
        }
        #endregion

        #region Methods
        private async Task CameraControls()
        {
            if (Game.IsControlJustPressed(0, Control.PhoneCancel))
            {
                Screen.Hud.IsVisible = true;
                Screen.Hud.IsRadarVisible = true;
                DestroyMobilePhone();
                CellCamActivate(false, false);
                usingCamera = false;
                selfieCamera = false;
                Tick -= CameraControls;
            }

            if (Game.IsControlJustPressed(0, Control.NextCamera))
            {
                selfieCamera = !selfieCamera;
                Function.Call((Hash)2635073306796480568L, selfieCamera);
            }
        }

        private async Task<bool> DvVehicle(Vehicle vehicle)
        {
            vehicle.IsPersistent = true;
            vehicle.Delete();

            int tempTimer = Game.GameTime;

            while (vehicle.Exists())
            {
                if (Game.GameTime - tempTimer > 5000)
                {
                    return false;
                }

                await Delay(0);
            }

            return true;
        }

        private async Task DisableControls()
        {
            foreach (Control control in controlsToDisable)
            {
                Game.DisableControlThisFrame(0, control);
            }
        }
        #endregion

        #region Commands
        [Command("engine")]
        private void ToggleEngine()
        {
            Vehicle vehicle = PlayerPed.CurrentVehicle;

            SetVehicleEngineOn(vehicle.Handle, !vehicle.IsEngineRunning, false, true);
        }

        [Command("handsup")]
        private void HandsUp()
        {
            handsUp = !handsUp;

            if (handsUp || handsOnHead)
            {
                ClearAnimationTask("random@arrests@busted", "idle_c");
                ClearAnimationTask("random@mugging3", "handsup_standing_base");
                handsOnHead = false;
                handsUpKnees = false;
                Tick -= DisableControls;
            }

            if (handsUp)
            {
                PlayAnimation("random@mugging3", "handsup_standing_base", 2.5f, -1, AnimationFlags.StayInEndFrame | AnimationFlags.UpperBodyOnly | AnimationFlags.AllowRotation);
                Tick += DisableControls;
            }
            else
            {
                ClearAnimationTask("random@mugging3", "handsup_standing_base");
                Tick -= DisableControls;
            }
        }

        [Command("handsonhead")]
        private void HandsOnHead()
        {
            handsOnHead = !handsOnHead;

            if (handsUpKnees || handsUp)
            {
                ClearAnimationTask("random@mugging3", "handsup_standing_base");
                ClearAnimationTask("random@getawaydriver", "idle_a");
                handsUp = false;
                handsUpKnees = false;
                Tick -= DisableControls;
            }

            if (handsOnHead)
            {
                PlayAnimation("random@arrests@busted", "idle_c", 2.5f, -1, AnimationFlags.StayInEndFrame | AnimationFlags.UpperBodyOnly | AnimationFlags.AllowRotation);
                Tick += DisableControls;
            }
            else
            {
                ClearAnimationTask("random@arrests@busted", "idle_c");
                Tick -= DisableControls;
            }
        }

        [Command("handsupknees")]
        private void HandsUpKnees()
        {
            handsUpKnees = !handsUpKnees;

            if (handsUp || handsOnHead)
            {
                ClearAnimationTask("random@arrests@busted", "idle_c");
                ClearAnimationTask("random@mugging3", "handsup_standing_base");
                handsOnHead = false;
                handsUp = false;
                Tick -= DisableControls;
            }

            if (handsUpKnees && !PlayerPed.IsGettingIntoAVehicle && !PlayerPed.IsInVehicle())
            {
                PlayAnimation("random@mugging3", "handsup_standing_base", 2.5f, -1, AnimationFlags.StayInEndFrame | AnimationFlags.UpperBodyOnly | AnimationFlags.AllowRotation);
                PlayAnimation("random@getawaydriver", "idle_a", 2.5f, -1, AnimationFlags.StayInEndFrame);
                Tick += DisableControls;
            }
            else
            {
                ClearAnimationTask("random@mugging3", "handsup_standing_base");
                ClearAnimationTask("random@getawaydriver", "idle_a");
                Tick -= DisableControls;
            }
        }

        [Command("dropweapon")]
        private void DropWeapon()
        {
            TriggerEvent("Essentials:Client:dropWeapon");
        }

        [Command("camera")]
        private void OnCameraCommand()
        {
            if (!PlayerPed.IsOnFoot)
            {
                return;
            }

            usingCamera = !usingCamera;

            if (usingCamera)
            {
                CreateMobilePhone(0);
                CellCamActivate(true, true);

                Screen.Hud.IsVisible = false;
                Screen.Hud.IsRadarVisible = false;
                Tick += CameraControls;
            }
            else
            {
                Screen.Hud.IsVisible = true;
                Screen.Hud.IsRadarVisible = true;
                selfieCamera = false;

                DestroyMobilePhone();
                CellCamActivate(false, false);
            }
        }

        [Command("shuffle")]
        private void OnShuffleCommand() => ShuffleSeats();

        [Command("shuff")]
        private void OnShuffCommand() => ShuffleSeats();

        [Command("dv")]
        private async void DvVehicleCommand()
        {
            if (PlayerPed.CurrentVehicle is null)
            {
                Vehicle closestVehicle = PlayerPed.GetClosestVehicleToClient(3f);

                if (closestVehicle is null)
                {
                    ErrorNotification("You must be in or near a vehicle.");
                    return;
                }

                if (closestVehicle.Driver.Exists() && closestVehicle.Driver.IsPlayer)
                {
                    ErrorNotification("That vehicle still has a driver.");
                    return;
                }

                if (NetworkGetEntityOwner(closestVehicle.Handle) == Game.Player.Handle)
                {
                    bool deleted = await DvVehicle(closestVehicle);

                    DisplayNotification(deleted ? "~g~Vehicle deleted." : "~r~Failed to delete vehicle, try again.", true);
                }
                else
                {
                    TriggerServerEvent("Essentials:Server:deleteVehicle", closestVehicle.NetworkId);

                    int tempTimer = Game.GameTime;

                    while (closestVehicle.Exists())
                    {
                        if (Game.GameTime - tempTimer > 5000)
                        {
                            ErrorNotification("Failed to delete vehicle, try again.");
                            return;
                        }

                        await Delay(0);
                    }

                    DisplayNotification("~g~Vehicle deleted.");
                }
            }
            else if (PlayerPed.SeatIndex != VehicleSeat.Driver)
            {
                ErrorNotification("You must be the driver.");
            }
            else
            {
                bool deleted = await DvVehicle(PlayerPed.CurrentVehicle);

                DisplayNotification(deleted ? "~g~Deleted vehicle." : "~r~Failed to delete vehicle, try again.", true);
            }
        }

        [Command("delveh")]
        private void DelVehCommand() => DvVehicleCommand();

        [Command("flip")]
        private void OnFlipCommand()
        {
            Vehicle vehicle = PlayerPed.CurrentVehicle;

            if (vehicle is not null)
            {
                if (PlayerPed.SeatIndex != VehicleSeat.Driver)
                {
                    ErrorNotification("You must be the driver.");
                    return;
                }

                if (SetVehicleOnGroundProperly(vehicle.Handle))
                {
                    SuccessNotification("Flipped vehicle.");
                    return;
                }

                ErrorNotification("Failed to flip vehicle, try again.");
            }
            else
            {
                ErrorNotification("You must be in or near a vehicle.");
            }
        }

        [Command("trunk")]
        private void OnTrunkCommand()
        {
            Vehicle vehicle = PlayerPed.CurrentVehicle ?? PlayerPed.GetClosestVehicleToClient(1f);

            if (vehicle is null)
            {
                ErrorNotification("You must be in or near a vehicle.");
                return;
            }

            if (PlayerPed.CurrentVehicle is not null && PlayerPed.SeatIndex != VehicleSeat.Driver)
            {
                ErrorNotification("You must be the driver.");
                return;
            }

            if (vehicle.LockStatus != VehicleLockStatus.Unlocked)
            {
                ErrorNotification("You must unlock the car.");
                return;
            }

            if (vehicle.Doors[VehicleDoorIndex.Hood].IsBroken)
            {
                ErrorNotification("The trunk isn't intact.");
                return;
            }

            if (vehicle.Doors[VehicleDoorIndex.Trunk].IsOpen)
            {
                if (NetworkGetEntityOwner(vehicle.Handle) == Game.Player.Handle)
                {
                    vehicle.Doors[VehicleDoorIndex.Trunk].Close();
                }
                else
                {
                    TriggerServerEvent("Essentials:Server:doorAction", vehicle.NetworkId, (int)VehicleDoorIndex.Trunk, false);
                }

                SuccessNotification("Trunk closed.");
            }
            else
            {
                if (NetworkGetEntityOwner(vehicle.Handle) == Game.Player.Handle)
                {
                    vehicle.Doors[VehicleDoorIndex.Trunk].Open();
                }
                else
                {
                    TriggerServerEvent("Essentials:Server:doorAction", vehicle.NetworkId, (int)VehicleDoorIndex.Trunk, true);
                }

                SuccessNotification("Trunk opened.");
            }
        }

        [Command("hood")]
        private void OnHoodCommand()
        {
            Vehicle vehicle = PlayerPed.CurrentVehicle ?? PlayerPed.GetClosestVehicleToClient(1f);

            if (vehicle is null)
            {
                ErrorNotification("You must be in or near a vehicle.");
                return;
            }

            if (PlayerPed.CurrentVehicle is not null && PlayerPed.SeatIndex != VehicleSeat.Driver)
            {
                ErrorNotification("You must be the driver.");
                return;
            }

            if (vehicle.LockStatus != VehicleLockStatus.Unlocked)
            {
                ErrorNotification("You must unlock the car.");
                return;
            }

            if (vehicle.Doors[VehicleDoorIndex.Hood].IsBroken)
            {
                ErrorNotification("The hood isn't intact.");
                return;
            }

            if (vehicle.Doors[VehicleDoorIndex.Hood].IsOpen)
            {
                if (NetworkGetEntityOwner(vehicle.Handle) == Game.Player.Handle)
                {
                    vehicle.Doors[VehicleDoorIndex.Hood].Close();
                }
                else
                {
                    TriggerServerEvent("Essentials:Server:doorAction", vehicle.NetworkId, (int)VehicleDoorIndex.Hood, false);
                }

                SuccessNotification("Hood closed.");
            }
            else
            {
                if (NetworkGetEntityOwner(vehicle.Handle) == Game.Player.Handle)
                {
                    vehicle.Doors[VehicleDoorIndex.Hood].Open();
                }
                else
                {
                    TriggerServerEvent("Essentials:Server:doorAction", vehicle.NetworkId, (int)VehicleDoorIndex.Hood, true);
                }

                SuccessNotification("Hood opened.");
            }
        }

        [Command("door")]
        private void OnDoorCommand(string[] args)
        {
            Vehicle vehicle = PlayerPed.CurrentVehicle ?? PlayerPed.GetClosestVehicleToClient(1f);

            if (vehicle is null)
            {
                ErrorNotification("You must be in or near a vehicle.");
                return;
            }

            if (PlayerPed.CurrentVehicle is not null && PlayerPed.SeatIndex != VehicleSeat.Driver)
            {
                ErrorNotification("You must be the driver.");
                return;
            }

            if (vehicle.LockStatus != VehicleLockStatus.Unlocked)
            {
                ErrorNotification("You must unlock the car.");
                return;
            }

            if (!int.TryParse(args[0], out int doorIndex) || doorIndex < 0 || doorIndex > 4)
            {
                ErrorNotification("Invalid door.");
                return;
            }

            if (doorIndex > 0)
            {
                doorIndex--;
            }

            if (vehicle.Doors[(VehicleDoorIndex)doorIndex].IsBroken)
            {
                ErrorNotification("That door isn't intact.");
                return;
            }

            if (vehicle.Doors[(VehicleDoorIndex)doorIndex].IsOpen)
            {
                if (NetworkGetEntityOwner(vehicle.Handle) == Game.Player.Handle)
                {
                    vehicle.Doors[(VehicleDoorIndex)doorIndex].Close();
                }
                else
                {
                    TriggerServerEvent("Essentials:Server:doorAction", vehicle.NetworkId, doorIndex, false);
                }

                SuccessNotification("Door closed.");
            }
            else
            {
                if (NetworkGetEntityOwner(vehicle.Handle) == Game.Player.Handle)
                {
                    vehicle.Doors[(VehicleDoorIndex)doorIndex].Open();
                }
                else
                {
                    TriggerServerEvent("Essentials:Server:doorAction", vehicle.NetworkId, doorIndex, true);
                }

                SuccessNotification("Door opened.");
            }
        }

        [Command("anchor")]
        private void AnchorCommand()
        {
            if (!PlayerPed.IsInBoat)
            {
                ErrorNotification("You must be conning a boat.");
                return;
            }

            Vehicle boat = PlayerPed.CurrentVehicle;

            if (boat.Speed >= 5f)
            {
                ErrorNotification("You're going to fast to anchor the boat.");
                return;
            }

            if (IsBoatAnchoredAndFrozen(boat.Handle))
            {
                SetBoatAnchor(boat.Handle, false);
                SuccessNotification("Un-anchored boat.");
            }
            else
            {
                SetBoatFrozenWhenAnchored(boat.Handle, true);
                SetBoatAnchor(boat.Handle, true);
                SuccessNotification("Anchored boat.");
            }
        }

        [Command("eng")]
        private void OnEngCommand() => ToggleEngine();

        [Command("hu")]
        private void OnHuCommand() => HandsUp();

        [Command("hoh")]
        private void OnHohCommand() => HandsOnHead();

        [Command("huk")]
        private void OnHukCommand() => HandsUpKnees();

        [Command("dw")]
        private void OnDropWeaponCommand() => DropWeapon();
        #endregion

        #region Event Handlers

        [EventHandler("Essentials:Client:shuffleSeats")]
        private void ShuffleSeats()
        {
            Vehicle vehicle = PlayerPed.CurrentVehicle;

            if (vehicle is null)
            {
                return;
            }

            if (vehicle.Driver == Game.PlayerPed)
            {
                PlayerPed.SetIntoVehicle(vehicle, VehicleSeat.Passenger);
            }
            else
            {
                noShuffle = false;
            }
        }

        [EventHandler("Essentials:Client:doorAction")]
        private void OnDoorAction(int netId, int doorIndex, bool open)
        {
            Vehicle vehicle = (Vehicle)Entity.FromNetworkId(netId);

            if (vehicle is null)
            {
                Debug.WriteLine($"Got Network ID '{netId}' from doorAction event and wasn't able to convert to vehicle, bailing.");
                return;
            }

            if (open)
            {
                vehicle.Doors[(VehicleDoorIndex)doorIndex].Open();
            }
            else
            {
                vehicle.Doors[(VehicleDoorIndex)doorIndex].Close();
            }
        }

        [EventHandler("Essentials:Client:dropWeapon")]
        private async void OnDropWeapon()
        {
            if (PlayerPed.Exists())
            {
                SetPedDropsInventoryWeapon(PlayerPed.Handle, (uint)GetSelectedPedWeapon(PlayerPed.Handle), 1, 1, 1, -1);
                PlayerPed.Weapons.Give(WeaponHash.Unarmed, -1, true, true);
            }
        }
        #endregion

        [Tick]
        private async Task MainTick()
        {
            foreach (Ped ped in World.GetAllPeds())
            {
                ped.BlockPermanentEvents = true;
            }

            DistantCopCarSirens(false);
            await Delay(250);

            SetVehicleDensityMultiplierThisFrame(densityMultiplier);
            SetParkedVehicleDensityMultiplierThisFrame(densityMultiplier);
            SetRandomVehicleDensityMultiplierThisFrame(densityMultiplier);
            SetPedDensityMultiplierThisFrame(densityMultiplier);
            SetScenarioPedDensityMultiplierThisFrame(densityMultiplier, densityMultiplier);

            DisablePlayerVehicleRewards(Game.Player.Handle);
            SetRadarZoom(1100);
        }

        [Tick]
        private async Task SecondaryTick()
        {
            Vehicle vehicle = PlayerPed.CurrentVehicle;

            if (vehicle is null)
            {
                await Delay(500);
                return;
            }

            if (Game.IsControlPressed(0, Control.VehicleExit) && PlayerPed.IsAlive && vehicle.ClassType != VehicleClass.Helicopters && vehicle.ClassType != VehicleClass.Planes)
            {
                await Delay(150);

                if (Game.IsControlPressed(0, Control.VehicleExit) && PlayerPed.IsAlive)
                {
                    vehicle.IsEngineRunning = true;
                }
            }

            if (noShuffle)
            {
                if (vehicle.GetPedOnSeat(VehicleSeat.Passenger) == PlayerPed && GetIsTaskActive(PlayerPed.Handle, 165))
                {
                    if (!vehicle.IsSeatFree(VehicleSeat.Driver) && !vehicle.Driver.IsPlayer)
                    {
                        await Delay(2000);
                    }
                    else
                    {
                        PlayerPed.SetConfigFlag(184, true);
                        PlayerPed.Task.ClearAllImmediately();
                        PlayerPed.SetIntoVehicle(vehicle, VehicleSeat.Passenger);
                    }
                }
            }
            else if (vehicle.IsSeatFree(VehicleSeat.Driver))
            {
                PlayerPed.ResetConfigFlag(184);
                PlayerPed.SetIntoVehicle(vehicle, VehicleSeat.Driver);
                noShuffle = true;
            }

            if (vehicle.Driver != Game.PlayerPed || vehicle.Model.IsBicycle)
            {
                return;
            }

            if (vehicle.SteeringAngle > 20f)
            {
                steeringAngle = 40f;
            }
            else if (vehicle.SteeringAngle < -20f)
            {
                steeringAngle = -40f;
            }
            else if (vehicle.SteeringAngle is > 5f or < -5f)
            {
                steeringAngle = vehicle.SteeringAngle;
            }

            if (PlayerPed.IsOnFoot || vehicle.IsStopped)
            {
                vehicle.SteeringAngle = steeringAngle;
            }
        }

        [Tick]
        private async Task TeritaryTick()
        {
            for (int i = 0; i < 16; i++)
            {
                EnableDispatchService(i, false);
            }

            foreach (var model in suppressedModels)
            {
                SetVehicleModelIsSuppressed(Game.GenerateHashASCII(model), true);
            }

            DisablePlayerVehicleRewards(PlayerPed.Handle);

            Vector3 playerPos = PlayerPed.Position;
            ClearAreaOfCops(playerPos.X, playerPos.Y, playerPos.Z, 800.0f, 0);

            await Delay(100);
        }
    }
}