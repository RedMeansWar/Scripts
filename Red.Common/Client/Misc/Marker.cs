using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.Common.Client.Misc
{
    public class Marker
    {
        /// <summary>
        /// Places a marker on the ground properly
        /// </summary>
        /// <param name="markerType"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <param name="dirX"></param>
        /// <param name="dirY"></param>
        /// <param name="dirZ"></param>
        /// <param name="rotationX"></param>
        /// <param name="rotationY"></param>
        /// <param name="rotationZ"></param>
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>
        /// <param name="scaleZ"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <param name="bobUpAndDown"></param>
        /// <param name="faceCamera"></param>
        /// <param name="p19"></param>
        /// <param name="rotate"></param>
        /// <param name="textureDict"></param>
        /// <param name="textureName"></param>
        /// <param name="drawOnEnts"></param>
        public static void PlaceMarkerOnGroundProperly(int markerType, float posX, float posY, float posZ, float dirX, float dirY, float dirZ, float rotationX, float rotationY, float rotationZ, float scaleX, float scaleY, float scaleZ, int r, int g, int b, int a, bool bobUpAndDown, bool faceCamera, int p19, bool rotate, string textureDict, string textureName, bool drawOnEnts)
        {
            p19 = 2;
            rotationX = 0f;
            rotationY = 0f;
            rotationZ = 0f;

            DrawMarker(markerType, posX, posY, posZ - 1f, dirX, dirY, dirZ, rotationX, rotationY, rotationZ, scaleX, scaleY, scaleZ, r, g, b, a, bobUpAndDown, faceCamera, p19, rotate, textureDict, textureName, drawOnEnts);
        }
        /// <summary>
        /// Places a marker on the ground properly
        /// </summary>
        /// <param name="markerType"></param>
        /// <param name="markerPos"></param>
        /// <param name="markerDir"></param>
        /// <param name="markerRot"></param>
        /// <param name="markerScale"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <param name="bobUpAndDown"></param>
        /// <param name="faceCamera"></param>
        /// <param name="p19"></param>
        /// <param name="rotate"></param>
        /// <param name="textureDict"></param>
        /// <param name="textureName"></param>
        /// <param name="drawOnEnts"></param>
        public static void PlaceMarkerOnGroundProperly(int markerType, Vector3 markerPos, Vector3 markerDir, Vector3 markerRot, Vector3 markerScale, int r, int g, int b, int a, bool bobUpAndDown, bool faceCamera, int p19, bool rotate, string textureDict, string textureName, bool drawOnEnts)
        {
            p19 = 2;
            markerRot.X = 0f;
            markerRot.Y = 0f;
            markerRot.Z = 0f;

            DrawMarker(markerType, markerPos.X, markerPos.Y, markerPos.Z - 1f, markerDir.X, markerDir.Y, markerDir.Z, markerRot.X, markerRot.Y, markerRot.Z, markerScale.X, markerScale.Y, markerScale.Z, r, g, b, a, bobUpAndDown, faceCamera, p19, rotate, textureDict, textureName, drawOnEnts);
        }
        /// <summary>
        /// Places a marker on the ground properly
        /// </summary>
        /// <param name="markerType"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <param name="markerDir"></param>
        /// <param name="markerRot"></param>
        /// <param name="markerScale"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <param name="bobUpAndDown"></param>
        /// <param name="faceCamera"></param>
        /// <param name="rotate"></param>
        /// <param name="textureDict"></param>
        /// <param name="textureName"></param>
        /// <param name="drawOnEnts"></param>
        public static void PlaceMarkerOnGroundProperly(int markerType, float posX, float posY, float posZ, Vector3 markerDir, Vector3 markerRot, Vector3 markerScale, int r, int g, int b, int a, bool bobUpAndDown, bool faceCamera, bool rotate, string textureDict, string textureName, bool drawOnEnts)
        {
            markerRot.X = 0f;
            markerRot.Y = 0f;
            markerRot.Z = 0f;

            DrawMarker(markerType, posX, posY, posZ - 1f, markerDir.X, markerDir.Y, markerDir.Z, markerRot.X, markerRot.Y, markerRot.Z, markerScale.X, markerScale.Y, markerScale.Z, r, g, b, a, bobUpAndDown, faceCamera, 2, rotate, textureDict, textureName, drawOnEnts);
        }
        /// <summary>
        /// Places a marker on the ground properly
        /// </summary>
        /// <param name="markerType"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>
        /// <param name="scaleZ"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public static void PlaceMarkerOnGroundProperly(int markerType, float posX, float posY, float posZ, float scaleX, float scaleY, float scaleZ, int r, int g, int b)
        {
            DrawMarker(markerType, posX, posY, posZ - 1f, 0f, 0f, 0f, 0f, 0f, 0f, scaleX, scaleY, scaleZ, r, g, b, 255, false, false, 2, false, null, null, false);
        }
        /// <summary>
        /// Places a marker on the ground properly
        /// </summary>
        /// <param name="markerType"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>
        /// <param name="scaleZ"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public static void PlaceMarkerOnGroundProperly(MarkerType markerType, float posX, float posY, float posZ, float scaleX, float scaleY, float scaleZ, int r, int g, int b)
        {
            DrawMarker((int)markerType, posX, posY, posZ - 1f, 0f, 0f, 0f, 0f, 0f, 0f, scaleX, scaleY, scaleZ, r, g, b, 255, false, false, 2, false, null, null, false);
        }
        /// <summary>
        /// Places a marker on the ground properly
        /// </summary>
        /// <param name="markerType"></param>
        /// <param name="markerPos"></param>
        /// <param name="markerDir"></param>
        /// <param name="markerRot"></param>
        /// <param name="markerScale"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <param name="bobUpAndDown"></param>
        /// <param name="faceCamera"></param>
        /// <param name="p19"></param>
        /// <param name="rotate"></param>
        /// <param name="textureDict"></param>
        /// <param name="textureName"></param>
        /// <param name="drawOnEnts"></param>
        public static void PlaceMarkerOnGroundProperly(MarkerType markerType, Vector3 markerPos, Vector3 markerDir, Vector3 markerRot, Vector3 markerScale, int r, int g, int b, int a, bool bobUpAndDown, bool faceCamera, int p19, bool rotate, string textureDict, string textureName, bool drawOnEnts)
        {
            p19 = 2;
            markerRot.X = 0f;
            markerRot.Y = 0f;
            markerRot.Z = 0f;

            DrawMarker((int)markerType, markerPos.X, markerPos.Y, markerPos.Z - 1f, markerDir.X, markerDir.Y, markerDir.Z, markerRot.X, markerRot.Y, markerRot.Z, markerScale.X, markerScale.Y, markerScale.Z, r, g, b, a, bobUpAndDown, faceCamera, p19, rotate, textureDict, textureName, drawOnEnts);
        }
        /// <summary>
        /// Places a marker on the ground properly
        /// </summary>
        /// <param name="markerType"></param>
        /// <param name="markerPos"></param>
        /// <param name="markerRot"></param>
        /// <param name="markerScale"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <param name="bobUpAndDown"></param>
        /// <param name="faceCamera"></param>
        /// <param name="rotate"></param>
        /// <param name="textureDict"></param>
        /// <param name="textureName"></param>
        /// <param name="drawOnEnts"></param>
        public static void PlaceMarkerOnGroundProperly(MarkerType markerType, Vector3 markerPos, Vector3 markerRot, Vector3 markerScale, int r, int g, int b, int a, bool bobUpAndDown, bool faceCamera, bool rotate, string textureDict, string textureName, bool drawOnEnts)
        {
            markerRot.X = 0f;
            markerRot.Y = 0f;
            markerRot.Z = 0f;

            DrawMarker((int)markerType, markerPos.X, markerPos.Y, markerPos.Z - 1f, 0f, 0f, 0f, 0f, 0f, 0f, markerScale.X, markerScale.Y, markerScale.Z, r, g, b, a, bobUpAndDown, faceCamera, 2, rotate, textureDict, textureName, drawOnEnts);
        }
        /// <summary>
        /// Places a marker on the ground properly
        /// </summary>
        /// <param name="markerType"></param>
        /// <param name="markerPos"></param>
        /// <param name="markerScale"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <param name="bobUpAndDown"></param>
        /// <param name="rotate"></param>
        /// <param name="textureDict"></param>
        /// <param name="textureName"></param>
        /// <param name="drawOnEnts"></param>
        public static void PlaceMarkerOnGroundProperly(MarkerType markerType, Vector3 markerPos, Vector3 markerScale, int r, int g, int b, int a, bool bobUpAndDown, bool rotate, string textureDict, string textureName, bool drawOnEnts)
        {
            DrawMarker((int)markerType, markerPos.X, markerPos.Y, markerPos.Z - 1f, 0f, 0f, 0f, 0f, 0f, 0f, markerScale.X, markerScale.Y, markerScale.Z, r, g, b, a, bobUpAndDown, false, 2, rotate, textureDict, textureName, drawOnEnts);
        }
        /// <summary>
        /// Places a marker on the ground properly
        /// </summary>
        /// <param name="markerType"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>
        /// <param name="scaleZ"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        public static void PlaceMarkerOnGroundProperly(int markerType, float posX, float posY, float posZ, float scaleX, float scaleY, float scaleZ, int r, int g, int b, int a)
        {
            DrawMarker(markerType, posX, posY, posZ - 1f, 0f, 0f, 0f, 0f, 0f, 0f, scaleX, scaleY, scaleZ, r, g, b, a, false, false, 2, false, null, null, false);
        }
        /// <summary>
        /// Places a marker on the ground properly
        /// </summary>
        /// <param name="markerType"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>
        /// <param name="scaleZ"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        public static void PlaceMarkerOnGroundProperly(MarkerType markerType, float posX, float posY, float posZ, float scaleX, float scaleY, float scaleZ, int r, int g, int b, int a)
        {
            DrawMarker((int)markerType, posX, posY, posZ - 1f, 0f, 0f, 0f, 0f, 0f, 0f, scaleX, scaleY, scaleZ, r, g, b, a, false, false, 2, false, null, null, false);
        }
        /// <summary>
        /// Places a marker on the ground properly
        /// </summary>
        /// <param name="markerType"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>
        /// <param name="scaleZ"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="textureDict"></param>
        /// <param name="textureName"></param>
        public static void PlaceMarkerOnGroundProperly(int markerType, float posX, float posY, float posZ, float scaleX, float scaleY, float scaleZ, int r, int g, int b, string textureDict, string textureName)
        {
            DrawMarker(markerType, posX, posY, posZ - 1f, 0f, 0f, 0f, 0f, 0f, 0f, scaleX, scaleY, scaleZ, r, g, b, 255, false, false, 2, false, textureDict, textureName, false);
        }
        /// <summary>
        /// Places a marker on the ground properly
        /// </summary>
        /// <param name="markerType"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>
        /// <param name="scaleZ"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="textureDict"></param>
        /// <param name="textureName"></param>
        public static void PlaceMarkerOnGroundProperly(MarkerType markerType, float posX, float posY, float posZ, float scaleX, float scaleY, float scaleZ, int r, int g, int b, string textureDict, string textureName)
        {
            DrawMarker((int)markerType, posX, posY, posZ - 1f, 0f, 0f, 0f, 0f, 0f, 0f, scaleX, scaleY, scaleZ, r, g, b, 255, false, false, 2, false, textureDict, textureName, false);
        }
        /// <summary>
        /// Places a marker on the ground properly
        /// </summary>
        /// <param name="markerType"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>
        /// <param name="scaleZ"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <param name="textureDict"></param>
        /// <param name="textureName"></param>
        public static void PlaceMarkerOnGroundProperly(MarkerType markerType, float posX, float posY, float posZ, float scaleX, float scaleY, float scaleZ, int r, int g, int b, int a, string textureDict, string textureName)
        {
            DrawMarker((int)markerType, posX, posY, posZ - 1f, 0f, 0f, 0f, 0f, 0f, 0f, scaleX, scaleY, scaleZ, r, g, b, a, false, false, 2, false, textureDict, textureName, false);
        }
        /// <summary>
        /// Places a marker on the ground properly
        /// </summary>
        /// <param name="markerType"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>
        /// <param name="scaleZ"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <param name="textureDict"></param>
        /// <param name="textureName"></param>
        public static void PlaceMarkerOnGroundProperly(int markerType, float posX, float posY, float posZ, float scaleX, float scaleY, float scaleZ, int r, int g, int b, int a, string textureDict, string textureName)
        {
            DrawMarker(markerType, posX, posY, posZ - 1f, 0f, 0f, 0f, 0f, 0f, 0f, scaleX, scaleY, scaleZ, r, g, b, a, false, false, 2, false, textureDict, textureName, false);
        }
        /// <summary>
        /// Gets the distance to a marker.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="markerPos"></param>
        public static void GetDistanceToMarker(Vector3 position, Vector3 markerPos) => Vdist(position.X, markerPos.Y, markerPos.Z, markerPos.X, markerPos.Y, markerPos.Z);
        /// <summary>
        /// Gets the distance to a marker.
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <param name="markerPosX"></param>
        /// <param name="markerPosY"></param>
        /// <param name="markerPosZ"></param>
        public static void GetDistanceToMarker(float posX, float posY, float posZ, float markerPosX, float markerPosY, float markerPosZ) => Vdist(posX, posY, posZ, markerPosX, markerPosY, markerPosZ);
    }
}
