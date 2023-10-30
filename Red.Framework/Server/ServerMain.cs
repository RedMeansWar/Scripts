using System;
using CitizenFX.Core;

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
                sender.TriggerEvent("chat:addMessage", new { color = new[] { 0, 73, 83 }, multiline = true, args = new[] { "", "We couldn't retrieve the current AOP, try again later." } });
            }
            else
            {
                sender.TriggerEvent("chat:addMessage", new { templateId = "TemplateGrey", color = new[] { 255, 255, 255 }, multiline = true, args = new[] { "", $"Current AOP is ^5^*{currentAOP}^r^7 (Set by: ^5^*{currentAOPSetter}^r^7)" } });
            }
        }
        #endregion
    }
}
