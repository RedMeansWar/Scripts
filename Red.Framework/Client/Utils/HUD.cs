using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static CitizenFX.Core.UI.Screen;

namespace Red.Framework.Client.Utils
{
    internal class HUD
    {
        #region Notifications
        public static string SuccessNotification(string message)
        {
            ShowNotification($"~g~~h~Success~h~~s~: {message}", true);
            return message;
        }

        public static string ErrorNotification(string message)
        {
            ShowNotification($"~r~~h~Error~h~~s~: {message}", true);
            return message;
        }

        public static string AlertNotification(string message)
        {
            ShowNotification($"~y~~h~Alert~h~~s~: {message}", true);
            return message;
        }

        public static void DisplayNotification(string message)
        {
            SetNotificationTextEntry("STRING");
            AddTextComponentString(message);
            EndTextCommandThefeedPostTicker(false, false);
        }
        #endregion
    }
}
