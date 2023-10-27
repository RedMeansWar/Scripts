using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;

namespace Red.Common.Client
{
    public class Vehicles
    {
        #region Variables
        protected static Ped PlayerPed = Game.PlayerPed;
        protected static Ped Character = Game.Player.Character;
        protected static Player Player = Game.Player;
        protected static Vector3 PlayerPosition = PlayerPed.Position;
        protected static Random random = new();
        protected static Vehicle ClosestVehicle = GetClosestVehicle(1.5f);
        protected static Tire ClosestTire = GetClosestTire(ClosestVehicle);

        protected static readonly IReadOnlyList<string> tires = new List<string>
        {
            "wheel_lf", "wheel_rf", "wheel_lm1", "wheel_rm1", "wheel_lm2", "wheel_rm2", "wheel_lm3", "wheel_rm3", "wheel_lr", "wheel_rr"
        };

        protected static readonly IReadOnlyList<int> tiresIndex = new List<int>
        {
            0, 1, 2, 3, 45, 47, 46, 48, 4, 5
        };
        #endregion

        #region Light Colors
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
        #endregion

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

        #region Vehicle Plate
        public static void SetVehiclePlate(string plate)
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

        public static void SetPlate(string plate) => SetVehiclePlate(plate);
        #endregion
        
        public static void FixVehicle()
        {
            if (Player != null && Character != null && Character.CurrentVehicle != null && Character.Exists())
            {
                Vehicle vehicle = Character.CurrentVehicle;
                vehicle.Repair();
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

        #region Toggle Engine
        public static void ToggleVehicleEngine()
        {
            if (Player != null && Character != null && Character.CurrentVehicle != null && Character.Exists())
            {
                Vehicle vehicle = Character.CurrentVehicle;
                vehicle.IsEngineRunning = false;
            }
        }

        public static void ToggleEngine() => ToggleVehicleEngine();
        #endregion

        #region Tires
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

        #region Unsafe Methods
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

    public class Tire
    {
        public float Distance { get; set; }
        public Vector3 BonePosition { get; set; }
        public int TireIndex { get; set; }
    }
    #region Enums
    public enum TireIndex
    {
        FrontLeft = 0,
        FrontRight = 1,
        LeftMiddle1 = 2,
        RightMiddle1 = 3,
        LeftRear = 4,
        RightRear = 5,
        LeftMiddle2 = 45,
        RightMiddle2 = 47,
        LeftMiddle3 = 46,
        RightMiddle3 = 48,
    }

    public class WheelIndex
    {
        public static string FrontLeft = "wheel_lf";
        public static string FrontRight = "wheel_rf";
        public static string LeftMiddle1 = "wheel_lm1";
        public static string RightMiddle1 = "wheel_rm1";
        public static string LeftRear = "wheel_lm2";
        public static string RightRear = "wheel_rm2";
        public static string LeftMiddle2 = "wheel_lm3";
        public static string RightMiddle2 = "wheel_rm3";
        public static string LeftMiddle3 = "wheel_lr";
        public static string RightMiddle3 = "wheel_rr";
    }

    public enum NeonLights
    {
        Left = 0,
        Right = 1,
        Font = 2,
        Back = 3
    }

    public enum VehiclePaint
    {
        // CLASSIC | METALLIC
        Black = 0,
        Carbon_Black = 147,
        Graphite = 1,
        Anhracite_Black = 11,
        Black_Steel = 2,
        Dark_Steel = 3,
        Silver = 4,
        Bluish_Silver = 5,
        Rolled_Steel = 6,
        Shadow_Silver = 7,
        Stone_Silver = 8,
        Midnight_Silver = 9,
        Cast_Iron_Silver = 10,
        Red = 27,
        Torino_Red = 28,
        Formula_Red = 29,
        Lava_Red = 150,
        Blaze_Red = 30,
        Grace_Red = 31,
        Garnet_Red = 32,
        Sunset_Red = 33,
        Cabernet_Red = 34,
        Wine_Red = 143,
        Candy_Red = 35,
        Hot_Pink = 135,
        Pfister_Pink = 137,
        Salmon_Pink = 136,
        Sunrise_Orange = 36,
        Orange = 38,
        Bright_Orange = 138,
        Gold = 37,
        Bronze = 90,
        Yellow = 88,
        Race_Yellow = 89,
        Dew_Yellow = 91,
        Green = 139,
        Dark_Green = 49,
        Racing_Green = 50,
        Sea_Green = 51,
        Olive_Green = 52,
        Bright_Green = 53,
        Gasoline_Green = 54,
        Lime_Green = 92,
        Hunter_Green = 144,
        Securiror_Green = 125,
        Midnight_Blue = 141,
        Galaxy_Blue = 61,
        Dark_Blue = 62,
        Saxon_Blue = 63,
        Blue = 64,
        Bright_Blue = 140,
        Mariner_Blue = 65,
        Harbor_Blue = 66,
        Diamond_Blue = 67,
        Surf_Blue = 68,
        Nautical_Blue = 69,
        Racing_Blue = 73,
        Ultra_Blue = 70,
        Light_Blue = 74,
        Police_Car_Blue = 127,
        Epsilon_Blue = 157,
        Chocolate_Brown = 96,
        Bison_Brown = 101,
        Creek_Brown = 95,
        Feltzer_Brown = 94,
        Maple_Brown = 97,
        Beechwood_Brown = 103,
        Sienna_Brown = 104,
        Saddle_Brown = 98,
        Moss_Brown = 100,
        Woodbeech_Brown = 102,
        Straw_Brown = 99,
        Sandy_Brown = 105,
        Bleached_Brown = 106,
        Schafter_Purple = 71,
        Spinnaker_Purple = 72,
        Midnight_Purple = 142,
        Metallic_Midnight_Purple = 146,
        Bright_Purple = 145,
        Cream = 107,
        Ice_White = 111,
        Frost_White = 112,
        Pure_White = 134,
        Default_Alloy = 156,
        Champagne = 93,

        // Matte
        Matte_Black = 12,
        Matte_Gray = 13,
        Matte_Light_Gray = 14,
        Matte_Ice_White = 131,
        Matte_Blue = 83,
        Matte_Dark_Blue = 82,
        Matte_Midnight_Blue = 84,
        Matte_Midnight_Purple = 149,
        Matte_Schafter_Purple = 148,
        Matte_Red = 39,
        Matte_Dark_Red = 40,
        Matte_Orange = 41,
        Matte_Yellow = 42,
        Matte_Lime_Green = 55,
        Matte_Green = 128,
        Matte_Forest_Green = 151,
        Matte_Foliage_Green = 155,
        Matte_Brown = 129,
        Matte_Olive_Darb = 152,
        Matte_Dark_Earth = 153,
        Matte_Desert_Tan = 154,

        // Utiltity
        Util_Black = 15,
        Util_Black_Poly = 16,
        Util_Dark_Silver = 17,
        Util_Silver = 18,
        Util_Gun_Metal = 19,
        Util_Shadow_Silver = 20,
        Util_Red = 43,
        Util_Bright_Red = 44,
        Util_Garnet_Red = 45,
        Util_Dark_Green = 56,
        Util_Green = 57,
        Util_Dark_Blue = 75,
        Util_Midnight_Blue = 76,
        Util_Blue = 77,
        Util_Sea_Foam_Blue = 78,
        Util_Lightning_Blue = 79,
        Util_Maui_Blue_Poly = 80,
        Util_Bright_Blue = 81,
        Util_Brown = 108,
        Util_Medium_Brown = 109,
        Util_Light_Brown = 110,
        Util_Off_White = 122,

        // Worn
        Worn_Black = 21,
        Worn_Graphite = 22,
        Worn_Silver_Grey = 23,
        Worn_Silver = 24,
        Worn_Blue_Silver = 25,
        Worn_Shadow_Silver = 26,
        Worn_Red = 46,
        Worn_Golden_Red = 47,
        Worn_Dark_Red = 48,
        Worn_Dark_Green = 58,
        Worn_Green = 59,
        Worn_Sea_Wash = 60,
        Worn_Dark_Blue = 85,
        Worn_Blue = 86,
        Worn_Light_Blue = 87,
        Worn_Honey_Beige = 113,
        Worn_Brown = 114,
        Worn_Dark_Brown = 115,
        Worn_Straw_Beige = 116,
        Worn_Off_White = 121,
        Worn_Yellow = 123,
        Worn_Light_Orange = 124,
        Worn_Taxi_Yellow = 126,
        Worn_Orange = 130,
        Worn_White = 132,
        Worn_Olive_Army_Green = 133,

        // METALS
        Brushed_Steel = 117,
        Brushed_Black_Steel = 118,
        Brushed_Aluminum = 119,
        Pure_Gold = 158,
        Brushed_Gold = 159,
        Secret_Gold = 160,

        // CHROME
        Chrome = 120,
    }
    #endregion
}
