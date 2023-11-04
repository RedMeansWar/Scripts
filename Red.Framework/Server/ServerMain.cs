using System;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.Common.Server.Server;

namespace Red.Framework.Server
{
    public class ServerMain : BaseScript
    {
        #region Variables
        protected string currentAOP = "Statewide";
        protected string aopSetter = "SYSTEM";
        #endregion

        #region Commands
        [Command("aop")]
        private void AopCommand([FromSource] Player player, string[] args)
        {
            if (args.Length != 0)
            {
                currentAOP = string.Join(" ", args);
                aopSetter = player.Name;
                TriggerClientEvent("Framework:Client:updateAOP", player.Name, currentAOP);
            }
        }
        #endregion

        #region Event Handlers
        [EventHandler("Framework:Server:dropUser")]
        private void OnDropUserFromFramework([FromSource] Player player, string reason)
        {
            DropUserFromServer(player, reason);
        }
        #endregion
    }
}
