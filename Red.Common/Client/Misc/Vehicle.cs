using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Red.Common.Client.Diagnostics;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;
using static Red.Common.Client.Hud.HUD;

namespace Red.Common.Client.Misc
{
    public struct VehiclePaintColor
    {
        public static VehiclePaint PrimaryColor { get; set; }
        public static VehiclePaint SecondaryColor { get; set; }

        public static string PrimaryColorName
        {
            get { return GetColorName(PrimaryColor); }
        }

        public static string SecondaryColorName
        {
            get { return GetColorName(SecondaryColor); }
        }

        public static string GetColorName(VehiclePaint paint)
        {
            string name = Enum.GetName(typeof(VehiclePaint), paint);
            return name.Replace("_", " ");
        }
    }

    public class Vehicles
    {
        protected static readonly IReadOnlyList<string> tires = new List<string>
        {
            "wheel_lf", "wheel_rf", "wheel_lm1", "wheel_rm1", "wheel_lm2", "wheel_rm2", "wheel_lm3", "wheel_rm3", "wheel_lr", "wheel_rr"
        };

        protected static readonly IReadOnlyList<int> tiresIndex = new List<int>
        {
            0, 1, 2, 3, 45, 47, 46, 48, 4, 5
        };

        protected static Ped PlayerPed = Game.PlayerPed;
        protected static Ped Character = Game.Player.Character;
        protected static Player Player = Game.Player;

        protected static Vehicle ClosestVehicle = GetClosestVehicle(1.5f);
        protected static Tire ClosestTire = GetClosestTire(ClosestVehicle);

        public static void SetNeonLightsColor(Vehicle vehicle, Color color) => Function.Call<uint>(Hash._SET_VEHICLE_NEON_LIGHTS_COLOUR, vehicle, (int)color.R, (int)color.G, (int)color.B);

        public static bool IsNeonLightsEnabled(Vehicle vehicle, NeonLights neonLight)
        {
            if (Function.Call<bool>(Hash._IS_VEHICLE_NEON_LIGHT_ENABLED, vehicle, (int)neonLight))
            {
                return true;
            }
            else if (!Function.Call<bool>(Hash._IS_VEHICLE_NEON_LIGHT_ENABLED, vehicle, (int)neonLight))
            {
                return false;
            }
            else
            {
                return false;
            }
        }

        public static void GetVehicleNeonColor(Vehicle vehicle, int r, int g, int b) => GetVehicleNeonLightsColour(vehicle.Handle, ref r, ref g, ref b);
        public static Color GetNeonLightsColor(Vehicle vehicle)
        {
            return UnsafeGetNeonLightsColor(vehicle);
        }

        public static VehicleSeat GetClosestSeat(Vehicle vehicle, bool isUnseat)
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

            return closestSeat;
        }

        public static Tire GetClosestTire(Vehicle vehicle)
        {
            Vector3 playerPos = PlayerPed.Position;
            float closestDist = float.MaxValue;
            Tire closestTire = null;

            for (int i = 0; i < tires.Count; i++)
            {
                Vector3 tirePos = vehicle.Bones[tires[i]].Position;
                float dist = Vector3.DistanceSquared(tirePos, playerPos);

                if (dist < 1.5f && dist < closestDist)
                {
                    closestDist = dist;
                    closestTire = new Tire { BonePosition = tirePos, Distance = dist, TireIndex = tiresIndex[i] };
                }
            }

            return closestTire;
        }

        public static string GetVehicleNameFromModel(string modelName) => GetLabelText(GetDisplayNameFromVehicleModel((uint)GetHashKey(modelName)));
        public static void GetVehicleNameFromHash(uint hash) => GetDisplayNameFromVehicleModel(hash);

        public static void RepairVehicle() => PlayerPed.CurrentVehicle.Repair();

        #region Tires
        public static float GetClosestTireHeading() => GetHeadingFromVector_2d(ClosestTire.BonePosition.X - PlayerPed.Position.X, ClosestTire.BonePosition.Y - PlayerPed.Position.Y);

        public static bool HaveAnyTiresBurst() => tiresIndex.Any(t => IsVehicleTyreBurst(PlayerPed.CurrentVehicle.Handle, t, false));
        public static bool IsVehicleTireBurst(Vehicle vehicle, int tireIndex, bool completely = false) => IsVehicleTyreBurst(vehicle.Handle, tireIndex, completely);

        public static void SetVehicleTireBurst(Vehicle vehicle, int tireIndex, bool onRim, float time) => SetVehicleTyreBurst(vehicle.Handle, tireIndex, onRim, time);
        public static void SetVehicleTireBurst(Vehicle vehicle, TireIndex tireIndex, bool onRim, float time) => SetVehicleTyreBurst(vehicle.Handle, (int)tireIndex, onRim, time);
        public static void SetVehicleTireBurst(int vehicle, int tireIndex, bool onRim, float time) => SetVehicleTyreBurst(vehicle, tireIndex, onRim, time);
        public static void SetVehicleTireBurst(int vehicle, TireIndex tireIndex, bool onRim, float time) => SetVehicleTyreBurst(vehicle, (int)tireIndex, onRim, time);

