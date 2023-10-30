using System;
using System.Linq;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.Chat.Server
{
    public class ServerMain : BaseScript
    {
        #region Commands
        [Command("say")]
        private void SayCommand([FromSource] Player player, string[] args)
        {
            if (player.Handle != null)
            {
                return;
            }

            TriggerLatentClientEvent("chat:chatMessage", 5000, "System", new int[] { 194, 39, 39 }, string.Join(" ", args));
        }
        #endregion

        #region Event Handlers
        [EventHandler("Chat:Server:chatNearby")]
        private void OnChatMessageNearby([FromSource] Player player, dynamic author, dynamic color, dynamic message, dynamic nearbyPlayers, Vector3 authorPos)
        {
            Debug.WriteLine($"{author}: {message}^0");

            foreach (var playerId in nearbyPlayers)
            {
                if (Players[playerId] is Player ply && ply.Name is not null)
                {
                    dynamic[] chatMessageArgs = { author, color, message };

                    if (ply.Handle != player.Handle)
                    {
                        chatMessageArgs = chatMessageArgs.Append(authorPos).ToArray();
                    }

                    ply.TriggerEvent("chat:chatMessage", chatMessageArgs);
                }
            }
        }

        [EventHandler("Chat:Server:messageEntered")]
        private void OnMessageEntered([FromSource] Player player, dynamic author, dynamic color, dynamic message)
        {
            Debug.WriteLine($"{author}: {message}^0");

            if (!WasEventCanceled() && !message.StartsWith("/"))
            {
                TriggerClientEvent("chat:chatMessage", author, color, message);
            }
        }

        [EventHandler("Chat:Server:radioMessage")]
        private void OnRadioMessage([FromSource] Player player, string message)
        {
            TriggerClientEvent("chat:radioMessage", $"{player.Name} (#{int.Parse(player.Handle)})", message);
        }

        [EventHandler("Chat:Server:twitterMessage")]
        private void OnTwitterMessage([FromSource] Player player, string username, string message)
        {
            TriggerClientEvent("chat:twitterMessage", username, message);
        }

        [EventHandler("Chat:Server:911Message")]
        private void On911Message([FromSource] Player player, string location, string message)
        {
            TriggerClientEvent("chat:911Message", $"{player.Name} [{location}] (#{int.Parse(player.Handle)})", message);
        }

        [EventHandler("Chat:Server:311Message")]
        private void On311Message([FromSource] Player player, string location, string message)
        {
            TriggerClientEvent("chat:311Message", $"{player.Name} [{location}] (#{int.Parse(player.Handle)})", message);
        }

        #endregion
    }
}