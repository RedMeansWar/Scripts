using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.Common.Client.Misc
{
    public class Graphic : BaseScript
    {
        public static void Draw3dLine(float pos1X, float pos1Y, float pos1Z, float pos2X, float pos2Y, float pos2Z, int r, int g, int b) => DrawLine(pos1X, pos1Y, pos1Z, pos2X, pos2Y, pos2Z, r, g, b, 255);
        public static void Draw3dLine(float pos1X, float pos1Y, float pos1Z, float pos2X, float pos2Y, float pos2Z, int r, int g, int b, int a) => DrawLine(pos1X, pos1Y, pos1Z, pos2X, pos2Y, pos2Z, r, g, b, a);
        public static void Draw3dLine(Vector3 pos1, Vector3 pos2, int r, int g, int b) => DrawLine(pos1.X, pos1.Y, pos1.Z, pos2.X, pos2.Y, pos2.Z, r, g, b, 255);
        public static void Draw3dLine(Vector3 pos1, Vector3 pos2, int r, int g, int b, int a) => DrawLine(pos1.X, pos1.Y, pos1.Z, pos2.X, pos2.Y, pos2.Z, r, g, b, a);

        public static void Draw2dDebugText(string text, float x, float y, float z, int r, int g, int b) => DrawDebugText_2d(text, x, y, z, r, g, b, 255);
        public static void Draw2dDebugText(string text, float x, float y, float z, int r, int g, int b, int a) => DrawDebugText_2d(text, x, y, z, r, g, b, a);
        public static void Draw2dDebugText(string text, Vector3 position, int r, int g, int b, int a) => DrawDebugText_2d(text, position.X, position.Y, position.Z, r, g, b, a);

        public static void DrawPolygram(float pos1X, float pos1Y, float pos1Z, float pos2X, float pos2Y, float pos2Z, float pos3X, float pos3Y, float pos3Z, int r, int g, int b) => DrawPoly(pos1X, pos1Y, pos1Z, pos2X, pos2Y, pos2Z, pos3X, pos3Y, pos3Z, r, g, b, 255);
        public static void DrawPolygram(float pos1X, float pos1Y, float pos1Z, float pos2X, float pos2Y, float pos2Z, float pos3X, float pos3Y, float pos3Z, int r, int g, int b, int a) => DrawPoly(pos1X, pos1Y, pos1Z, pos2X, pos2Y, pos2Z, pos3X, pos3Y, pos3Z, r, g, b, a);
        public static void DrawPolygram(Vector3 position1, Vector3 position2, Vector3 position3, int r, int g, int b) => DrawPoly(position1.X, position1.Y, position1.Z, position2.X, position2.Y, position2.Z, position3.X, position3.Y, position3.Z, r, g, b, 255); 
        public static void DrawPolygram(Vector3 position1, Vector3 position2, Vector3 position3, int r, int g, int b, int a) => DrawPoly(position1.X, position1.Y, position1.Z, position2.X, position2.Y, position2.Z, position3.X, position3.Y, position3.Z, r, g, b, a);

        public static void DrawCircle(float x, float y, float z, float radius, int r, int g, int b, float opacity) => DrawSphere(x, y, z, radius, r, g, b, opacity);
        public static void DrawCircle(Vector3 position, float radius, int r, int g, int b, float opacity) => DrawSphere(position.X, position.Y, position.Z, radius, r, g, b, opacity);
        public static void DrawCircle(float x, float y, float z, float radius, int r, int g, int b, int a) => DrawMarker(28, x, y, z, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, radius, radius, radius, r, g, b, a, false, false, 2, false, null, null, false);

        public static async void RequestTextureDict(string textureDictionary)
        {
            RequestStreamedTextureDict(textureDictionary, true);

            while (!HasStreamedTextureDictLoaded(textureDictionary))
            {
                await Delay(0);
            }
        }

        public static void RemovePTFX(CoordinateParticleEffect particalEffect)
        {
            StopParticleFxLooped(particalEffect.Handle, false);
            RemoveParticleFx(particalEffect.Handle, true);
        }

        public static void RemoveParticle(CoordinateParticleEffect particalEffect) => RemovePTFX(particalEffect);

        public static async void RequestParticles()
        {
            RequestNamedPtfxAsset("core");
            while(!HasNamedPtfxAssetLoaded("core"))
            {
                await Delay(0);
            }
        }
    }
}