        public static void SetVehicleTireFixed(Vehicle vehicle, int tireIndex) => SetVehicleTyreFixed(vehicle.Handle, tireIndex);
        public static void SetVehicleTireFixed(Vehicle vehicle, TireIndex tireIndex) => SetVehicleTireFixed(vehicle, (int)tireIndex);
        public static void SetVehicleTireFixed(int vehicle, int tireIndex) => SetVehicleTyreFixed(vehicle, tireIndex);
        public static void SetVehicleTireFixed(int vehicle, TireIndex tireIndex) => SetVehicleTyreFixed(vehicle, (int)tireIndex);

        public static void SetTireSmoke(Vehicle vehicle, int r, int g, int b) => SetVehicleTyreSmokeColor(vehicle.Handle, r, g, b);
        public static void SetTireSmoke(Vehicle vehicle, Color color) => SetVehicleTyreSmokeColor(vehicle.Handle, color.R, color.G, color.B);
        public static void SetTireSmoke(int vehicle, int r, int g, int b) => SetVehicleTyreSmokeColor(vehicle, r, g, b);
        public static void SetTireSmoke(int vehicle, Color color) => SetVehicleTyreSmokeColor(vehicle, color.R, color.G, color.B);

        public static void SetVehicleTiresCanBurst(Vehicle vehicle, bool toggle) => SetVehicleTyresCanBurst(vehicle.Handle, toggle);
        public static void SetVehicleTiresCanBurst(int vehicle, bool toggle) => SetVehicleTyresCanBurst(vehicle, toggle);
        
        public static void SetBulletProofTires(Vehicle vehicle) => SetVehicleTiresCanBurst(vehicle, false);
        public static void SetBulletProofTires(int vehicle) => SetVehicleTiresCanBurst(vehicle, false);

        public static void SetDriftTires(Vehicle vehicle) => SetDriftTyresEnabled(vehicle.Handle, true);
        public static void SetDriftTires(int vehicle) => SetDriftTyresEnabled(vehicle, true);

        public static void GetTireHealth(Vehicle vehicle, int tireIndex) => GetTyreHealth(vehicle.Handle, tireIndex);
        public static void GetTireHealth(Vehicle vehicle, TireIndex tireIndex) => GetTyreHealth(vehicle.Handle, (int)tireIndex);
        public static void GetTireHealth(int vehicle, int tireIndex) => GetTyreHealth(vehicle, tireIndex);
        public static void GetTireHealth(int vehicle, TireIndex tireIndex) => GetTyreHealth(vehicle, (int)tireIndex);

        public static void SetTireHealth(Vehicle vehicle, int tireIndex, float health) => SetTyreHealth(vehicle.Handle, tireIndex, health);
        public static void SetTireHealth(Vehicle vehicle, TireIndex tireIndex, float health) => SetTyreHealth(vehicle.Handle, (int)tireIndex, health);
        public static void SetTireHealth(int vehicle, int tireIndex, float health) => SetTyreHealth(vehicle, tireIndex, health);
        public static void SetTireHealth(int vehicle, TireIndex tireIndex, float health) => SetTyreHealth(vehicle, (int)tireIndex, health);
        #endregion

        // Forked from Albo1125 Vehicle Controls (https://github.com/Albo1125/VehicleControls/blob/master/VehicleControls/VehicleControls.cs#L88)
        public static void SetPlate(string plate)
        {
            if (Player != null && Character != null && Character.CurrentVehicle != null && Character.Exists())
            {
                Vehicle vehicle = Character.CurrentVehicle;

                if (plate.Length > 8)
                {
                    Log.Debug("Plate was set but the text is too long. Must have a total of 8 characters. (Numbers and Letters)");
                }
                else
                {
                    SetVehicleNumberPlateText(vehicle.Handle, plate);
                }
            }
        }

        public static void ToggleDoor(VehicleDoorIndex index)
        {
            VehicleDoor door = Character.CurrentVehicle.Doors[index];

            if (Player != null && Character != null && Character.CurrentVehicle != null && Character.Exists())
            {
                if (door != null)
                {
                    if (door.IsOpen)
                    {
                        door.Close();
                    }
                    else
                    {
                        door.Open();
                    }
                }
            }
        }

        public static void SetVehicleColors(Vehicle vehicle, VehicleColor primaryColor, VehicleColor secondaryColor)
        {
            SetVehicleColours(vehicle.Handle, (int)primaryColor, (int)secondaryColor);
        }

        #region Speeds
        public static float ConvertToMPH(float speed) => speed * 2.236936f;
        public static float ConvertFromMPH(float speed) => speed * 0.44704f;
        #endregion

        public static void ToggleVehicleEngine(Vehicle vehicle)
        {
            if (vehicle is null)
            {
                ErrorNotification("You must be in a vehicle");
                return;
            }

            PlayerPed.SetConfigFlag(429, true);
            vehicle.IsEngineRunning = !vehicle.IsEngineRunning;
        }

        #region Misc Methods
        private static unsafe Color UnsafeGetNeonLightsColor(Vehicle vehicle)
        {
            Color color;
            int red;
            int green;
            int blue;
            ulong GetVehicleNeonLightsColourHash = 0x7619eee8c886757f;
            Function.Call<uint>((Hash)GetVehicleNeonLightsColourHash, vehicle, &red, &green, &blue);

            return color = Color.FromArgb(red, green, blue);
        }
        #endregion
    }
}
