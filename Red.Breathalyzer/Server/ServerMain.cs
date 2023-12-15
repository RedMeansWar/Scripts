using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.Breathalyzer.Server
{
    public class ServerMain : BaseScript
    {
        [EventHandler("Breathalyzer:Server:startTest")]
        private void OnStartTest(int testedId)
        {
            Player testedPlayer = Players[testedId];
            testedPlayer?.TriggerEvent("Breathalyzer:Client:requestBacTest", testedPlayer.Handle);
        }
    }
}