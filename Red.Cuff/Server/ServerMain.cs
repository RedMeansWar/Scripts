using System;
using CitizenFX.Core;

namespace Red.Cuff.Server
{
    public class ServerMain : BaseScript
    {
        [EventHandler("Cuff:Server:cuffClosestPlayer")]
        private void OnCuffClosestPlayer([FromSource] Player sender, int target, bool isFront, bool isZiptie)
        {
            Player targetPlayer = Players[target];

            targetPlayer?.TriggerEvent("Cuff:Client:getCuffed", sender.Handle, isFront, isZiptie);
        }

        [EventHandler("Cuff:Server:playCuffAnimation")]
        private void OnPlayCuffAnimation(int cuffer, bool uncuff)
        {
            Player cufferPlayer = Players[cuffer];

            cufferPlayer?.TriggerEvent("Cuff:Client:playCuffAnimation", uncuff);
        }
    }
}