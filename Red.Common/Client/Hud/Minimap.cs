using System;
using static CitizenFX.Core.Native.API;

namespace Red.Common.Client.Hud
{
    internal class Minimap
    {
        public float Width { get; set; }
        public float Height { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float LeftX { get; set; }
        public float RightX { get; set; }
        public float TopY { get; set; }
        public float BottomY { get; set; }
        public float XUnit { get; set; }
        public float YUnit { get; set; }
        /// <summary>
        /// Gets the minimap and anchors it, helps with text position without
        /// it moving (adjusts based off of SafeZone sizes).
        /// 
        /// Minimap Anchor made by https://github.com/glitchdetector/fivem-minimap-anchor/blob/master/MINIANCHOR.lua
        /// 16:9 Modifications give to by traditionalism https://github.com/traditionalism
        /// </summary>
        /// <returns></returns>
        public static Minimap GetMinimapAnchor()
        {
            // Calculate safezone (grabs user safezone from game)
            float safeZoneSize = GetSafeZoneSize();

            // Get aspect ratio (wide monitors might affect UI layout)
            float aspectRatio = GetAspectRatio(false);
            
            // constant for scaling map based off of safezone size and aspect ratio
            float factor1 = 0.05f;
            float factor2 = 0.05f;

            // Default 1920x1080 resolution (updated later)
            int resX = 1920;
            int resY = 1080;

            // Cap aspect ratio to a 16:9 for baseline calculations in the future.
            if ((double)aspectRatio > 2.0)
            {
                aspectRatio = 1.77777779f; // 16:9 aspect ratio. Could this be shortend to 1.8?
            }

            // Get actual screen resolution for dynamic scaling.
            GetActiveScreenResolution(ref resX, ref resY);

            // Calculate unit for dimension scaling based off of screen size.
            float unitX = 1f / resX;
            float unitY = 1f / resY;

            // Create an anchor object and set the initial width based off of aspect ratio.
            Minimap minimap = new Minimap()
            {
                Width = unitX * (resX / (4f * aspectRatio)),
                Height = unitY * (resY / 5.674f), // Set fixed height for the anchor (can be adjusted as needed)
                LeftX = unitX * (resX * (factor1 * (Math.Abs(safeZoneSize - 1f) * 10f))) // Calculate initial left position based on safe zone size (adjustment factor can be modified)
            };

            // Adjust minimap position and width for ultrawide or wide aspect ratios
            if ((double)aspectRatio > 2.0)
            {
                minimap.LeftX += minimap.Width * 0.845f;
                minimap.Width *= 0.76f;
            }
            else if ((double)aspectRatio > 1.7999999523162842)
            {
                minimap.LeftX += minimap.Width * 0.2225f;
                minimap.Width *= 0.995f;
            }

            // Calculate remaining minimap coordinates based on screen size and safe zone
            minimap.BottomY = (float)(1.0f - (double)unitX * (resY * ((double)factor2 * ((double)Math.Abs(safeZoneSize - 1f) * 10.0))));
            minimap.RightX = minimap.LeftX + minimap.Width;
            minimap.TopY = minimap.BottomY - minimap.Height;
            minimap.X = minimap.LeftX;
            minimap.Y = minimap.TopY;
            minimap.XUnit = unitX;
            minimap.YUnit = unitY;

            return minimap;
        }
    }
}
