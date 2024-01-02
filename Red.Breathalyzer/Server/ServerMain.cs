using System;
using System.Collections.Generic;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.Breathalyzer.Server
{
    public class ServerMain : ServerScript
    {
        #region Event Handlers
        [EventHandler("Breathalyzer:Server:submitBacTest")]
        private void OnSubmitBacTest([FromSource] Player testerPlayer, int testedId)
        {
            Player testedPlayer = Players[testedId];
            testedPlayer?.TriggerEvent("Breathalyzer:Client:doBacTest", testerPlayer.Handle);
        }

        [EventHandler("Breathalyzer:Server:returnBacTest")]
        private void OnReturnBacTest(string testerId, string bacLevel)
        {
            Player testerPlayer = Players[int.Parse(testerId)];
            testerPlayer.TriggerEvent("Breathalyzer:Client:returnBacLevel", bacLevel);
        }
        #endregion
    }
}