using CitizenFX.Core;

namespace Red.Common.Server
{
    public static class Extensions
    {
        #region Player Identifiers
        public static string GetLicenseId(this Player player) => Server.GetLicenseId(player); // Best (hard to say "Best" considering on a system reset it changes.

        public static string GetDiscordId(this Player player) => Server.GetDiscordId(player); // Excellent

        public static string GetSteamId(this Player player) => Server.GetSteamId(player); // Excellent

        public static string GetLiveId(this Player player) => Server.GetLiveId(player); // Poor

        public static string GetIpId(this Player player) => Server.GetIpId(player);  // Excellent

        public static string GetXblId(this Player player) => Server.GetXblId(player); // Poor

        public static string GetIpAddress(this Player player) => Server.GetIpAddress(player); // Best
        #endregion
    }
}
