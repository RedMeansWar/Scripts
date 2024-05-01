using System.Linq;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.Chat.Server
{
    public class ServerMain : BaseScript
    {
        #region Commands
        [Command("say")]
        private void OnSayCommand([FromSource] Player player, string[] args)
        {
            if (player.Handle != "0")
            {
                return;
            }

            TriggerLatentClientEvent("_chat:messageEntered", 5000, "INRP", new int[] { 194, 39, 39 }, string.Join(" ", args));
        }
        #endregion

        #region Event Handlers
        [EventHandler("__cfx_internal:commandFallback")]
        private void OnCommandNotRcon([FromSource] Player player, string msg)
        {
            string name = player.Name;
            TriggerEvent("chatMessage", player.Handle, name, $"/{msg}");
            CancelEvent();
        }

        [EventHandler("playerDropped")]
        private void OnPlayerDropped([FromSource] Player player, string reason) => TriggerClientEvent("chatMessage", "", new[] { 255, 255, 255 }, $"^* ^2 {player.Name} left. ({reason})");

        [EventHandler("_chatNearby")]
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

                    ply.TriggerEvent("_chat:messageEntered", chatMessageArgs);
                }
            }
        }

        [EventHandler("_chat:messageEntered")]
        private void OnMessageEntered([FromSource] Player player, dynamic author, dynamic color, dynamic message)
        {
            Debug.WriteLine($"{author}: {message}^0");

            if (!WasEventCanceled() && !message.StartsWith("/"))
            {
                TriggerClientEvent("_chat:messageEntered", author, color, message);
            }
        }

        [EventHandler("_chat:radioMessage")]
        private void OnRadioMessage([FromSource] Player player, string message)
        {
            TriggerClientEvent("chat:radioMessage", $"{player.Name} (#{int.Parse(player.Handle)})", message);
        }

        [EventHandler("_chat:twitterMessage")]
        private void OnTwitterMessage([FromSource] Player player, string username, string message)
        {
            TriggerClientEvent("chat:twitterMessage", username, message);
        }

        [EventHandler("_chat:911Message")]
        private void On911Message([FromSource] Player player, string location, string message)
        {
            TriggerClientEvent("chat:911Message", $"{player.Name} [{location}] (#{int.Parse(player.Handle)})", message);
        }
        #endregion
    }
}