using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;
using static Red.Common.Client.Hud.HUD;

namespace Red.OpenInteriors.Client
{
    internal class ClientMain : BaseScript
    {
        // Forked TayMcKenziNZ Online Interiors (Converted to C#)
        #region Variables
        protected bool inCasino, showBigWin;
        protected int? videoWallRenderTarget = null;
        protected Blips blips;
        protected Ped PlayerPed = Game.PlayerPed;
        protected Vehicle CurrentVehicle = Game.PlayerPed.CurrentVehicle;

        protected readonly List<Blips> blipList = new()
        {
            new("Michael's House", (BlipColor)2, (BlipSprite)40, new(-817.93f, 177.68f, 72.22f)),
            new("Franklin's House", (BlipColor)69, (BlipSprite)40, new(8.32f, 539.75f, 176.03f)), // Rockford Hills
            new("Franklin's House", (BlipColor)69, (BlipSprite) 40, new(-14.19f, -1442.09f, 31.1f)), // Yee Yee Ass Haircut
            new("Trevor's Trailer", (BlipColor)40, (BlipSprite) 40, new(1981.51f, 3819.35f, 32.25f)),
            new("Floyd's House", (BlipColor)27, (BlipSprite) 40, new(-1157.31f, -1516.72f, 4.36f)),
            new("Lester's House", (BlipColor)1, (BlipSprite) 40, new(1274.65f, -1720.83f, 54.68f)),

            new("Lester's Factory", (BlipColor)3, (BlipSprite)475, new(717.93f, -976.46f, 24.91f)),
            new("Simeon's Dealership", 0, (BlipSprite)293, new(-68.22f, -1111.15f, 25.91f)),
            new("O'Niel Farm", (BlipColor)69, (BlipSprite)270, new(2453.57f, 4955.34f, 44.96f)),

            new("Apartment", (BlipColor)3, (BlipSprite)40, new(-911.91f, -451.08f, 39.61f)),
            new("Apartment", (BlipColor)3, (BlipSprite)40, new(-268.911f, -956.445f, 31.223f)),
            new("Apartment", (BlipColor)3, (BlipSprite)40, new(-47.49f, -585.85f, 37.95f)),
            new("Apartment", (BlipColor)3, (BlipSprite)40, new(-911.91f, -451.08f, 39.61f)),
            new("Apartment", (BlipColor)3, (BlipSprite)40, new(-933.50f, -384.39f, 38.96f)),
            new("Apartment", (BlipColor)3, (BlipSprite)40, new(-1447.31f, -537.77f, 34.74f)),
            new("Apartment", (BlipColor)3, (BlipSprite)40, new(-617.75f, 44.39f, 43.59f)),
            new("Apartment", (BlipColor)3, (BlipSprite)40, new(-773.88f, 311.73f, 85.70f)),
            new("Apartment", (BlipColor)3, (BlipSprite)40, new(-468.84f, -678.36f, 32.72f)),
            new("Apartment", (BlipColor)3, (BlipSprite)40, new(-810.06f, -978.83f, 14.22f)),
            new("Apartment", (BlipColor)3, (BlipSprite)40, new(292.25f, -162.46f, 64.62f)),

            new("House", (BlipColor)47, (BlipSprite)40, new(-169.286f, 486.4938f, 137.4436f)),
            new("House", (BlipColor)47, (BlipSprite)40, new(340.9412f, 437.1798f, 149.3925f)),
            new("House", (BlipColor)47, (BlipSprite)40, new(373.023f, 416.105f, 145.7006f)),
            new("House", (BlipColor)47, (BlipSprite)40, new(-676.127f, 588.612f, 145.1698f)),
            new("House", (BlipColor)47, (BlipSprite)40, new(-763.107f, 615.906f, 144.1401f)),
            new("House", (BlipColor)47, (BlipSprite)40, new(-857.798f, 682.563f, 152.6529f)),
            new("House", (BlipColor)47, (BlipSprite)40, new(120.500f, 549.952f, 184.097f)),
            new("House", (BlipColor)47, (BlipSprite)40, new(-1288.000f, 440.748f, 97.69459f)),
            new("Motel Room", (BlipColor)2, (BlipSprite)40, new(1121.45f, 2641.68f, 38.14f)),
            
            new("Office", (BlipColor)8, (BlipSprite)475, new(-68.342f, -799.828f, 44.227f)),
            new("Office", (BlipColor)8, (BlipSprite)475, new(-115.121f, -605.403f, 36.281f)),
            new("Office", (BlipColor)8, (BlipSprite)475, new(-1371.115f, -503.707f, 33.157f)),
            new("Office", (BlipColor)8, (BlipSprite)475, new(-1581.242f, -558.449f, 34.953f)),

            new("Cocaine Lockup", (BlipColor)30, (BlipSprite)497, new(51.92f, 6486.31f, 31.43f)),
            new("Counterfeit Cash Factory", (BlipColor)25, (BlipSprite)500, new(-1166.843f, -1386.159f, 4.971f)),
            new("Document Forgery Office", (BlipColor)49, (BlipSprite)498, new(1644.294f, 4857.999f, 41.011f)),
            new("Meth Lab", (BlipColor)83, (BlipSprite)499, new(1181.816f, -3113.832f, 6.028f)),
            new("Weed Farm", (BlipColor)25, (BlipSprite)496, new(102.446f, 176.030f, 104.716f)),

            new("F.I.B Office", (BlipColor)25, (BlipSprite)475, new(105.22f, -744.42f, 45.75f)),
            new("Facility", (BlipColor)2, (BlipSprite)590, new(1.79f, 6832.14f, 15.82f)),
            new("Bunker", (BlipColor)17, (BlipSprite)557, new(1571.97f, 2234.43f, 79.06f)),

            new("Garage", 0, (BlipSprite)357, new(507.87f, -1496.00f, 29.20f)),
            new("Garage", 0, (BlipSprite)357, new(639.16f, 2774.31f, 41.90f)),

            new("Hangar", (BlipColor)45, (BlipSprite)569, new(-1139.08f, -3387.34f, 13.94f)),
            
            new("Vehicle Warehouse", 0, (BlipSprite)524, new(-668.50f, -2385.95f, 13.93f)),
            new("Vehicle Warehouse", 0, (BlipSprite)524, new(266.64f, -1159.73f, 29.25f)),

            new("Crate Warehouse", 0, (BlipSprite)473, new(926.66f, -1560.23f, 30.74f)),
            new("Crate Warehouse", 0, (BlipSprite)473, new(-324.90f, -1356.23f, 31.30f)),
            new("Crate Warehouse", 0, (BlipSprite)473, new(274.54f, -3015.40f, 5.70f)),

            new("Clubhouse", (BlipColor)28, (BlipSprite)492, new(973.487f, -101.972f, 74.850f)),
            new("Clubhouse", (BlipColor)28, (BlipSprite)492, new(-38.47f, 6419.88f, 31.49f)),
            new("Clubhouse", (BlipColor)28, (BlipSprite)492, new(1737.78f, 3709.592f, 34.14f)),

            new("Nightclub", (BlipColor)55, (BlipSprite)614, new(346.02f, -977.81f, 29.37f)),

            new("Yacht", (BlipColor)3, (BlipSprite)455, new(-2043.97f, -1031.58f, 11.98f)),
            new("Yacht", (BlipColor)3, (BlipSprite)455, new(-1363.72f, 6734.10f, 2.44f)),
            new("Yacht", (BlipColor)3, (BlipSprite)455, new(3611.22f, -4781.03f, 11.91f)),

            new("Arcade", (BlipColor)15, (BlipSprite)647, new(758.76f, -816.06f, 26.29f)),
            new("Split Sides West", 0, (BlipSprite)102, new(-430.17f, 261.5f, 82.32f)),
            new("LSIA", 0, (BlipSprite)307, new(-1045.95f, -2751.59f, 21.0f)),
            new("Morgue", (BlipColor)49, (BlipSprite)61, new(240.74f, -1379.18f, 33.55f)),
            new("Bahama Mamas", 0, (BlipSprite)93, new(-1388.67f, -586.68f, 30.2f)),
            new("Movie Theater", 0, (BlipSprite)135, new(-1423.6f, -215.54f, 45.81f)),
            new("Martin Madrazo's Ranch", (BlipColor)48, (BlipSprite)414, new(1395.21f, 1141.76f, 114.4f)),
            new("Submarine", (BlipColor)3, (BlipSprite)308, new(493.79f, -3222.95f, 10.5f)),
            new("Server Farm", 0, (BlipSprite)521, new(2476.11f, -384.15f, 94.2f)),
            new("IAA Facility", (BlipColor)74, (BlipSprite)590, new(2049.84f, 2949.75f, 47.55f)),
            new("Doomsday Facility", (BlipColor)14, (BlipSprite)548, new(-356.04f, 4823.27f, 142.74f)),

            new("Police Station", (BlipColor)57, (BlipSprite)60, new(1107.68f, -844.84f, 19.32f)),
            new("Police Station", (BlipColor)57, (BlipSprite)60, new(-441.2f, 6018.61f, 31.54f)),
            new("Police Station", (BlipColor)57, (BlipSprite)60, new(434.15f, -981.85f, 30.71f)),
            new("Police Station", (BlipColor)57, (BlipSprite)60, new(1855.53f, 3683.04f, 34.27f)),

            new("Fire Station", (BlipColor)1, (BlipSprite)635, new(1200.46f, -1457.68f, 34.88f)),
            new("Fire Station", (BlipColor)1, (BlipSprite)635, new(216.61f, -1637.6f, 29.49f)),

            new("Hospital", (BlipColor)49, (BlipSprite)61, new(291.7f, -586.94f, 43.2f)),
            new("Hospital", (BlipColor)49, (BlipSprite)61, new(266.69f, -1432.62f, 29.33f)),

            new("Humane Labs", (BlipColor)76, (BlipSprite)80, new(3545.68f, 3776.25f, 29.36f)),

            new("Bar", (BlipColor)46, (BlipSprite)93, new(1992.45f, 3058.73f, 47.06f)),
            new("Bar", (BlipColor)46, (BlipSprite)93, new(-564.93f, 271.58f, 83.02f)),

            new("Strip Club", (BlipColor)48, (BlipSprite)121, new(133.08f, -1305.86f, 29.16f)),
            new("Legion Square", (BlipColor)30, (BlipSprite)77, new(189.13f, -967.22f, 29.82f)),
            new("Sandy Shores Airfield", (BlipColor)4, (BlipSprite)584, new(133.08f, -1305.86f, 29.16f)),

            new("Store", (BlipColor)4, (BlipSprite)59, new(28.88f, -1351.34f, 29.34f)),
            new("Store", (BlipColor)4, (BlipSprite)59, new(1159.52f, -326.66f, 69.22f)),
            new("Store", (BlipColor)4, (BlipSprite)59, new(1166.72f, 2707.77f, 38.16f)),
            new("Store", (BlipColor)4, (BlipSprite)59, new(544.59f, 2669.36f, 42.16f)),
            new("Store", (BlipColor)4, (BlipSprite)59, new(1393.16f, 3601.69f, 34.98f)),
            new("Store", (BlipColor)4, (BlipSprite)59, new(-1225.37f, -904.8f, 12.33f)),
            new("Store", (BlipColor)4, (BlipSprite)59, new(-3041.08f, 589.04f, 7.91f)),
            new("Store", (BlipColor)4, (BlipSprite)59, new(-2969.37f, 390.49f, 15.04f)),
            new("Store", (BlipColor)4, (BlipSprite)59, new(1701.39f, 4927.68f, 42.06f)),
            new("Store", (BlipColor)4, (BlipSprite)59, new(-51.51f, -1755.9f, 29.42f)),
            new("Store", (BlipColor)4, (BlipSprite)59, new(-3240.84f, 1004.61f, 12.83f)),
            new("Store", (BlipColor)4, (BlipSprite)59, new(2681.04f, 3282.96f, 55.24f)),
            new("Store", (BlipColor)4, (BlipSprite)59, new(1965.57f, 3740.15f, 32.33f)),
            new("Store", (BlipColor)4, (BlipSprite)59, new(-1488.89f, -381.45f, 40.16f)),
            new("Store", (BlipColor)4, (BlipSprite)59, new(376.89f, 324.78f, 103.57f)),
            new("Store", (BlipColor)4, (BlipSprite)59, new(2558.23f, 385.5f, 108.62f)),
            new("Store", (BlipColor)4, (BlipSprite)59, new(1141.72f, -980.74f, 46.21f)),
            new("Store", (BlipColor)4, (BlipSprite)59, new(1730.59f, 6411.09f, 35.00f)),
            new("Store", (BlipColor)4, (BlipSprite)59, new(-1826.39f, 792.25f, 142.76f)),
            new("Store", (BlipColor)4, (BlipSprite)59, new(-712.36f, -911.79f, 23.76f)),
            new("Store", (BlipColor)4, (BlipSprite)59, new(-492.38f, -342.63f, 42.32f)),

            new("Del Perro Pier", (BlipColor)4, (BlipSprite)266, new(-1843.5f, -1219.53f, 13.02f)),
            new("Vangelico Jewlery", (BlipColor)28, (BlipSprite)617, new(-631.57f, -237.9f, 38.08f)),
            new("Cluckin' Bell Factory", (BlipColor)4, (BlipSprite)84, new(-71.49f, 6266.47f, 31.15f)),
            new("Casino", (BlipColor)4, (BlipSprite)679, new(917.3f, 50.76f, 80.9f)),

            new("Bolingbroke Penitentiary", (BlipColor)47, (BlipSprite)526, new(1851.78f, 2606.26f, 45.67f)),
            new("Fort Zancudo", (BlipColor)76, (BlipSprite)421, new(-2067.11f, 3097.35f, 32.81f)),
            new("Benny's Original Motorwork", (BlipColor)64, (BlipSprite)446, new(-205.57f, -1309.5f, 30.72f)),

            new("Los Santos Customs", (BlipColor)64, (BlipSprite)72, new(-364.84f, -131.46f, 38.68f)),
            new("Los Santos Customs", (BlipColor)64, (BlipSprite)72, new(714.01f, -1082.67f, 22.33f)),
            new("Los Santos Customs", (BlipColor)64, (BlipSprite)72, new(-1135.07f, -1984.93f, 13.17f)),
            new("Los Santos Customs", (BlipColor)64, (BlipSprite)72, new(1193.45f, 2670.64f, 37.78f)),
            new("Los Santos Customs", (BlipColor)64, (BlipSprite)72, new(120.33f, 6608.87f, 31.92f)),
            // Gamebuild 2372
            new("Tuner Garage", (BlipColor)48, (BlipSprite)524, new(804.21f, -963.61f, 42.57f)),
            new("Tuner Car Meet", (BlipColor)2, (BlipSprite)777, new(776.35f, -1867.26f, 52.93f)),
            // Gamebuild 2545
            new("Agency", 0, (BlipSprite)826, new(-1035.3f, -431.42f, 39.62f)),
            new("Agency", 0, (BlipSprite)826, new(384.8140f, -60.7270f, 102.3630f)),
            new("Agency", 0, (BlipSprite)826, new(-1003.9110f, -759.6040f, 60.894190f)),

            new("Record A Studios", (BlipColor)64, (BlipSprite)72, new(-841.51f, -229.07f, 37.27f)),

            new("Therapist Office", (BlipColor)0, (BlipSprite)205, new(-1902.15f, -564.28f, 11.82f)),
            new("Solomon's Office", (BlipColor)70, (BlipSprite)475, new(-1011.41f, -479.98f, 39.97f)),
            new("Torture Room", (BlipColor)1, (BlipSprite)458, new(960.57f, -2185.24f, 30.5f)),

            new("Bank", (BlipColor)25, (BlipSprite)207, new(-115.09f, 6458.77f, 31.47f)),
            new("Bank", (BlipColor)25, (BlipSprite)207, new(229.91f, 214.37f, 105.56f)),
            // Criminal Enterprise DLC
            new("Farmhouse Interior", 0, (BlipSprite)84, new(1930.07f, 4634.88f, 40.47f)),
            //LS Drug Wars DLC
            new("Drug RV", (BlipColor)2, (BlipSprite)499, new(2318.85f, 2553.79f, 47.69f)),
            new("Freakshop Hideout", (BlipColor)2, (BlipSprite)473, new(599.0f, -426.12f, 266.14f)),
            new("Vinewood Rooftop", (BlipColor)48, (BlipSprite)184, new(-271.73f, 289.52f, 104.99f)),
            // LS Mercenary DLC
            new("The Vinewood Car Club", (BlipColor)49, (BlipSprite)524, new(1233.44f, -3234.42f, 5.53f)),
            new("ort Zancudo Secret Base", (BlipColor)1, (BlipSprite)66, new(-2052.02f, 3237.51f, 31.5f)),
        };

