using System;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;
using static Red.Common.Client.Hud.HUD;
using static Red.Common.Client.Diagnostics.Log;

namespace Red.Grab.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected bool grabbed;
        protected Player grabberPlayer, grabbedPlayer;
        protected int escapeAttempts;
        protected readonly Random random = new();
        #endregion

        #region Commands

        [Command("grab")]
        private void OnGrabCommand() => GrabClosestPlayer();

        [Command("seat")]
        private void OnSeatCommand() => SeatGrabbedPlayer();

        [Command("unseat")]
        private void OnUnseatCommand() => UnseatPlayer();

        #endregion

        #region Event Handlers

        [EventHandler("Grab:Client:getGrabbed")]
        private void OnGetGrabbed(string sender)
        {
            grabbed = !grabbed;

            if (grabbed)
            {
                grabberPlayer = Players[int.Parse(sender)];

                Tick += DisableControls;
                Tick += GrabTick;

                if (!Game.PlayerPed.IsDead)
                {
                    DisplayHelpText("Spam ~INPUT_FRONTEND_RDOWN~ for a chance to escape.");
                }
            }
            else
            {
                Tick -= DisableControls;
                Tick -= GrabTick;

                grabberPlayer = null;
                Game.PlayerPed.Detach();
                escapeAttempts = 0;
            }
        }

        [EventHandler("Seat:Client:seatAction")]
        private void OnSeatAction(int netId, int seat, bool unseat)
        {
            Vehicle vehicle = (Vehicle)Entity.FromNetworkId(netId);

            if (vehicle is null)
            {
                Info($"Got Network ID '{netId}' from seatAction event and wasn't able to convert to vehicle, bailing.");
                return;
            }

            if (unseat)
            {
                Game.PlayerPed.Task.LeaveVehicle(LeaveVehicleFlags.WarpOut);
            }
            else
            {
                Game.PlayerPed.SetIntoVehicle(vehicle, (VehicleSeat)seat);
            }
        }

        [EventHandler("Grab:Client:showClientNotification")]
        private void OnShowClientNotification(string message) => Screen.ShowNotification(message, true);
        #endregion

        #region Methods
        private bool IsControlPressedRegardlessOfDisabled(Control control) => (Game.IsControlJustPressed(0, control) || Game.IsDisabledControlJustPressed(0, control)) && UpdateOnscreenKeyboard() != 0;

        private async void SeatGrabbedPlayer()
        {
            if (grabbedPlayer is null)
            {
                ErrorNotification("You must be grabbing someone to use this command.");
                return;
            }

            Vehicle closestVehicle = GetClosestVehicleToPlayer(3f);

            if (closestVehicle is null)
            {
                ErrorNotification("You must be close to a vehicle to use this.");
                return;
            }

            VehicleSeat closestSeat = GetClosestSeat(closestVehicle, false);

            if (closestSeat == VehicleSeat.None)
            {
                return;
            }

            if (closestVehicle.IsSeatFree(closestSeat))
            {
                GrabClosestPlayer();
                await Delay(10);
                TriggerServerEvent("Seat:Server:seatAction", grabbedPlayer.ServerId, closestVehicle.NetworkId, (int)closestSeat);
            }
        }

        private async void UnseatPlayer()
        {
            Vehicle closestVehicle = GetClosestVehicleToPlayer(3f);

            if (closestVehicle is null)
            {
                ErrorNotification("You must be close to a vehicle to use this.");
                return;
            }

            VehicleSeat closestSeat = GetClosestSeat(closestVehicle, true);

            if (closestSeat == VehicleSeat.None)
            {
                return;
            }

            Ped closestSeatedPed = closestVehicle.GetPedOnSeat(closestSeat);

            if (closestSeatedPed is not null && closestSeatedPed.IsPlayer)
            {
                Player closestPlayer = Players.FirstOrDefault(p => p.Character.NetworkId == closestSeatedPed.NetworkId);

                if (closestPlayer is not null)
                {
                    TriggerServerEvent("Seat:Server:seatAction", closestPlayer.ServerId, closestVehicle.NetworkId, 0, true);
                    await Delay(100);
                    GrabClosestPlayer();
                }
            }
        }

        private void GrabClosestPlayer()
        {
            if (grabbed)
            {
                return;
            }

            Player closestPlayer = GetClosestPlayer(4f);

            if (closestPlayer is null)
            {
                ErrorNotification("You must be closer to the person you wish to grab.", true);
                return;
            }

            grabbedPlayer = closestPlayer;
            TriggerServerEvent("Grab:Server:grabClosestPlayer", closestPlayer.ServerId);
        }

        private VehicleSeat GetClosestSeat(Vehicle vehicle, bool isUnseat)
        {
            Vector3 plyPos = Game.PlayerPed.Position;
            VehicleSeat closestSeat = VehicleSeat.None;
            float closestDist = float.MaxValue;

            if (isUnseat)
            {
                Vector3 lfPos = vehicle.Bones["handle_dside_f"].Position;
                float distLF = Vector3.DistanceSquared(plyPos, lfPos);
                if (distLF < 1f && distLF < closestDist)
                {
                    closestSeat = VehicleSeat.Driver;
                    closestDist = distLF;
                }
            }

            Vector3 rfPos = vehicle.Bones["handle_pside_f"].Position;
            float distRF = Vector3.DistanceSquared(plyPos, rfPos);
            if (distRF < 1f && distRF < closestDist)
            {
                closestSeat = VehicleSeat.Passenger;
                closestDist = distRF;
            }

            bool hasRearDriverHandle = vehicle.Bones.HasBone("handle_dside_r");
            bool hasRearDriverWindow = vehicle.Bones.HasBone("window_lr");

            if (hasRearDriverHandle || hasRearDriverWindow)
            {
                Vector3 lrPos = hasRearDriverHandle ? vehicle.Bones["handle_dside_r"].Position : vehicle.Bones["window_lr"].Position;
                float distLR = Vector3.DistanceSquared(plyPos, lrPos);
                if (distLR < 1f && distLR < closestDist)
                {
                    closestSeat = VehicleSeat.LeftRear;
                    closestDist = distLR;
                }
            }

            bool hasRearPassengerHandle = vehicle.Bones.HasBone("handle_pside_r");
            bool hasRearPassengerWindow = vehicle.Bones.HasBone("window_rr");

            if (hasRearPassengerHandle || hasRearPassengerWindow)
            {
                Vector3 rrPos = hasRearPassengerHandle ? vehicle.Bones["handle_pside_r"].Position : vehicle.Bones["window_rr"].Position;
                float distRR = Vector3.DistanceSquared(plyPos, rrPos);
                if (distRR < 1f && distRR < closestDist)
                {
                    closestSeat = VehicleSeat.RightRear;
                }
            }

            if (closestSeat == VehicleSeat.None)
            {
                ErrorNotification("Couldn't seat, try again.");
            }

            return closestSeat;
        }

        private async Task GrabTick()
        {
            int escapeChance = 10;

            if (Game.PlayerPed.IsCuffed)
            {
                escapeChance += 10;
            }

            AttachEntityToEntity(Game.PlayerPed.Handle, grabberPlayer.Character.Handle, 11816, 0.45f, 0.35f, 0f, 0f, 0f, 0f, false, false, false, false, 2, true);

            if (IsControlPressedRegardlessOfDisabled(Control.FrontendRdown) && escapeAttempts < 3 && !Game.Player.IsDead)
            {
                escapeAttempts++;

                if (escapeAttempts == 1)
                {
                    TriggerServerEvent("Grab:Server:escapeNotify", grabberPlayer.ServerId, "The person you're grabbing is attempting to wiggle out from your grip!");
                }

                int randomNum = random.Next(100);

                if (randomNum < escapeChance)
                {
                    TriggerServerEvent("Grab:Server:escapeNotify", grabberPlayer.ServerId, "They've wiggled out from your grip!");
                    Game.PlayerPed.Detach();
                    escapeAttempts = 0;
                    Tick -= DisableControls;
                    Tick -= GrabTick;
                }
                else if (escapeAttempts > 2)
                {
                    Screen.ShowNotification("~y~You've failed to wiggle out from their grip.", true);
                    ClearHelp(true);
                }

                await Delay(500);
            }
        }

        private async Task DisableControls()
        {
            Game.DisableAllControlsThisFrame(0);
            Game.EnableControlThisFrame(0, Control.LookLeftRight);
            Game.EnableControlThisFrame(0, Control.LookUpDown);
            Game.EnableControlThisFrame(0, Control.MpTextChatAll);
            Game.EnableControlThisFrame(0, Control.PushToTalk);
            Game.EnableControlThisFrame(0, Control.Phone);
        }
        #endregion
    }
}