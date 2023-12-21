using System;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.Breathalyzer.Server
{
    public class ServerMain : ServerScript
    {
        #region Event Handlers
        [EventHandler("Breathalyzer:Server:sumbitBacTest")]
        private void OnSubmitBacTest([FromSource] Player testerPlayer, int testedId)
        {
            Player testedPlayer = Players[testedId];
            testerPlayer?.TriggerEvent("Breathalyzer:Client:doBacTest", testedPlayer.Handle);
        }

        [EventHandler("Breathalyzer:Server:returnBacTest")]
        private void OnReturnBacTest(string testerId, string bacLevel)
        {
            Player testerPlayer = Players[int.Parse(testerId)];
            testerPlayer.TriggerEvent("Breathalyzer:Client:displayClientNotification", $"The BAC test returned a value of {bacLevel}%");
        }
        #endregion
    }
}