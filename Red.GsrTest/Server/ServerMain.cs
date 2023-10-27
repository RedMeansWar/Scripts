using System;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace Red.GsrTest.Server
{
    public class ServerMain : BaseScript
    {
        [EventHandler("GsrTest:Server:doGsrTest")]
        private void OnSubmitGsrTest([FromSource] Player testerPlayer, int testedPlayerId)
        {
            Player testedPlayer = Players[testedPlayerId];

            testedPlayer?.TriggerEvent("GsrTest:Client:doGsrTest", testerPlayer.Handle);
        }

        [EventHandler("GsrTest:Server:returnGsrResult")]
        private void OnReturnGsrTest(bool shotRecently, string testerPlayerId)
        {
            Player testerPlayer = Players[int.Parse(testerPlayerId)];

            testerPlayer.TriggerEvent("GsrTest:Client:showClientNotification", shotRecently ? "Sample from swab comes back ~g~~h~positive~h~~s~." : "Sample from swab comes back ~o~~h~negative~h~~s~.");
        }
    }
}