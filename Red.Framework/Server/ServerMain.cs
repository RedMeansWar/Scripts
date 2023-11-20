using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using CitizenFX.Core;
using Red.Common.Server.Diagnostics;
using static CitizenFX.Core.Native.API;
using static Red.Common.Server.Server;

namespace Red.Framework.Server
{
    public class ServerMain : BaseScript
    {
        #region Variables
        protected string currentAOP = "Not Set";
        protected string aopSetter = "System";
        #endregion

        #region Commands
        [Command("aop")]
        private void AopCommand([FromSource] Player player, string[] args)
        {
            if (args.Length != 0)
            {
                currentAOP = string.Join(" ", args);
                aopSetter = player.Name;
                TriggerClientEvent("Framework:Client:changeAOP", currentAOP, player.Name);
            }
            else if (currentAOP is null || aopSetter is null)
            {
                AddChatMessage("System", "We couldn't retrieve the current AOP, try again later.");
            }
            else
            {
                AddChatMessage("System", $"Current AOP is ^5^*{currentAOP}^r^7 (Set by: ^5^*{aopSetter}^r^7)");
            }
        }
        #endregion

        #region Event Handlers
        [EventHandler("Framework:DropUser")]
        private void OnDropUser([FromSource] Player player) => DropUserFromServer(player, "Dropped via framework.");
        #endregion
    }
}