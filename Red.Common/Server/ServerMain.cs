using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.Common.Server
{
    public class ServerMain : ServerScript
    {
        /// <summary>
        /// Get Player Identifier. Converted from my FrameworkHelper.lua found in my framework script.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="identifierPrefix"></param>
        /// <returns></returns>
        public static string GetIdentifierFromType(Player player, string identifierPrefix)
        {
            string id = ""; 
            
            if (player is not null && player.Identifiers is not null)
            {
                id = player.Identifiers.Where(prefix => prefix.StartsWith(identifierPrefix)).Select(s => s.Replace(identifierPrefix, "")).FirstOrDefault();

                if (string.IsNullOrWhiteSpace(id))
                {
                    id = "";
                }
            }

            return id;
        }
        /// <summary>
        /// Gets a players Rockstar Id
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static string GetLicenseId(Player player) => GetIdentifierFromType(player, "license:");
        /// <summary>
        /// Gets a players Discord Id
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static string GetDiscordId(Player player) => GetIdentifierFromType(player, "discord:");
        /// <summary>
        /// Gets a players steam Id
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static string GetSteamId(Player player) => GetIdentifierFromType(player, "steam:");
        /// <summary>
        /// Gets a player by their Server Id
        /// </summary>
        /// <param name="players"></param>
        /// <param name="serverId"></param>
        /// <returns></returns>
        public static Player GetPlayerById(List<Player> players, int serverId)
        {
            return players.Find(player => player.Handle == serverId.ToString());
        }
        /// <summary>
        /// Connects to a Discord guild and gets a specific role
        /// </summary>
        /// <param name="token"></param>
        /// <param name="guildId"></param>
        /// <param name="role"></param>
        public static async void GetDiscordRole(string token, string guildId, string role)
        {
            Player player = null;

            try
            {
                var playerName = player.Name;

                if (string.IsNullOrEmpty(player.Identifiers["discord"]))
                {
                    return;
                }

                HttpClientHandler clientHandler = new()
                {
                    ClientCertificateOptions = ClientCertificateOption.Automatic,
                    UseProxy = true,
                    UseDefaultCredentials = true
                };
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                using (var discordAPI = new HttpClient(clientHandler))
                {
                    discordAPI.DefaultRequestHeaders.Accept.Clear();
                    discordAPI.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    discordAPI.DefaultRequestHeaders.Add("Authorization", $"Bot {token}");

                    HttpResponseMessage memberObjRequest = discordAPI.GetAsync($"https://discord.com/api/v9/guilds/{guildId}/members/{player.Identifiers["discord"]}").GetAwaiter().GetResult();

                    if (!memberObjRequest.IsSuccessStatusCode)
                    {
                        if (memberObjRequest.StatusCode == HttpStatusCode.NotFound)
                        {
                            return;
                        }
                        else
                        {
                            var memberErrObjString = await memberObjRequest.Content.ReadAsStringAsync();
                            Debug.WriteLine($"Error {memberObjRequest.StatusCode}: {memberErrObjString}");
                        }
                    }

                    var memberObjString = await memberObjRequest.Content.ReadAsStringAsync();
                    DiscordGuildMember memberObj = JsonConvert.DeserializeObject<DiscordGuildMember>(memberObjString);

                    if (memberObj.Roles.Contains(role))
                    {
                        Debug.WriteLine($"successfully connected to {guildId} and found role {role}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex}");
            }
        }
        #region Event Handlers
        /// <summary>
        /// Drops a player from a player with a reason (default: No reason given.)
        /// </summary>
        /// <param name="player"></param>
        /// <param name="reason"></param>
        [EventHandler("Server:DropPlayer")]
        private void OnDropPlayer([FromSource] Player player, string reason = "No reason given.") => DropPlayer(player.Handle, reason);
        /// <summary>
        /// A event form of Client:SoundToClient
        /// </summary>
        /// <param name="networkId"></param>
        /// <param name="soundFile"></param>
        /// <param name="soundVolume"></param>
        #endregion

        public class DiscordUser
        {
            [JsonProperty("id")]
            public ulong Id { get; set; }

            [JsonProperty("username")]
            public string Username { get; set; }

            [JsonProperty("avatar")]
            public string Avatar { get; set; }

            [JsonProperty("avatar_decoration")]
            public string AvatarDecoration { get; set; }

            [JsonProperty("discriminator")]
            public string Discriminator { get; set; }

            [JsonProperty("public_flags")]
            public int PublicFlags { get; set; }
        }

        public class DiscordGuildMember
        {
            [JsonProperty("avatar")]
            public string Avatar { get; set; }

            [JsonProperty("communication_disabled_until")]
            public DateTimeOffset? TimedOutUntil { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("is_pending")]
            public bool IsPending { get; set; }

            [JsonProperty("joined_at")]
            public DateTimeOffset JoinedAt { get; set; }

            [JsonProperty("nick")]
            public string Nick { get; set; }

            [JsonProperty("pending")]
            public bool Pending { get; set; }

            [JsonProperty("premium_since")]
            public DateTimeOffset? PremiumSince { get; set; }

            [JsonProperty("roles")]
            public List<string> Roles { get; set; }

            [JsonProperty("user")]
            public DiscordGuildMember User { get; set; }

            [JsonProperty("mute")]
            public bool Mute { get; set; }

            [JsonProperty("deaf")]
            public bool Deaf { get; set; }
        }
    }
}