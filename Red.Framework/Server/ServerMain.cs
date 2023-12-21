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
        protected string currentAOP = "Not Set";
        protected string aopSetter = "System";
        #endregion

        #region Constructor
        public ServerMain()
        {
            SetMapName("San Andreas");
            SetConvarServerInfo("AreaOfPatrol", "");
        }
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
        private string GetServerServiceName() => GetConvar("red_service", "");

        #endregion

        #region Event Handlers
        [EventHandler("Framework:DropUser")]
        private void OnDropUser([FromSource] Player player) => DropUserFromServer(player, "Dropped via framework.");

        [EventHandler("Framework:Server:updateAOP")]
        private void OnUpdateAOP(string responseService, string responseCode)
        {
            string serviceName = GetServerServiceName();
            Player player = null;

            if (serviceName == "" || serviceName != responseService)
            {
                return;
            }

            if (responseCode == "400" || responseCode == "401")
            {
                return;
            }

            responseCode = responseCode.Replace("\"", "");

            if (currentAOP is null || (responseCode != "0" && currentAOP != responseCode))
            {
                currentAOP = responseCode;
                TriggerClientEvent("Framework:Client:changeAOP", currentAOP, player.);
            }
        }
        #endregion

        #region Ticks
        [Tick]
        private async Task AopTick()
        {
        }
        #endregion
    }
}