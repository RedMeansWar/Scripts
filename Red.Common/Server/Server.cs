using System.Linq;
using CitizenFX.Core;

namespace Red.Common.Server
{
    #pragma warning disable
    public class Server : BaseScript
    {
        #region Variables
        // These are all seperated method for use if method is not needed.
        protected static ExportDictionary ResourceExports;
        #endregion

        #region Misc Methods
        /// <summary>
        /// Retrieves a dynamic export from a resource.
        /// </summary>
        /// <param name="resource">The name of the resource to retrieve the export from.</param>
        /// <returns>The exported value, or null if not found.</returns>
        public static dynamic Exports(string resource) => ResourceExports[resource];
        #endregion

        #region Player Identifiers
        // Methods for retrieving various player identifiers with reliability ratings:

        /// <summary>
        /// Gets the player's license ID.
        /// Reliability: Best, but may change on system resets.
        /// </summary>
        /// <param name="player">The player to get the identifier for.</param>
        /// <returns>The player's license ID, or an empty string if not found.</returns>
        public static string GetLicenseId(Player player) => GetIdentifierFromType(player, "license:");

        /// <summary>
        /// Gets the player's Discord ID.
        /// Reliability: Excellent
        /// </summary>
        /// <param name="player">The player to get the identifier for.</param>
        /// <returns>The player's Discord ID, or an empty string if not found.</returns>
        public static string GetDiscordId(Player player) => GetIdentifierFromType(player, "discord:");

        /// <summary>
        /// Gets the player's Steam Hex.
        /// Reliability: Excellent
        /// </summary>
        /// <param name="player">The player to get the identifier for.</param>
        /// <returns>The player's Steam Hex, or an empty string if not found.</returns>
        public static string GetSteamId(Player player) => GetIdentifierFromType(player, "steam:");

        /// <summary>
        /// Gets the player's Live ID.
        /// Reliability: Poor
        /// </summary>
        /// <param name="player">The player to get the identifier for.</param>
        /// <returns>The player's Xbox Live Id, or an empty string if not found.</returns>
        public static string GetLiveId(Player player) => GetIdentifierFromType(player, "live:");

        /// <summary>
        /// Gets the player's IP address.
        /// Reliability: Excellent
        /// </summary>
        /// <param name="player">The player to get the identifier for.</param>
        /// <returns>The player's IP address, or an empty string if not found.</returns>
        public static string GetIpId(Player player) => GetIdentifierFromType(player, "ip:");

        /// <summary>
        /// Gets the player's Xbox Live ID.
        /// Reliability: Poor
        /// </summary>
        /// <param name="player">The player to get the identifier for.</param>
        /// <returns>The player's Xbox Live Id, or an empty string if not found.</returns>
        public static string GetXblId(Player player) => GetIdentifierFromType(player, "xbl:");

        /// <summary>
        /// Gets the player's Public IP Address.
        /// Reliability: Best
        /// </summary>
        /// <param name="player"></param>
        /// <returns>The player's IP Address, or an empty string if not found.</returns>
        public static string GetIpAddress(Player player) => GetIdentifierFromType(player, "ip:");

        /// <summary>
        /// Gets the identifier from a player's identifiers that starts with a given prefix.
        /// </summary>
        /// <param name="player">The player to get the identifier from.</param>
        /// <param name="identifierPrefix">The prefix of the identifier to get.</param>
        /// <returns>The identifier that starts with the given prefix, or an empty string if not found.</returns>
        public static string GetIdentifierFromType(Player player, string identifierPrefix)
        {
            // Initialize an empty string to store the indentifier
            string id = "";

            // Check if the player and thheir identifiers are not null
            if (player is not null && player.Identifiers is not null)
            {
                // Use LINQ to find the first identifier that start with the given prefix
                //   - Where: filters the identifiers to only include those that start with the prefix
                //   - Select: removes the prefix from the identifier
                //   - FirstOrDefault: gets the first element from the filtered collection, or null if none found
                id = player.Identifiers.Where(prefix => prefix.StartsWith(identifierPrefix))
                                       .Select(s => s.Replace(identifierPrefix, ""))
                                       .FirstOrDefault();

                // If the id is null or whitespace, set it to an empty string
                if (string.IsNullOrWhiteSpace(id))
                {
                    id = "";
                }
            }

            // Return the identifier.
            return id;
        }
        #endregion
    }
}