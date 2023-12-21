using System;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.Common.Server
{
    public class ServerMain : ServerScript
    {
        /// <summary>
        /// Get Player Identifier
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
    }
}