        public static Dictionary<int, TeleportData> teleports = new()
        {
            [1] = { text = "MazeBank Helipad", dest = { 2, 3, 8, 9 }, coord = new(-75.21f, -824.83f, 321.29f), h = 157.83f, veh = false },
            [2] = { text = "MazeBank Office", dest = { 1, 3, 8, 9 }, coord = new(-75.46f, -827.14f, 242.50f), h = 67.20f, veh = false },
            [3] = { text = "MazeBank Modshop", dest = { 1, 2, 8, 9 }, coord = new(-70.08f, -827.78f, 285.00f), h = 71.42f },
            [4] = { text = "MazeBank Modshop", dest = { 5, 6, 7, 10 }, coord = new(-72.77f, -814.61f, 285.00f), h = 158.89f },
            [5] = { text = "Mazebank Car Garage - Floor 1C", dest = { 4, 6, 7, 10 }, coord = new(-70.29f, -834.81f, 232.68f), h = 339.08f },
            [6] = { text = "Mazebank Car Garage - Floor 1B", dest = { 4, 5, 7, 10 }, coord = new(-70.29f, -834.81f, 227.35f), h = 339.08f },
            [7] = { text = "Mazebank Car Garage - Floor 1A", dest = { 4, 5, 6, 10 }, coord = new(-70.29f, -834.81f, 221.99f), h = 339.08f },
            [8] = { text = "Mazebank Car Garage", dest = { 1, 2, 3, 9 }, coord = new(-91.33f, -821.34f, 222.00f), h = 251.88f },
            [9] = { text = "MazeBank Lobby", dest = { 1, 2, 3, 8 }, coord = new(-68.69f, -801.04f, 44.22f), h = 337.14f, veh = false },
            [10] = { text = "MazeBank Parking Garage", dest = { 4, 5, 6, 7 }, coord = new(-84.19f, -821.56f, 36.02f), h = 350.11f },
            [11] = { text = "Arcadius Helipad", dest = { 12, 13, 18, 19 }, coord = new(-144.63f, -599.27f, 206.91f), h = 157.78f, veh = false },
            [12] = { text = "Arcadius Office", dest = { 11, 13, 18, 19 }, coord = new(-141.40f, -621.01f, 167.90f), h = 275.04f, veh = false },
            [13] = { text = "Arcadius Modshop", dest = { 11, 12, 18, 19 }, coord = new(-146.36f, -603.84f, 167.00f), h = 41.27f },
            [14] = { text = "Arcadius Modshop", dest = { 15, 16, 17, 20 }, coord = new(-142.37f, -591.26f, 167.00f), h = 130.70f },
            [15] = { text = "Arcadius Car Garage - Floor 1C", dest = { 14, 16, 17, 20 }, coord = new(-173.25f, -583.49f, 146.69f), h = 354.00f },
            [16] = { text = "Arcadius Car Garage - Floor 1B", dest = { 14, 15, 17, 20 }, coord = new(-173.25f, -583.49f, 141.34f), h = 354.00f },
            [17] = { text = "Arcadius Car Garage - Floor 1A", dest = { 14, 15, 16, 20 }, coord = new(-173.25f, -583.49f, 136.00f), h = 354.00f },
            [18] = { text = "Arcadius Car Garage", dest = { 11, 12, 13, 19 }, coord = new(-198.13f, -580.68f, 136.00f), h = 282.80f },
            [19] = { text = "Arcadius Lobby", dest = { 11, 12, 13, 18 }, coord = new(-115.88f, -604.96f, 36.28f), h = 248.76f, veh = false },
            [20] = { text = "Arcadius Parking Garage", dest = { 14, 15, 16, 17 }, coord = new(-143.92f, -575.99f, 32.42f), h = 159.14f },
            [21] = { text = "W. MazeBank Helipad", dest = { 22, 23, 28, 29 }, coord = new(-1368.91f, -471.64f, 84.44f), h = 302.96f, veh = false },
            [22] = { text = "W. MazeBank Office", dest = { 21, 23, 28, 29 }, coord = new(-1392.67f, -480.18f, 71.20f), h = 2.53f, veh = false },
            [23] = { text = "W. MazeBank Modshop", dest = { 21, 22, 28, 29 }, coord = new(-1397.59f, -470.40f, 78.19f), h = 277.99f },
            [24] = { text = "W. MazeBank Modshop", dest = { 25, 26, 27, 30 }, coord = new(-1388.75f, -480.90f, 78.20f), h = 8.45f },
            [25] = { text = "W. Mazebank Car Garage - Floor 1C", dest = { 24, 26, 27, 30 }, coord = new(-1370.84f, -481.24f, 59.78f), h = 353.27f },
            [26] = { text = "W. Mazebank Car Garage - Floor 1B", dest = { 24, 25, 27, 30 }, coord = new(-1370.84f, -481.24f, 54.44f), h = 353.27f },
            [27] = { text = "W. Mazebank Car Garage - Floor 1A", dest = { 24, 25, 26, 30 }, coord = new(-1370.84f, -481.24f, 49.10f), h = 353.27f },
            [28] = { text = "W. Mazebank Car Garage", dest = { 21, 22, 23, 29 }, coord = new(-1395.90f, -480.69f, 49.10f), h = 286.12f },
            [29] = { text = "W. MazeBank Lobby", dest = { 21, 22, 23, 28 }, coord = new(-1370.96f, -503.46f, 33.15f), h = 150.13f, veh = false },
            [30] = { text = "W. MazeBank Parking Garage", dest = { 24, 25, 26, 27 }, coord = new(-1362.40f, -472.13f, 31.59f), h = 103.50f },
            [31] = { text = "Lombank Helipad", dest = { 32, 36, 37 }, coord = new(-1570.19f, -576.45f, 114.44f), h = 35.73f, veh = false },
            [32] = { text = "Lombank Office", dest = { 31, 36, 37 }, coord = new(-1579.30f, -564.77f, 107.70f), h = 301.98f, veh = false },
            [33] = { text = "Lombank Car Garage - Floor 1C", dest = { 34, 35, 38 }, coord = new(-1575.78f, -584.69f, 97.19f), h = 294.88f },
            [34] = { text = "Lombank Car Garage - Floor 1B", dest = { 33, 35, 38 }, coord = new(-1575.78f, -584.69f, 91.85f), h = 294.88f },
            [35] = { text = "Lombank Car Garage - Floor 1A", dest = { 33, 34, 38 }, coord = new(-1575.78f, -584.69f, 86.50f), h = 294.88f },
            [36] = { text = "Lombank Car Garage", dest = { 31, 32, 37 }, coord = new(-1585.95f, -561.92f, 86.50f), h = 218.17f },
            [37] = { text = "Lombank Lobby", dest = { 31, 32, 36 }, coord = new(-1581.47f, -558.02f, 34.95f), h = 37.76f, veh = false },
            [38] = { text = "Lombank Parking Garage", dest = { 33, 34, 35 }, coord = new(-1537.43f, -577.77f, 25.70f), h = 36.26f },
            [39] = { text = "3 Alta Street, Apt 57", dest = { 41 }, coord = new(-269.87f, -941.05f, 92.51f), h = 65.59f, veh = false },
            [40] = { text = "3 Alta Street, Apt 10", dest = { 41 }, coord = new(-273.23f, -967.30f, 77.23f), h = 247.85f, veh = false },
            [41] = { text = "3 Alta Street Lobby", dest = { 39, 40 }, coord = new(-268.91f, -956.44f, 31.22f), h = 202.50f, veh = false },
            [42] = { text = "4 Integrity Way, Apt 35", dest = { 45 }, coord = new(-25.51f, -607.33f, 100.24f), h = 241.47f, veh = false },
            [43] = { text = "4 Integrity Way, Apt 30", dest = { 45 }, coord = new(-18.47f, -591.43f, 90.11f), h = 339.32f, veh = false },
            [44] = { text = "4 Integrity Way, Apt 28", dest = { 45 }, coord = new(-31.04f, -595.27f, 80.03f), h = 246.76f, veh = false },
            [45] = { text = "4 Integrity Way Lobby", dest = { 42, 43, 44 }, coord = new(-47.49f, -585.85f, 37.95f), h = 64.96f, veh = false },
            [46] = { text = "Weazel Plaza, Apt 101", dest = { 49 }, coord = new(-907.69f, -453.55f, 126.53f), h = 211.15f, veh = false },
            [47] = { text = "Weazel Plaza, Apt 70", dest = { 49 }, coord = new(-890.67f, -436.73f, 121.60f), h = 23.89f, veh = false },
            [48] = { text = "Weazel Plaza, Apt 26", dest = { 49 }, coord = new(-890.69f, -452.86f, 95.46f), h = 286.55f, veh = false },
            [49] = { text = "Weazel Plaza Lobby", dest = { 46, 47, 48 }, coord = new(-911.91f, -451.08f, 39.61f), h = 116.77f, veh = false },
            [50] = { text = "Richards Majestic, Apt 51", dest = { 152, 53 }, coord = new(-907.13f, -372.43f, 109.44f), h = 23.43f, veh = false },
            [51] = { text = "Richards Majestic, Apt 4", dest = { 152, 53 }, coord = new(-922.99f, -378.56f, 85.48f), h = 217.47f, veh = false },
            [52] = { text = "Richards Majestic, Apt 2", dest = { 152, 53 }, coord = new(-912.96f, -365.21f, 114.28f), h = 112.55f, veh = false },
            [53] = { text = "Richards Majestic Lobby", dest = { 152, 50, 51, 52 }, coord = new(-933.50f, -384.39f, 38.96f), h = 121.42f, veh = false },
            [54] = { text = "Del Perro Heights, Apt 20", dest = { 57, 155 }, coord = new(-1449.93f, -525.76f, 69.55f), h = 30.20f, veh = false },
            [55] = { text = "Del Perro Heights, Apt 7", dest = { 57, 155 }, coord = new(-1449.98f, -525.8f, 56.93f), h = 29.66f, veh = false },
            [56] = { text = "Del Perro Heights, Apt 4", dest = { 57, 155 }, coord = new(-1452.48f, -540.26f, 74.04f), h = 33.06f, veh = false },
            [57] = { text = "Del Perro Heights Lobby", dest = { 54, 55, 56, 155 }, coord = new(-1447.31f, -537.77f, 34.74f), h = 208.41f, veh = false },
            [58] = { text = "Tinsel Towers, Apt 45", dest = { 61 }, coord = new(-596.40f, 56.10f, 108.20f), h = 356.60f, veh = false },
            [59] = { text = "Tinsel Towers, Apt 42", dest = { 61 }, coord = new(-603.43f, 58.96f, 98.20f), h = 88.01f, veh = false },
            [60] = { text = "Tinsel Towers, Apt 29", dest = { 61 }, coord = new(-604.99f, 51.2f, 93.63f), h = 169.52f, veh = false },
            [61] = { text = "Tinsel Towers Lobby", dest = { 58, 59, 60 }, coord = new(-617.75f, 44.39f, 43.59f), h = 179.79f, veh = false },
            [62] = { text = "Eclipse Towers, Penthouse 3", dest = { 70, 71 }, coord = new(-774.32f, 341.98f, 196.68f), h = 94.64f, veh = false },
            [63] = { text = "Eclipse Towers, Penthouse 2", dest = { 70, 71 }, coord = new(-786.78f, 315.74f, 187.92f), h = 270.71f, veh = false },
            [64] = { text = "Eclipse Towers, Penthouse 1", dest = { 70, 71 }, coord = new(-786.82f, 315.78f, 217.63f), h = 272.12f, veh = false },
            [65] = { text = "Eclipse Towers, Apt 40", dest = { 70, 71 }, coord = new(-781.99f, 326.06f, 223.25f), h = 169.69f, veh = false },
            [66] = { text = "Eclipse Towers, Apt 31", dest = { 70, 71 }, coord = new(-774.61f, 331.51f, 160.00f), h = 356.70f, veh = false },
            [67] = { text = "Eclipse Towers, Apt 9", dest = { 70, 71 }, coord = new(-781.83f, 326.10f, 176.81f), h = 179.20f, veh = false },
            [68] = { text = "Eclipse Towers, Apt 5", dest = { 70, 71 }, coord = new(-774.12f, 331.18f, 207.62f), h = 356.29f, veh = false },
            [69] = { text = "Eclipse Towers, Apt 3", dest = { 70, 71 }, coord = new(-784.80f, 323.63f, 211.99f), h = 263.39f, veh = false },
            [70] = { text = "Eclipse Towers Lobby", dest = { 62, 63, 64, 65, 66, 67, 68, 69, 71 }, coord = new(-773.88f, 311.73f, 85.70f), h = 171.28f, veh = false },
            [71] = { text = "Eclipse Towers Garage", dest = { 62, 63, 64, 65, 66, 67, 68, 69, 70 }, coord = new(240.45f, -1004.74f, -99.00f), h = 97.03f },
            [72] = { text = "Eclipse Towers Garage", dest = { 73 }, coord = new(228.16f, -1002.04f, -99.00f), h = 359.13f },
            [73] = { text = "Exit Eclipse Towers Garage", dest = { }, coord = new(-800.42f, 332.97f, 85.70f), h = 178.81f },
            [74] = { text = "Eclipse Towers Garage", dest = { 72 }, coord = new(-791.71f, 332.84f, 85.70f), h = 349.23f },
            [75] = { text = "302 San Andreas Ave, Apt 2", dest = { 77 }, coord = new(-467.52f, -708.72f, 77.09f), h = 271.91f, veh = false },
            [76] = { text = "302 San Andreas Ave, Apt 1", dest = { 77 }, coord = new(-468.07f, -689.57f, 53.40f), h = 92.59f, veh = false },
            [77] = { text = "302 San Andreas Ave Lobby", dest = { 75, 76 }, coord = new(-468.84f, -678.36f, 32.72f), h = 367.38f, veh = false },
            [78] = { text = "Mid-End Garage", dest = { 79 }, coord = new(198.39f, -1002.87f, -99.00f), h = 357.40f },
            [79] = { text = "Exit Mid-End Garage", dest = { }, coord = new(501.13f, -1496.61f, 28.70f), h = 178.86f },
            [80] = { text = "Mid-End Garage", dest = { 78 }, coord = new(507.87f, -1496.00f, 29.29f), h = 0.23f },
            [81] = { text = "Low-End Garage", dest = { 82 }, coord = new(172.74f, -1005.87f, -99.02f), h = 0.50f },
            [82] = { text = "ExitLow-End Garag", dest = { 81 }, coord = new(639.16f, 2774.31f, 41.98f), h = 4.00f },
            [83] = { text = "Nightclub", dest = { 84 }, coord = new(-1569.37f, -3017.17f, -74.41f), h = 0.32f, veh = false },
            [84] = { text = "Exit Nightclub ", dest = { 83 }, coord = new(346.02f, -977.81f, 29.37f), h = 277.02f, veh = false },
            [85] = { text = "Exit Nightclub", dest = { 86, 87 }, coord = new(333.28f, -997.71f, 29.12f), h = 186.64f },
            [86] = { text = "Nightclub Garage", dest = { 85, 87 }, coord = new(-1637.61f, -2989.78f, -77.54f), h = 261.02f },
            [87] = { text = "Nightclub Basement", dest = { 85, 86 }, coord = new(-1515.97f, -2978.62f, -80.89f), h = 268.58f },
            [88] = { text = "Nightclub Garage", dest = { 89 }, coord = new(-1618.58f, -2998.95f, -78.15f), h = 0.79f },
            [89] = { text = "Nightclub Basement", dest = { 88 }, coord = new(-1507.62f, -3017.20f, -79.24f), h = 347.61f, veh = false },
            [90] = { text = "3655 Wild Oats Drive", dest = { 91 }, coord = new(-174.33f, 497.53f, 137.67f), h = 190.77f, veh = false },
            [91] = { text = "Exit House", dest = { 90 }, coord = new(-174.90f, 502.37f, 137.42f), h = 77.73f, veh = false },
            [92] = { text = "Bedroom", dest = { 93 }, coord = new(-167.59f, 478.52f, 133.84f), h = 344.39f, veh = false },
            [93] = { text = "Deck", dest = { 92 }, coord = new(-167.33f, 476.79f, 133.90f), h = 189.39f, veh = false },
            [94] = { text = "2044 North Conker Ave", dest = { 95 }, coord = new(341.75f, 437.58f, 149.39f), h = 122.12f, veh = false },
            [95] = { text = "Exit House", dest = { 94 }, coord = new(346.83f, 440.71f, 147.70f), h = 301.65f, veh = false },
            [96] = { text = "2045 North Conker Ave", dest = { 97 }, coord = new(373.55f, 423.36f, 145.91f), h = 164.03f, veh = false },
            [97] = { text = "Exit House", dest = { 96 }, coord = new(373.55f, 427.94f, 145.68f), h = 74.67f, veh = false },
            [98] = { text = "3677 Whispymound Drive", dest = { 99 }, coord = new(117.26f, 559.51f, 184.30f), h = 184.50f, veh = false },
            [99] = { text = "Exit House", dest = { 98 }, coord = new(119.36f, 564.57f, 183.96f), h = 0.65f, veh = false },
            [100] = { text = "2862 Hillcrest Ave", dest = { 101 }, coord = new(-682.05f, 592.23f, 145.39f), h = 217.49f, veh = false },
            [101] = { text = "Exit House", dest = { 100 }, coord = new(-686.41f, 596.59f, 143.64f), h = 46.78f, veh = false },
            [102] = { text = "2868 Hillcrest Ave", dest = { 103 }, coord = new(-758.81f, 618.90f, 144.15f), h = 111.08f, veh = false },
            [103] = { text = "Exit House", dest = { 102 }, coord = new(-751.72f, 621.05f, 142.23f), h = 289.15f, veh = false },
            [104] = { text = "2874 Hillcrest Ave", dest = { 105 }, coord = new(-859.85f, 690.90f, 152.86f), h = 186.09f, veh = false },
            [105] = { text = "Exit House", dest = { 104 }, coord = new(-853.08f, 695.89f, 148.79f), h = 8.33f, veh = false },
            [106] = { text = "2113 Mad Wayne Thunder Drive", dest = { 107 }, coord = new(-1289.82f, 449.37f, 97.90f), h = 177.79f, veh = false },
            [107] = { text = "Exit House", dest = { 106 }, coord = new(-1294.33f, 454.89f, 97.47f), h = 5.19f, veh = false },
            [108] = { text = "1162 Power Street, Apt 3", dest = { 109 }, coord = new(346.50f, -1012.36f, -99.20f), h = 3.66f, veh = false },
            [109] = { text = "Exit Apartment", dest = { 108 }, coord = new(292.25f, -162.46f, 64.62f), h = 65.95f, veh = false },
            [110] = { text = "0112 South Rockford Drive, Apt 13", dest = { 111 }, coord = new(265.97f, -1003.11f, -99.01f), h = 21.68f, veh = false },
            [111] = { text = "Exit Apartment", dest = { 110 }, coord = new(-810.06f, -978.83f, 14.22f), h = 122.47f, veh = false },
            [112] = { text = "Sandy Shores Clubhouse", dest = { 113 }, coord = new(1121.05f, -3152.13f, -37.07f), h = 355.17f, veh = false },
            [113] = { text = "Exit Clubhouse", dest = { 112 }, coord = new(1737.78f, 3709.59f, 34.14f), h = 20.94f, veh = false },
            [114] = { text = "Sandy Shores Clubhouse Garage", dest = { 115 }, coord = new(1110.14f, -3164.25f, -37.52f), h = 356.00f },
            [115] = { text = "Exit Clubhouse", dest = { 114 }, coord = new(1725.67f, 3708.79f, 34.23f), h = 22.44f },
            [116] = { text = "Paleto Bay Clubhouse", dest = { 117 }, coord = new(997.25f, -3158.10f, -38.91f), h = 268.83f, veh = false },
            [117] = { text = "Exit Clubhouse", dest = { 116 }, coord = new(-38.47f, 6419.88f, 31.49f), h = 231.76f, veh = false },
            [118] = { text = "Paleto Bay Clubhouse Garage", dest = { 119 }, coord = new(998.82f, -3164.34f, -38.91f), h = 266.85f },
            [119] = { text = "Exit Clubhouse", dest = { 118 }, coord = new(-33.59f, 6422.45f, 31.43f), h = 221.21f },
            [120] = { text = "Cocaine Lockup", dest = { 121 }, coord = new(1088.66f, -3187.66f, -38.99f), h = 177.66f, veh = false },
            [121] = { text = "Exit Cocaine Lockup", dest = { 120 }, coord = new(51.92f, 6486.31f, 31.43f), h = 318.31f, veh = false },
            [122] = { text = "Cocaine Lockup", dest = { 123 }, coord = new(1103.32f, -3195.89f, -38.99f), h = 89.39f, veh = false },
            [123] = { text = "Exit Cocaine Lockup", dest = { 122 }, coord = new(56.73f, 6471.21f, 31.43f), h = 228.98f, veh = false },
            [124] = { text = "Counterfeit Cash Factory", dest = { 125 }, coord = new(1138.21f, -3198.80f, -39.67f), h = 357.07f, veh = false },
            [125] = { text = "Exit Counterfeit Cash Factory", dest = { 124 }, coord = new(-1170.99f, -1380.93f, 4.96f), h = 30.09f, veh = false },
            [126] = { text = "Counterfeit Cash Factory", dest = { 127 }, coord = new(1118.72f, -3193.27f, -40.40f), h = 177.34f, veh = false },
            [127] = { text = "Exit Counterfeit Cash Factory", dest = { 126 }, coord = new(-1168.95f, -1388.90f, 4.92f), h = 131.87f, veh = false },
            [128] = { text = "Document Forgery Office", dest = { 129 }, coord = new(1173.50f, -3196.66f, -39.01f), h = 88.37f, veh = false },
            [129] = { text = "Exit Document Forgery Office", dest = { 128 }, coord = new(1643.77f, 4857.89f, 42.01f), h = 96.51f, veh = false },
            [130] = { text = "Meth Lab", dest = { 131 }, coord = new(997.49f, -3200.70f, -36.40f), h = 274.34f, veh = false },

            [131] = { text = "Exit Meth Lab", dest = { 130 }, coord = new(1180.88f, -3113.84f, 6.03f), h = 99.59f, veh = false },
            [132] = { text = "Weed Farm", dest = { 133 }, coord = new(1066.01f, -3183.38f, -39.16f), h = 93.01f, veh = false },
            [133] = { text = "Exit Weed Farm", dest = { 132 }, coord = new(102.07f, 175.09f, 104.59f), h = 165.63f, veh = false },
            [134] = { text = "Vehicle Warehouse", dest = { 135 }, coord = new(970.83f, -2990.88f, -39.65f), h = 179.69f },
            [135] = { text = "Exit Vehicle Warehouse", dest = { 134 }, coord = new(-666.58f, -2379.13f, 13.87f), h = 61.39f },
            [136] = { text = "Vehicle Warehouse Modshop", dest = { 137 }, coord = new(954.89f, -2991.19f, -39.65f), h = 180.23f },
            [137] = { text = "Exit Vehicle Warehouse", dest = { 136 }, coord = new(-673.56f, -2391.19f, 13.87f), h = 60.95f },
            [138] = { text = "Vehicle Warehouse Basement", dest = { 139 }, coord = new(946.57f, -2999.03f, -47.65f), h = 269.91f },
            [139] = { text = "Vehicle Warehouse", dest = { 138 }, coord = new(978.25f, -3002.00f, -39.65f), h = 90.23f },
            [140] = { text = "Large Logistics Depot", dest = { 141 }, coord = new(992.82f, -3097.80f, -39.00f), h = 269.20f, veh = false },
            [141] = { text = "Exit Large Logistics Depot", dest = { 140 }, coord = new(926.66f, -1560.23f, 30.74f), h = 92.72f, veh = false },
            [142] = { text = "Disused Factory Outlet", dest = { 143 }, coord = new(1048.57f, -3097.13f, -39.00f), h = 274.83f, veh = false },
            [143] = { text = "Exit Disused Factory Outlet", dest = { 142 }, coord = new(-324.90f, -1356.23f, 31.30f), h = 90.77f, veh = false },
            [144] = { text = "Pier 400 Utility Building", dest = { 145 }, coord = new(1087.91f, -3099.38f, -39.00f), h = 277.33f, veh = false },
            [145] = { text = "Exit Pier 400 Utility Building", dest = { 144 }, coord = new(274.54f, -3015.40f, 5.70f), h = 94.82f, veh = false },
            [146] = { text = "LSIA Hangar 1", dest = { 147 }, coord = new(-1267.02f, -2982.38f, -48.49f), h = 179.76f },
            [147] = { text = "Exit LSIA Hangar 1", dest = { 146 }, coord = new(-1139.08f, -3387.34f, 13.94f), h = 328.99f },
            [148] = { text = "Farmhouse Bunker", dest = { 149 }, coord = new(890.55f, -3245.86f, -98.27f), h = 91.61f },
            [149] = { text = "Exit Farmhouse Bunker", dest = { 148 }, coord = new(1571.97f, 2234.43f, 79.06f), h = 182.06f },
            [150] = { text = "Paleto Bay Facility", dest = { 151 }, coord = new(482.67f, 4812.92f, -58.38f), h = 13.05f },
            [151] = { text = "Exit Paleto Bay Facility", dest = { 150 }, coord = new(1.79f, 6832.14f, 15.82f), h = 248.36f },
            [152] = { text = "Richards Majestic Helipad", dest = { 50, 51, 52, 53 }, coord = new(-903.17f, -369.94f, 136.28f), h = 116.96f, veh = false },
            [153] = { text = "Nightclub Office", dest = { 154 }, coord = new(-1618.43f, -3007.99f, -75.20f), h = 174.46f, veh = false },
            [154] = { text = "Nightclub Basement", dest = { 153 }, coord = new(-1507.66f, -3024.46f, -79.24f), h = 177.89f, veh = false },
            [155] = { text = "Del Perro Heights Garage", dest = { 54, 55, 56, 57 }, coord = new(-1456.37f, -514.44f, 31.58f), h = 211.62f, veh = false },
            [158] = { text = "Exit Arcade", coord = new(2737.96f, -374.12f, -47.99f), h = 174.4f, dest = { 159 } },
            [159] = { text = "Enter Arcade", coord = new(758.76f, -816.06f, 26.29f), h = 278.34f, dest = { 158 } },
            [160] = { text = "Exit Split Sides Comedy Store", dest = { 161 }, coord = new(-430.06f, 261.72f, 83.0f), h = 170.49f, veh = false },
            [161] = { text = "Split Sides Comedy Store", dest = { 160 }, coord = new(-458.85f, 284.66f, 78.5f), h = 266.75f, veh = false },
            [164] = { text = "Morgue", dest = { 165 }, coord = new(275.44f, -1361.26f, 24.3f), h = 48.5f, veh = false },
            [165] = { text = "Exit Morgue", dest = { 164 }, coord = new(240.74f, -1379.18f, 33.55f), h = 147.44f, veh = false },
            [168] = { text = "Exit Movie Theatre", dest = { 169 }, coord = new(-1423.6f, -215.54f, 46.2f), h = 360f, veh = false },
            [169] = { text = "Movie Theatre", dest = { 168 }, coord = new(-1436.89f, -257.8f, 16.09f), h = 359.81f, veh = false },
            [172] = { text = "Exit Submarine", dest = { 173 }, coord = new(493.69f, -3222.95f, 10.5f), h = 179.25f, veh = false },
            [173] = { text = "Submarine", dest = { 172 }, coord = new(514.25f, 4888.15f, -62.6f), h = 176.33f, veh = false },
            [174] = { text = "Exit Server Farm", dest = { 175 }, coord = new(2476.11f, -384.15f, 94.2f), h = 268.8f, veh = false },
            [175] = { text = "Server Farm", dest = { 174 }, coord = new(2154.87f, 2921.0f, -81.26f), h = 270.67f, veh = false },
            [176] = { text = "Exit IAA Facility", dest = { 177 }, coord = new(2049.84f, 2949.75f, 47.55f), h = 256.84f, veh = false },
            [177] = { text = "IAA Facility", dest = { 176 }, coord = new(2155.05f, 2921.00f, -62.09f), h = 91.39f, veh = false },
            [178] = { text = "Doomsday Facility", coord = new(1255.97f, 4796.37f, -39.24f), h = 346.87f, dest = { 179 } },
            [179] = { text = "Exit Doomsday Facility", coord = new(-356.04f, 4823.27f, 142.74f), h = 138.58f, dest = { 178 } },
            [180] = { text = "Missile Silo", coord = new(369.49f, 6319.51f, -160.12f), h = 200.0f, dest = { 181 }, veh = false },
            [181] = { text = "Exit Missile Silo", coord = new(1259.31f, 4799.19f, -39.5f), h = 78.22f, dest = { 180 }, veh = false },
            [197] = { text = "Rooftop", coord = new(964.83f, 58.48f, 112.37f), h = 64.13f, dest = { 245, 198, 199, 200, 202, 203, 283, 201 }, veh = false },
            [198] = { text = "Hotel", coord = new(2518.76f, -262.10f, -39.13f), h = 4.95f, dest = { 245, 197, 199, 200, 202, 203, 283, 201 }, veh = false },
            [199] = { text = "Offices", coord = new(2517.65f, -263.4f, -55.12f), h = 10.29f, dest = { 245, 197, 198, 200, 201, 202, 203, 283 }, veh = false },
            [200] = { text = "Ground Floor", coord = new(2463.24f, -281.05f, -58.48f), h = 315.36f, dest = { 245, 197, 198, 199, 202, 203, 283, 201 }, veh = false },
            [201] = { text = "Casino Main Entrance", coord = new(935.8f, 46.88f, 81.1f), h = 133.58f, dest = { 245, 197, 198, 199, 200, 202, 203, 283 }, veh = false },
            [202] = { text = "Loading Bay", coord = new(2518.55f, -279.1f, -64.72f), h = 268.34f, dest = { 245, 197, 198, 199, 200, 203, 283, 201 }, veh = false },
            [203] = { text = "Vault", coord = new(2519.03f, -279.21f, -70.71f), h = 271.9f, dest = { 245, 197, 198, 199, 200, 202, 201, 283 }, veh = false },
            [204] = { text = "Exit Casino Loading Bay", coord = new(999.05f, -53.19f, 74.95f), h = 211.72f, dest = { 205 } },
            [205] = { text = "Enter Casino Loading Bay", coord = new(2654.94f, -343.43f, -64.72f), h = 58.87f, dest = { 204 } },
            [208] = { text = "Leave Music Locker", coord = new(1578.25f, 253.86f, -46.01f), h = 181.46f, dest = { 209 }, veh = false },
            [209] = { text = "Enter Music Locker", coord = new(987.57f, 79.82f, 80.99f), h = 330.39f, dest = { 208 }, veh = false },
            [210] = { text = "Leave Tuners Car Garage", dest = { 211 }, coord = new(-1357.85f, 165.96f, -99.03f), h = 184.89f },
            [211] = { text = "Enter Tuners Car Garage", dest = { 210 }, coord = new(798.51f, -962.21f, 25.97f), h = 92.76f },
            [212] = { text = "Leave Tuners Car Meet", dest = { 213 }, coord = new(-2220.81f, 1157.72f, -23.26f), h = 182.38f },
            [213] = { text = "Enter Tuners Car Meet", dest = { 212 }, coord = new(782.47f, -1868.65f, 29.25f), h = 266.53f },
            [214] = { text = "Leave Rockford Hills Agency", dest = { 215, 226, 227 }, coord = new(-1016.51f, -413.24f, 39.62f), h = 22.26f, veh = false },
            [215] = { text = "Enter Rockford Hills Agency", dest = { 226, 227, 214 }, coord = new(-1033.35f, -434.92f, 63.86f), h = 296.08f, veh = false },
            [216] = { text = "Leave Vespucci Agency", dest = { 217, 228, 229 }, coord = new(-1011.84f, -734.16f, 21.53f), h = 37.45f, veh = false },
            [217] = { text = "Enter Vespucci Agency", dest = { 228, 229, 216 }, coord = new(-1002.99f, -774.63f, 61.89f), h = 1.79f, veh = false },
            [218] = { text = "Leave Hawick Agency", dest = { 219, 230, 231 }, coord = new(389.92f, -76.12f, 68.18f), h = 159.33f, veh = false },
            [219] = { text = "Enter Hawick Agency", dest = { 230, 231, 218 }, coord = new(370.34f, -56.36f, 103.36f), h = 253.34f, veh = false },
            [223] = { text = "Leave Studio", dest = { 224, 225 }, coord = new(-841.6f, -229.09f, 37.26f), h = 3002.62f, veh = false },
            [224] = { text = "Enter Studio", dest = { 223, 225 }, coord = new(-1021.83f, -92.39f, -99.4f), h = 2.11f, veh = false },
            [225] = { text = "Studio Rooftop", dest = { 223, 224 }, coord = new(-843.28f, -236.25f, 61.02f), h = 44.71f, veh = false },
            [226] = { text = "2nd Floor", dest = { 227, 215, 214 }, coord = new(-1033.16f, -435.54f, 72.46f), h = 300.6f, veh = false },
            [227] = { text = "Rooftop", dest = { 226, 215, 214 }, coord = new(-1023.5f, -432.78f, 77.37f), h = 117.41f, veh = false },
            [228] = { text = "2nd Floor", dest = { 229, 217, 216 }, coord = new(-1002.53f, -774.67f, 70.49f), h = 359.76f, veh = false },
            [229] = { text = "Rooftop", dest = { 228, 217, 216 }, coord = new(-1001.03f, -752.06f, 76.54f), h = 273.25f, veh = false },
            [230] = { text = "2nd Floor", dest = { 231, 219, 218 }, coord = new(370.15f, -56.96f, 111.96f), h = 251.8f, veh = false },
            [231] = { text = "Rooftop", dest = { 230, 219, 218 }, coord = new(383.05f, -51.54f, 122.54f), h = 202.94f, veh = false },
            [232] = { text = "Leave via Rooftop", dest = { 233 }, coord = new(966.71f, 65.62f, 112.55f), h = 117.99f, veh = false },
            [233] = { text = "Enter Penthouse", dest = { 232 }, coord = new(969.82f, 63.1f, 112.56f), h = 244.2f, veh = false },
            [234] = { text = "Basement", dest = { 235 }, coord = new(5012.48f, -5748.91f, 28.95f), h = 142.41f, veh = false },
            [235] = { text = "Office", dest = { 234 }, coord = new(5013.69f, -5744.81f, 15.48f), h = 147.46f, veh = false },
            [236] = { text = "Enter Therapist Office", dest = { 238, 237 }, coord = new(-1904.17f, -568.86f, 19.1f), h = 227.34f, veh = false },
            [237] = { text = "Leave Therapist Office", dest = { 238, 236 }, coord = new(-1898.53f, -572.49f, 11.85f), h = 187.6f, veh = false },
            [238] = { text = "Rooftop", dest = { 236, 237 }, coord = new(-1908.39f, -570.94f, 22.97f), h = 137.9f, veh = false },
            [239] = { text = "Level -3", dest = { 240 }, coord = new(3540.65f, 3675.46f, 20.99f), h = 171.02f, veh = false },
            [240] = { text = "Level -1", dest = { 239 }, coord = new(3540.65f, 3675.46f, 28.12f), h = 166.91f, veh = false },
            [241] = { text = "Level 1", dest = { 242 }, coord = new(136.13f, -761.63f, 45.75f), h = 168.15f, veh = false },
            [242] = { text = "Level 49", dest = { 241 }, coord = new(136.14f, -761.64f, 242.15f), h = 155.18f, veh = false },
            [243] = { text = "Leave Solomon Office", dest = { 244 }, coord = new(-1003.16f, -477.9f, 50.03f), h = 98.47f, veh = false },
            [244] = { text = "Enter Solomon", dest = { 243 }, coord = new(-1011.41f, -479.98f, 39.97f), h = 120.88f, veh = false },
            [245] = { text = "Helipad", dest = { 197, 198, 199, 200, 201, 202, 203, 283 }, coord = new(971.89f, 51.91f, 120.24f), h = 328.03f, veh = false },
            [246] = { text = "Rooftop", dest = { 247 }, coord = new(743.43f, -1797.16f, 29.29f), h = 82.66f, veh = false },
            [247] = { text = "Rooftop", dest = { 246 }, coord = new(748.87f, -1792.96f, 49.31f), h = 355.72f, veh = false },
            [248] = { text = "Union Depository", dest = { 249 }, coord = new(-0.05f, -705.85f, 16.13f), h = 336.49f, veh = false },
            [249] = { text = "Union Depository", dest = { 248 }, coord = new(10.5f, -671.31f, 33.45f), h = 7.11f, veh = false },
            [250] = { text = "Exit Garage", dest = { 251 }, coord = new(2680.64f, -361.38f, -55.19f), h = 267.19f, veh = true },
            [251] = { text = "Enter Garage", dest = { 250 }, coord = new(723.93f, -822.25f, 24.75f), h = 181.28f, veh = true },
            [252] = { text = "Exit Hole", dest = { 253 }, coord = new(2480.05f, -294.69f, -70.64f), h = 232.25f, veh = false },
            [253] = { text = "Enter Hole", dest = { 252 }, coord = new(2480.4f, -288.36f, -70.7f), h = 237.6f, veh = false },
            [254] = { text = "Exit Tunnel", dest = { 255 }, coord = new(2517.14f, -327.1f, -70.65f), h = 85.02f, veh = false },
            [255] = { text = "Enter Tunnel", dest = { 254 }, coord = new(873.29f, -228.46f, 18.33f), h = 265.27f, veh = false },
            [256] = { text = "Exit Vehicle Warehouse", dest = { 257 }, coord = new(265.71f, -1159.67f, 29.25f), h = 86.17f, veh = true },
            [257] = { text = "Enter Vehicle Warehouse", dest = { 256 }, coord = new(813.92f, -3001.28f, -69.0f), h = 84.84f, veh = true },
            [258] = { text = "Exit Vehicle Warehouse", dest = { 259 }, coord = new(286.97f, -1148.59f, 29.29f), h = 354.52f, veh = false },
            [259] = { text = "Enter Vehicle Warehouse", dest = { 258 }, coord = new(782.39f, -2997.9f, -69.0f), h = 269.64f, veh = false },
            [260] = { text = "Exit Farmhouse", dest = { 261 }, coord = new(1929.95f, 4634.96f, 40.47f), h = 359.21f, veh = false },
            [261] = { text = "Enter Farmhouse", dest = { 260 }, coord = new(844.68f, -3004.97f, -44.4f), h = 91.45f, veh = false },
            [262] = { text = "Exit RV", dest = { 263 }, coord = new(2318.96f, 2553.64f, 47.69f), h = 221.7f, veh = false },
            [263] = { text = "Enter RV", dest = { 262 }, coord = new(482.35f, -2623.84f, -49.06f), h = 180.13f, veh = false },
            [264] = { text = "Exit Freakshop", dest = { 265 }, coord = new(574.46f, -422.73f, -69.65f), h = 91.33f, veh = true },
            [265] = { text = "Enter Freakshop", dest = { 264 }, coord = new(599.00f, -426.12f, 24.74f), h = 266.14f, veh = true },
            [266] = { text = "Exit Morgue", dest = { 267 }, coord = new(232.15f, -1360.84f, 28.65f), h = 51.9f, veh = false },
            [267] = { text = "Enter Morgue", dest = { 266 }, coord = new(486.69f, -2573.35f, -66.6f), h = 1.75f, veh = false },
            [268] = { text = "Leave", dest = { 269, 272 }, coord = new(-277.71f, 282.77f, 89.89f), h = 181.52f, veh = false },
            [269] = { text = "Go To Rooftop", dest = { 272, 268 }, coord = new(-271.8f, 289.57f, 104.99f), h = 183.29f, veh = false },
            [270] = { text = "Enter Garage", dest = { 271 }, coord = new(519.88f, -2637.43f, -49.0f), h = 356.62f, veh = true },
            [271] = { text = "Leave Garage", dest = { 270 }, coord = new(-270.71f, 280.69f, 90.2f), h = 187.77f, veh = true },
            [272] = { text = "Enter Garage On Foot", dest = { 269, 268 }, coord = new(531.72f, -2637.62f, -49.0f), h = 89.72f, veh = false },
            [273] = { text = "Enter Secret Facility", dest = { 274 }, coord = new(-1922.49f, 3749.84f, -99.65f), h = 271.74f, veh = false },
            [274] = { text = "Leave Secret Facility", dest = { 273 }, coord = new(-2052.06f, 3237.58f, 31.5f), h = 56.98f, veh = false },
            [275] = { text = "Enter Vinewood Car club", dest = { 276 }, coord = new(1233.45f, -3230.44f, 5.69f), h = 358.82f, veh = true },
            [276] = { text = "Leave Vinewood Car club", dest = { 275 }, coord = new(1205.75f, -3253.51f, -48.99f), h = 92.05f, veh = true },
            [277] = { text = "Enter Vinewood Car club", dest = { 278 }, coord = new(1196.8f, -3253.66f, 7.1f), h = 93.16f, veh = false },
            [278] = { text = "Leave Vinewood Car club", dest = { 277 }, coord = new(1180.82f, -3260.52f, -48.0f), h = 269.77f, veh = false },
            [279] = { text = "Enter Motel", dest = { 280 }, coord = new(151.44f, -1007.8f, -99.0f), h = 357.17f, veh = false },
            [280] = { text = "Leave Motel", dest = { 279 }, coord = new(1121.37f, 2642.07f, 38.14f), h = 359.26f, veh = false },
            [281] = { text = "Enter Garage", dest = { 282 }, coord = new(934.11f, -2.45f, 78.76f), h = 150.13f, veh = true },
            [282] = { text = "Leave Garage", dest = { 281 }, coord = new(1340.99f, 183.77f, -47.97f), h = 261.53f, veh = true },
            [283] = { text = "Casino Garage", dest = { 245, 197, 198, 199, 200, 202, 201 }, coord = new(1380.28f, 178.28f, -48.99f), h = 3.74f, veh = false },
            [284] = { text = "Enter Yacht", dest = { 285 }, coord = new(3602.13f, -4781.0f, 5.89f), h = 280.01f, veh = false },
            [285] = { text = "Leave Yacht", dest = { 284 }, coord = new(3577.75f, -4781.08f, 5.89f), h = 79.55f, veh = false },
            [286] = { text = "Enter Captain's Room", dest = { 287 }, coord = new(3614.43f, -4781.14f, 11.91f), h = 80.03f, veh = false },
            [287] = { text = "Exit", dest = { 286 }, coord = new(3620.65f, -4781.14f, 11.92f), h = 267.34f, veh = false },
        };
        #endregion

