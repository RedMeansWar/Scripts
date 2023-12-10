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
        /// it moving (adjusts based off of SafeZone sizes.
        /// 
        /// Minimap Anchor made by https://github.com/glitchdetector/fivem-minimap-anchor/blob/master/MINIANCHOR.lua
        /// 16:9 Modifications give to by traditionalism https://github.com/traditionalism
        /// </summary>
        /// <returns></returns>
        public static Minimap GetMinimapAnchor()
        {
            // 0.05 * ((safezone - 0.9) * 10)
            float safeZoneSize = GetSafeZoneSize();
            float aspectRatio = GetAspectRatio(false);

            float factor1 = 0.05f;
            float factor2 = 0.05f;

            int resX = 1920;
            int resY = 1080;

            if ((double)aspectRatio > 2.0)
            {
                aspectRatio = 1.77777779f; // aspect ratio of 16:9 (assuming that the screen ratio is 16:9 (1920x1080)
            }

            GetActiveScreenResolution(ref resX, ref resY);

            float unitX = 1f / resX;
            float unitY = 1f / resY;

            Minimap minimap = new Minimap()
            {
                Width = unitX * (resX / (4f * aspectRatio)),
                Height = unitY * (resY / 5.674f),
                LeftX = unitX * (resX * (factor1 * (Math.Abs(safeZoneSize - 1f) * 10f)))
            };
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
