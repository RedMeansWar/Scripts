using System;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.Common.Server.Server;

namespace Red.Framework.Server
{
    public class ServerMain : BaseScript
    {
        #region Variables
        protected static readonly Random random = new();
        protected string currentAOP = "Statewide";
        protected string currentAOPSetter = "System";
        #endregion

        #region Commands
        [Command("aop")]
        private void AopCommand([FromSource] Player player, string[] args) => ParseAOP(player, args);
        #endregion

        #region Event Handlers
        [EventHandler("Framework:Server:DropUser")]
        private void OnDropFrameworkUser([FromSource] Player player, string reason) => DropPlayer(player.Handle, reason);
        #endregion

        #region Methods
        private void ParseAOP(Player sender, string[] args)
        {
            if (args.Length != 0)
            {
                currentAOP = string.Join(" ", args);
                currentAOPSetter = sender.Name;
                TriggerClientEvent("Framework:Client:updateAOP", currentAOP, sender.Name);
            }
            else if (currentAOP is null || currentAOPSetter is null)
            {
                AddChatMessage("", "We couldn't retrieve the current AOP, try again later.", 0, 73, 83);
            }
            else
            {
                AddChatMessage("", $"Current AOP is ^5^*{currentAOP}^r^7 (Set by: ^5^*{currentAOPSetter}^r^7)");
            }
        }
        #endregion
    }
}