        #region Constructor
        public ClientMain()
        {
            for (int i = 0; i < blipList.Count; i++)
            {
                int blip = AddBlipForCoord(blips.InteriorCoords.X, blips.InteriorCoords.Y, blips.InteriorCoords.Z);

                SetBlipAsShortRange(blip, true);
                SetBlipSprite(blip, (int)blips.BlipSprite);
                SetBlipColour(blip, (int)blips.BlipColor);
                SetBlipDisplay(blip, 4);
                SetBlipScale(blip, 0.9f);
                BeginTextCommandSetBlipName("STRING");
                AddTextComponentString(blips.Name);
                EndTextCommandSetBlipName(blip);
            }
        }
        #endregion

        #region Methods
        private void SetVideoWallTvChannel()
        {
            SetTvChannelPlaylist(0, "CASINO_DIA_PL", true);
            SetTvAudioFrontend(true);
            SetTvVolume(-100.0f);
            SetTvChannel(0);
        }
        #endregion

        #region Event Handlers
        [EventHandler("OpenInteriors:Client:enterCasino")]
        private void OnEnterCasino()
        {
            inCasino = true;
            Tick += CasinoTick;
        }

        [EventHandler("OpenInteriors:Client:exitCasino")]
        private void OnExitCasino()
        {
            inCasino = false;
        }

