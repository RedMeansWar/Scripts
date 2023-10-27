using System;
using System.Collections.Generic;
using CitizenFX.Core;

namespace Red.Common.Server
{
    public class Server : BaseScript
    {
        protected Dictionary<int, string> SteamId = new();

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
    }
}