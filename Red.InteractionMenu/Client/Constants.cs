using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;

namespace Red.InteractionMenu.Client
{
    internal class Constants
    {
        public static Ped PlayerPed = Game.PlayerPed;

        public static string forwardArrow = "→→→";
        public static string backwardArrow = "←←←";

        public static Vehicle GetClosestVehicle(float radius = 2f) => PlayerPed.GetClosestVehicleToClient(radius);
        
        public static string SuccessNotification(string message, bool blink = false)
        {
            Screen.ShowNotification($"~g~~h~Success~h~~s~: {message}", blink);
            return message;
        }

        public static string ErrorNotification(string message, bool blink = false)
        {
            Screen.ShowNotification($"~r~~h~Error~h~~s~: {message}", blink);
            return message;
        }
    }

    public static class Extensions
    {
        public static Vehicle GetClosestVehicleToClient(this Ped ped, float radius = 2f)
        {
            Vector3 plyrPos = ped.Position;

            RaycastResult raycast = World.RaycastCapsule(plyrPos, plyrPos, radius, (IntersectOptions)10, Game.PlayerPed);

            if (!Entity.Exists(raycast.HitEntity) || !raycast.HitEntity.Model.IsVehicle || !raycast.DitHitEntity)
            {
                return null;
            }
            else
            {
                return (Vehicle)raycast.HitEntity;
            }
        }
    }
}