        [EventHandler("onResourceStop")]
        private void OnResourceStop(string resource)
        {
            if (resource == GetCurrentResourceName())
            {
                ReleaseNamedRendertarget(resource);
            }
        }
        #endregion

        #region Ticks
        private async Task TeleportPlayer(int teleport)
        {
            await Delay(0);

            var entity = Game.Player.Character.Handle;

            foreach (var pair in teleports)
            {
                if (pair.Key == teleport)
                {
                    if (CurrentVehicle.Exists())
                    {
                        entity = CurrentVehicle.Handle;

                        if (!pair.Value.veh)
                        {
                            AddChatMessage("SYSTEM", "Vehicles are not allowed.", 255, 0, 0);
                            return;
                        }
                    }
                }

                DoScreenFadeOut(500);
                await Delay(500);

                NetworkFadeOutEntity(entity, false, true);
                await Delay(500);

                SetEntityCoordsNoOffset(entity, pair.Value.coord.X, pair.Value.coord.Y, pair.Value.coord.Z, false, false, false);
                SetGameplayCamRelativeHeading(pair.Value.h);
                SetGameplayCamRelativePitch(-20.0f, 1.0f);
                SetEntityHeading(entity, pair.Value.h);

                await Delay(500);
                NetworkFadeInEntity(entity, true);

                await Delay(500);
                DoScreenFadeIn(500);

                if (pair.Key == 200)
                {
                    TriggerEvent("OpenInteriors:Client:enterCasino");
                }

                break;
            }
        }

