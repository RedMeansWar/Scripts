using System;
using System.Collections.Generic;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.Common.Server
{
    public class Server : BaseScript
    {
        #region Constructor
        public Server()
        {
            EventHandlers["Server:DropUser"] += new Action<Player, string>(DropUserFromServer);
        }
        #endregion

        public static void SendPlayerMessage(Player player, string message, double r, double g, double b)
        {
            TriggerClientEvent(player, "chat:addMessage", new
            {
                color = new[] { r, g, b },
                args = new[] { message }
            });
        }

        public static void SendChatMessageToAll(List<Player> players, string message, double r, double g, double b)
        {
            foreach (var player in players)
            {
                TriggerClientEvent(player, "chat:addMessage", new
                {
                    color = new[] { r, g, b },
                    args = new[] { message }
                });
            }
        }

        public static float RandomRange(float min, float max)
        {
            Random random = new();
            return (float)random.NextDouble() * (max - min) + min;
        }

        public static Player GetPlayerById(List<Player> players, int id)
        {
            return players.Find(player => player.Handle == id.ToString());
        }

        public static void AddChatMessage(string author, string text, bool multiLineEnabled = true, int r = 255, int g = 255, int b = 255)
        {
            TriggerEvent("chat:addMessage", new
            {
                multiline = multiLineEnabled,
                args = new[] { author, text },
                color = new[] { r, g, b }
            });
        }

        public static void AddChatMessage(string author, string text, int r = 255, int g = 255, int b = 255)
        {
            TriggerEvent("chat:addMessage", new
            {
                args = new[] { author, text },
                color = new[] { r, g, b }
            });
        }

        public static void AddChatMessage(string author, string text)
        {
            TriggerEvent("chat:addMessage", new
            {
                args = new[] { author, text },
            });
        }

        #region Event Handlers
        public static void DropUserFromServer([FromSource] Player player, string reason = "Dropped from server.") => DropPlayer(player.Handle, reason);
        #endregion
    }
}