using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.Framework.Server
{
    #pragma warning disable
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
            if (args.Length != 0 && IsPlayerAceAllowed(player.Handle, "framework.updateAOP"))
            {
                currentAOP = string.Join(" ", args);
                aopSetter = player.Name;
                TriggerClientEvent("Framework:Client:changeAOP", currentAOP);
            }
            else if (currentAOP is null || aopSetter is null)
            {
                TriggerClientEvent("_chat:chatMessage", "SYSTEM", new[] { 255, 255, 255 }, $"Current AOP is now ^5^*{currentAOP}^r^7 (Set by: ^5^*{aopSetter}^r^7)");
                return;
            }
            else
            {
                TriggerClientEvent("chat:addMessage", "SYSTEM", new[] { 255, 255, 255 }, $"Current AOP is ^5^*{currentAOP}^r^7");
            }
        }
        #endregion

        #region Methods
        private void DropUserFromServer([FromSource] Player player, string reason = "Dropped from server.") => DropPlayer(player.Handle, reason);
        #endregion

        #region Event Handlers
        [EventHandler("Framework:DropUser")]
        private void OnDropUser([FromSource] Player player) => DropUserFromServer(player, "Dropped via framework.");

        [EventHandler("Framework:Server:syncAop")]
        private void OnSyncInfo(string aop) => TriggerClientEvent("Framework:Client:syncAop", aop);

        [EventHandler("Framework:Server:configError")]
        private void OnConfigError(string message) => Debug.WriteLine(message);

        [EventHandler("Framework:Server:updateCommunityName")]
        private void OnUpdateCommunityName(string communityName) => TriggerClientEvent("Framework:Client:updateCommunityName", communityName);
        #endregion

        #region Ticks
        [Tick] 
        private async Task SyncAOP() => OnSyncInfo(currentAOP);
        #endregion
    }
}