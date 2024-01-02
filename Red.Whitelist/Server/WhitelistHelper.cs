using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Red.Whitelist.Server
{
    internal class ApiResponse
    {
        public int page { get; set; }
        public int perPage { get; set; }
        public int totalResults { get; set; }
        public int totalPages { get; set; }
        public ApiUser[] results { get; set; }
    }
    internal class ApiUser
    {
        public int id { get; set; }
        public string name { get; set; }
        public ApiGroup primaryGroup { get; set; }
        public ApiGroup[] secondaryGroups { get; set; }
        public Dictionary<string, ApiCustomField> customFields { get; set; }

    }
    internal class ApiGroup
    {
        public int id { get; set; }
        public string name { get; set; }
        public string formattedName { get; set; }
    }
    internal class ApiCustomField
    {
        public string name { get; set; }
        public Dictionary<string, ApiField> fields { get; set; }
    }
    internal class ApiField
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    internal class WhitelistConfig
    {

        public string apiBaseURL { get; set; }
        public string apiToken { get; set; }
        public string[] allowedGroupIds { get; set; }

        public string profileFieldName { get; set; }

        public string profileFieldSubNode { get; set; }
    }

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