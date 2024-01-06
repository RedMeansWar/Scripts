using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using Red.Common.Server;
using static CitizenFX.Core.Native.API;

namespace Red.Framework.Server
{
    public class ServerMain : BaseScript
    {
        #region Variables
        protected string currentAOP = "Statewide";
        protected string aopSetter = "System";
        #endregion

        #region Constructor
        public ServerMain() => SetMapName("San Andreas");
        #endregion

        #region Commands
        [Command("aop")]
        private void AopCommand([FromSource] Player player, string[] args)
        {
            if (args.Length != 0)
            {
                currentAOP = string.Join(" ", args);
                aopSetter = player.Name;
                TriggerClientEvent("Framework:Client:changeAOP", currentAOP);
            }
            else if (currentAOP is null || aopSetter is null)
            {
                TriggerClientEvent("_chat:chatMessage", "SYSTEM", new[] { 255, 255, 255 }, $"Current AOP is ^5^*{currentAOP}^r^7 (Set by: ^5^*{aopSetter}^r^7)");
            }
            else
            {
                TriggerClientEvent("_chat:chatMessage", "SYSTEM", new[] { 255, 255, 255 }, $"Current AOP is ^5^*{currentAOP}^r^7 (Set by: ^5^*{aopSetter}^r^7)");
            }
        }
        #endregion

        #region Methods
        private void DropUserFromServer([FromSource] Player player, string reason = "Dropped from server.") => DropPlayer(player.Handle, reason);
        #endregion

        #region Event Handlers
        [EventHandler("playerConnecting")]
        private void OnPlayerConnecting([FromSource] Player player) => player.TriggerEvent("Framework:Client:syncInfo", currentAOP);

        [EventHandler("Framework:DropUser")]
        private void OnDropUser([FromSource] Player player) => DropUserFromServer(player, "Dropped via framework.");
        #endregion

        #region Ticks
        [Tick] 
        private async Task UpdateAop() => TriggerClientEvent("Framework:Client:syncInfo", currentAOP);
        #endregion
    }
}