using System;
using System.Threading.Tasks;
using System.Linq;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using static CitizenFX.Core.Native.API;

namespace Red.Grab.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected bool grabbed;
        protected int escapeAttempts;
        protected readonly Random random = new();
        protected Player grabberPlayer, grabbedPlayer;
        protected static Ped PlayerPed = Game.PlayerPed;
        #endregion

        #region Commands
        [Command("grab")]
        private void GrabCommand() => GrabClosestPlayer();

        [Command("seat")]
        private void SeatCommand() => SeatGrabbedPlayer();

        [Command("unseat")]
        private void UnseatCommand() => UnseatPlayer();
        #endregion

        #region Event Handlers
        [EventHandler("Grab:Client:getGrabbed")]
        private void OnGetGrabbed(string sender)
        {
            grabbed = !grabbed;

            if (grabbed)
            {
                grabberPlayer = Players[int.Parse(sender)];

                Tick += DisableControlsTick;
                Tick += GrabTick;

                if (!PlayerPed.IsDead)
                {
                    Screen.DisplayHelpTextThisFrame("Spam ~INPUT_FRONTEND_RDOWN~ for a chance to escape.");
                }
            }
            else
            {
                Tick -= DisableControlsTick;
                Tick -= GrabTick;

                grabberPlayer = null;
                PlayerPed.Detach();
                escapeAttempts = 0;
            }
        }

        [EventHandler("Seat:Client:seatAction")]
        private void OnSeatAction(int netId, int seat, bool unseat)
        {
            Vehicle vehicle = (Vehicle)Entity.FromNetworkId(netId);

            if (vehicle is null)
            {
                Debug.WriteLine($"[Seat]: Got Network Id '{netId}' from seatAction event and wasn't able to convert to vehicle, bailing.");
                return;
            }

            if (unseat)
            {
                PlayerPed.Task.LeaveVehicle(LeaveVehicleFlags.WarpOut);
            }
            else
            {
                PlayerPed.SetIntoVehicle(vehicle, (VehicleSeat)seat);
            }
        }
        #endregion

        #region Methods
        private async void SeatGrabbedPlayer()
        {
            if (grabbedPlayer is null)
            {
                Screen.ShowNotification("~r~~h~Error~h~~s~: You must be grabbing someone to use this command.", true);
                return;
            }

            Vehicle closestVehicle = GetClosestVehicle(3f);

            if (closestVehicle is null)
            {
                Screen.ShowNotification("~r~~h~Error~h~~s~: You must be close to a vehicle to use this.", true);
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
                TriggerServerEvent("pnw:framework:server:seatAction", grabbedPlayer.ServerId, closestVehicle.NetworkId, (int)closestSeat);
            }
        }

        private async void UnseatPlayer()
        {
            Vehicle closestVehicle = GetClosestVehicle(3f);

            if (closestVehicle is null)
            {
                Screen.ShowNotification("~r~~h~Error~h~~s~: You must be close to a vehicle to use this.", true);
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

        private Player GetClosestPlayer(float radius = 2f)
        {
            Vector3 plyPos = Game.PlayerPed.Position;
            Player closestPlayer = null;
            float closestDist = float.MaxValue;

            foreach (Player p in Players)
            {
                if (p is null || p == Game.Player)
                {
                    continue;
                }

                float dist = Vector3.DistanceSquared(p.Character.Position, plyPos);
                if (dist < closestDist && dist < radius)
                {
                    closestPlayer = p;
                    closestDist = dist;
                }
            }

            return closestPlayer;
        }

        private static Vehicle GetClosestVehicle(float radius = 2f)
        {
            RaycastResult raycast = World.RaycastCapsule(PlayerPed.Position, PlayerPed.Position, radius, (IntersectOptions)10, PlayerPed);
            return raycast.HitEntity as Vehicle;
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
                Screen.ShowNotification("~r~~h~Error~h~~s~: You must be closer to the person you wish to grab.", true);
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
                Screen.ShowNotification("~r~~h~Error~h~~s~: Couldn't seat, try again.", true);
            }

            return closestSeat;
        }

        private bool IsKeyJustPressedRegardlessOfDisabled(Control control) => Game.IsDisabledControlJustReleased(0, control) && Game.CurrentInputMode == InputMode.MouseAndKeyboard &&  UpdateOnscreenKeyboard() != 0;
        #endregion

        #region Ticks
        private async Task GrabTick()
        {
            int escapeChance = 10;

            if (Game.PlayerPed.IsCuffed)
            {
                escapeChance += 10;
            }

            AttachEntityToEntity(Game.PlayerPed.Handle, grabberPlayer.Character.Handle, 11816, 0.45f, 0.35f, 0f, 0f, 0f, 0f, false, false, false, false, 2, true);

            if (IsKeyJustPressedRegardlessOfDisabled(Control.FrontendRdown) && escapeAttempts < 3 && !Game.Player.IsDead)
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
                    Tick -= DisableControlsTick;
                    Tick -= GrabTick;
                }
                else if (escapeAttempts > 2)
                {
                    Screen.ShowNotification("You've failed to wiggle out from their grip.", true);
                    ClearHelp(true);
                }

                await Delay(500);
            }
        }

        private async Task DisableControlsTick()
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