        private async Task CasinoTick()
        {
            var interior = GetInteriorAtCoords(PlayerPed.Position.X, PlayerPed.Position.Y, PlayerPed.Position.Z);
            int lastUpdatedTvChannel = 0;

            PinInteriorInMemory(interior);

            while (!IsInteriorReady(interior))
            {
                Wait(10);
                RequestStreamedTextureDict("Prop_Screen_Vinewood", false);
            }

            while (!HasStreamedTextureDictLoaded("Prop_Screen_Vinewood"))
            {
                Wait(100);
            }

            RegisterNamedRendertarget("casinoscreen_01", false);
            LinkNamedRendertarget((uint)GetHashKey("vw_vwint01_video_overlay"));

            videoWallRenderTarget = GetNamedRendertargetRenderId("casinoscreen_01");

            while (true)
            {
                Wait(0);

                if (!inCasino)
                {
                    ReleaseNamedRendertarget("casinoscreen_01");

                    videoWallRenderTarget = null;
                    showBigWin = false;

                    break;
                }

                if (videoWallRenderTarget != null)
                {
                    int currentTime = GetGameTimer();

                    if (currentTime - lastUpdatedTvChannel >= 42666)
                    {
                        SetVideoWallTvChannel();
                        lastUpdatedTvChannel = currentTime;
                    }

                    SetTextRenderId((int)videoWallRenderTarget);
                    SetScriptGfxDrawOrder(4);
                    SetScriptGfxDrawBehindPausemenu(true);
                    DrawInteractiveSprite("Prop_Screen_Vinewood", "BG_Wall_Colour_4x4", 0.25f, 0.5f, 0.5f, 1.0f, 0.0f, 255, 255, 255, 255);
                    DrawTvChannel(0.5f, 0.5f, 1.0f, 1.0f, 0.0f, 255, 255, 255, 255);
                    SetTextRenderId(GetDefaultScriptRendertargetRenderId());
                }
            }
        }
        #endregion
    }
}