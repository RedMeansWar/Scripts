using System;
using System.Linq;
using CitizenFX.Core;
using SharpConfig;
using static CitizenFX.Core.Native.API;


namespace Red.Chat.Server
{
    public class ServerMain : BaseScript
    {

        #region Varibles
        protected string communityName;
        #endregion

        #region Constructor
        public ServerMain() => ReadConfigFile();
        #endregion

        #region Methods
        private void ReadConfigFile()
        {
            var data = LoadResourceFile(GetCurrentResourceName(), "config.ini");

            if (Configuration.LoadFromString(data).Contains("Chat", "CommunityName") == true)
            {
                Configuration loaded = Configuration.LoadFromString(data);

                communityName = loaded["Chat"]["CommunityName"].StringValue;
            }
            else
            {
                Debug.WriteLine($"[Chat]: Config file has not been configured correctly.");
            }
        }
        #endregion

        #region Commands
        [Command("say")]
        private void OnSayCommand([FromSource] Player player, string[] args)
        {
            if (player.Handle != "0")
            {
                return;
            }

            TriggerLatentClientEvent("chat:addMessage", 5000, "pnw", new int[] { 194, 39, 39 }, string.Join(" ", args));
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

                    ply.TriggerEvent("chat:addMessage", chatMessageArgs);
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
        #endregion
    }
}