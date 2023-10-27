using System;
using System.Threading.Tasks;
using System.Linq;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.Framework.Events;

namespace Red.Framework.Server
{
    public class ServerMain : BaseScript
    {
        #region Variables
        protected Player Player;
        protected readonly Random random = new();
        protected string firstName, lastName, gender, department;
        protected string currentAOP = "AOP: Not Set";
        protected string aopSetter = "SYSTEM:";
        #endregion

        #region Constructor
        #endregion

        #region Event Handlers
        [EventHandler(ServerDropUserFromFramework)]
        private async void OnDropPlayerFromFramework([FromSource] Player player, string reason)
        {
            await Delay(1000);
            player.Drop(reason);
        }
        #endregion

        #region Commands
        [Command("aop")]
        private void AopCommand([FromSource] Player sender, string[] args)
        {
            if (IsAceAllowed("vMenu.Staff"))
            {
                currentAOP = string.Join(" ", args);
                aopSetter = sender.Name;
                TriggerClientEvent(ClientUpdateAreaOfPatrol, currentAOP, sender.Name);
            }
            else if (currentAOP is null || aopSetter is null)
            {
                sender.TriggerEvent("chat:addMessage", new { color = new[] { 0, 73, 83 }, multiline = true, args = new[] { "", "We couldn't retrieve the current AOP, try again later." } });
            }
            else
            {
                sender.TriggerEvent("chat:addMessage", new { color = new[] { 255, 255, 255 }, multiline = true, args = new[] { "", $"Current AOP is ^5^*{currentAOP}^r^7 (Set by: ^5^*{aopSetter}^r^7)" } });
            }
        } 
        #endregion
    }
}