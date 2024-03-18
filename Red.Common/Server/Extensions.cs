using System.Linq;
using CitizenFX.Core;

namespace Red.Common.Server
{
    #pragma warning disable
    public static class Extensions
    {
        #region Variables
        // These are all seperated method for use if method is not needed.
        #endregion

        #region Player Identifiers
        // Methods for retrieving various player identifiers with reliability ratings:

        /// <summary>
        /// Gets the player's license ID.
        /// Reliability: Best, but may change on system resets.
        /// </summary>
        /// <param name="player">The player to get the identifier for.</param>
        /// <returns>The player's license ID, or an empty string if not found.</returns>
        public static string GetLicenseId(this Player player) => GetIdentifierFromType(player, "license:");

        /// <summary>
        /// Gets the player's Discord ID.
        /// Reliability: Excellent
        /// </summary>
        /// <param name="player">The player to get the identifier for.</param>
        /// <returns>The player's Discord ID, or an empty string if not found.</returns>
        public static string GetDiscordId(this Player player) => GetIdentifierFromType(player, "discord:");

        /// <summary>
        /// Gets the player's Steam Hex.
        /// Reliability: Excellent
        /// </summary>
        /// <param name="player">The player to get the identifier for.</param>
        /// <returns>The player's Steam Hex, or an empty string if not found.</returns>
        public static string GetSteamId(this Player player) => GetIdentifierFromType(player, "steam:");

        /// <summary>
        /// Gets the player's Live ID.
        /// Reliability: Poor
        /// </summary>
        /// <param name="player">The player to get the identifier for.</param>
        /// <returns>The player's Xbox Live Id, or an empty string if not found.</returns>
        public static string GetLiveId(this Player player) => GetIdentifierFromType(player, "live:");

        /// <summary>
        /// Gets the player's IP address.
        /// Reliability: Excellent
        /// </summary>
        /// <param name="player">The player to get the identifier for.</param>
        /// <returns>The player's IP address, or an empty string if not found.</returns>
        public static string GetIpId(this Player player) => GetIdentifierFromType(player, "ip:");

        /// <summary>
        /// Gets the player's Xbox Live ID.
        /// Reliability: Poor
        /// </summary>
        /// <param name="player">The player to get the identifier for.</param>
        /// <returns>The player's Xbox Live Id, or an empty string if not found.</returns>
        public static string GetXblId(this Player player) => GetIdentifierFromType(player, "xbl:");

        /// <summary>
        /// Gets the identifier from a player's identifiers that starts with a given prefix.
        /// </summary>
        /// <param name="player">The player to get the identifier from.</param>
        /// <param name="identifierPrefix">The prefix of the identifier to get.</param>
        /// <returns>The identifier that starts with the given prefix, or an empty string if not found.</returns>
        public static string GetIdentifierFromType(this Player player, string identifierPrefix)
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

        #region Colored Text
        public static string RedOrangeText(string message) => $"^1{message}";

        public static string LightGreenText(string message) => $"^2{message}";

        public static string LightYellowText(this string message) => $"^3{message}";

        public static string DarkBlueText(this string message) => $"^4{message}";

        public static string LightBlueText(this string message) => $"^5{message}";

        public static string VioletText(this string message) => $"^6{message}";

        public static string WhiteText(this string message) => $"^7{message}";

        public static string BloodRedText(this string message) => $"^8{message}";

        public static string FuchsiaText(this string message) => $"^9{message}";

        public static string BoldText(this string message) => $"^*{message}";

        public static string UnderlineText(this string message) => $"^_{message}";

        public static string StrikethroughText(this string message) => $"^~{message}";

        public static string UnderlineStrikethroughText(this string message) => $"^={message}";

        public static string BoldUnderlineStrikethroughText(this string message) => $"^*^={message}";

        public static string CancelFormattingText(this string message) => $"^r{message}";
        #endregion
    }
}
