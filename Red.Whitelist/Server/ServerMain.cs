using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using SharpConfig;
using RestSharp;
using Newtonsoft.Json;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.Whitelist.Server
{
    public class ServerMain : BaseScript
    {
        #region Variabless
        protected bool usingInvision;
        protected string communityURL, apiKey, profileFieldName, profileFieldSubNode, discordToken, guildId;
        protected string whitelistedRole = "1187946523753988116";

        protected List<string> groupIds = new();
        protected HashSet<string> whitelistedSteamHexes { get; set; }
        #endregion

        #region Constuructor
        public ServerMain()
        {
            ReadConfig();

            if (usingInvision)
            {
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sllPolicyErrors) => true;

                if (communityURL is null || apiKey is null || groupIds.Count == 0)
                {
                    Debug.WriteLine($"^1No config variables set! Whitelist won't operate.^0");
                    throw new Exception("NO_CONFIG");
                }

                Timer updateTimer = new(TimeSpan.FromHours(6).TotalMilliseconds);
                updateTimer.AutoReset = true;
                updateTimer.Elapsed += UpdateTimer_Elapsed;
                updateTimer.Start();
            }
        }
        #endregion

        #region Commands
        [Command("updatewhitelist")]
        private void UpdateWhitelistCommand([FromSource] Player player)
        {
            if (usingInvision)
            {
                if (IsPlayerAceAllowed(player.Handle, "vMenu:Admin"))
                {
                    Debug.WriteLine($"{player.Name} is updating the Invision whitelist...");
                }
                else
                {
                    player.TriggerEvent("_chat:chatMessage", "SYSTEM", new[] { 255, 0, 0 }, $"You don't have permission to use this command!");
                }

                if (player is null)
                {
                    Debug.WriteLine("Updating Invision whitelist...");
                }

                UpdateWhitelist();
            }
            else
            {
                Debug.WriteLine("Invision is not enabled in the config this command has been disabled.");
                player.TriggerEvent("_chat:chatMessage", "SYSTEM", new[] { 255, 0, 0 }, $"This command is disabled.");
            }
        }
        #endregion

        #region Methods
        private async void UpdateWhitelist()
        {
            HashSet<ApiUser> users = new();
            List<string> steamHexes = new();

            for (int i = 0; i < groupIds.Count; i++)
            {
                RestClient restClient = new(communityURL);
                RestRequest restRequest = (RestRequest)new RestRequest($"core/members?group={groupIds[i]}&perPage=500&key={apiKey}", Method.GET, DataFormat.Json).AddHeader("User-Agent", "BreadSupply/1.0");
                RestResponse restResponse = (RestResponse)await restClient.ExecuteAsync(restRequest);

                if (restResponse.StatusCode == (HttpStatusCode)200)
                {
                    var formatedResponse = JsonConvert.DeserializeObject<ApiResponse>(restResponse.Content);

                    foreach (var user in users)
                    {
                        users.Add(user);
                    }

                    for (int z = 2; z <= formatedResponse.totalPages; z++)
                    {
                        RestRequest internalRestRequest = (RestRequest)new RestRequest($"core/members?group={groupIds[i]}&perPage=500&page={z}&key={apiKey}", Method.GET, DataFormat.Json).AddHeader("User-Agent", "BreadSupply/1.0");
                        RestResponse internalRestResponse = (RestResponse)await restClient.ExecuteAsync(internalRestRequest);

                        if (internalRestResponse.StatusCode == (HttpStatusCode)200)
                        {
                            var internalFormattedResponse = JsonConvert.DeserializeObject<ApiResponse>(internalRestResponse.Content);

                            foreach (var user in internalFormattedResponse.results)
                            {
                                users.Add(user);
                            }
                        }
                        else
                        {
                            Debug.WriteLine($"^1Recieved a non 200 response code when trying to query the IPS API for some whitelisted users! ^7Status: {internalRestResponse.StatusCode} \n Response Content: {internalRestResponse.Content}^0");
                            Debug.WriteLine(internalRestResponse.ErrorMessage);
                        }
                    }

                    Debug.WriteLine($"^4Retrieved {users.Count} users so far from the API.");
                }
                else
                {
                    Debug.WriteLine($"^1Recieved a non 200 response code when trying to query the IPS API for some whitelisted users! ^7Status: {restResponse.StatusCode} \n Response Content: {restResponse.Content}^0");
                    Debug.WriteLine(restResponse.ErrorMessage);
                }

                foreach (var apiUser in users)
                {
                    if (CheckApiUserGroup(apiUser))
                    {
                        if (!(apiUser.customFields is null))
                        {
                            foreach (var customField in apiUser.customFields)
                            {
                                if (customField.Value.name.ToLower() == profileFieldName.ToLower())
                                {
                                    foreach (var field in customField.Value.fields)
                                    {
                                        if (field.Value.name.ToLower().StartsWith(profileFieldSubNode.ToLower()))
                                        {
                                            string steamHex = field.Value.value;
                                            if (!(steamHex is null))
                                            {
                                                steamHexes.Add(field.Value.value.ToLower().Replace("steam:", ""));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            Debug.WriteLine("^1 There are no custom fields set on the IPS Suite");
                            return;
                        }
                    }
                }
            }

            whitelistedSteamHexes = new HashSet<string>(steamHexes);
            Debug.WriteLine($"^2Updated whitelisted users! There are {whitelistedSteamHexes.Count} whitelisted users. ^0");
        }

        private void ReadConfig()
        {
            var data = LoadResourceFile(GetCurrentResourceName(), "config.ini");

            if (Configuration.LoadFromString(data).Contains("Whitelist", "UsingInvision") == true)
            {
                Configuration loaded = Configuration.LoadFromString(data);
                usingInvision = loaded["Whitelist"]["UsingInvision"].BoolValue;
                communityURL = loaded["Whitelist"]["CommunityURL"].StringValue;
                apiKey = loaded["Whitelist"]["ApiKey"].StringValue;
                profileFieldName = loaded["Whitelist"]["ProfileFieldName"].StringValue;
                profileFieldSubNode = loaded["Whitelist"]["ProfileFieldSubNode"].StringValue;
            }
            else
            {
                Debug.WriteLine("Config file has not been configured properly.");
            }
        }

        private bool CheckApiUserGroup(ApiUser user)
        {
            if (groupIds.Contains(user.primaryGroup.id.ToString()))
            {
                return true;
            }

            foreach (ApiGroup group in user.secondaryGroups)
            {
                if (groupIds.Contains(user.id.ToString()))
                {
                    return true;
                }
            }

            return false;
        }

        private void UpdateTimer_Elapsed(object sender, ElapsedEventArgs e) => Task.Factory.StartNew(UpdateWhitelist);
        #endregion

        #region Event Handlers
        [EventHandler("onServerResourceStart")]
        private void OnServerResourceStart(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName)
            {
                return;
            }

            Task.Factory.StartNew(UpdateWhitelist);
        }

        [EventHandler("playerConnecting")]
        private async void OnPlayerConnecting([FromSource] Player player, string playerName, dynamic setKickReason, dynamic deferrals)
        {
            deferrals.defer();
            await Delay(0);

            try
            {
                playerName = player.Name;
                deferrals.update($"Checking user: {playerName}");

                if (string.IsNullOrEmpty(player.Identifiers["discord"]))
                {
                    deferrals.done($"You must have Discord open and linked to FiveM to connect to the server!");
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
                    discordAPI.DefaultRequestHeaders.Add("Authorization", $"Bot TOKEN_HERE");

                    HttpResponseMessage memberObjRequest = discordAPI.GetAsync($"https://discord.com/api/v9/guilds/GUILD_ID_HERE/members/{player.Identifiers["discord"]}").GetAwaiter().GetResult();

                    if (!memberObjRequest.IsSuccessStatusCode)
                    {
                        if (memberObjRequest.StatusCode == HttpStatusCode.NotFound)
                        {
                            deferrals.done($"You aren't whitelisted! If this is an error contact a member of Administration.");
                            return;
                        }
                        else
                        {
                            deferrals.done($"Something went wrong while connecting to the server!");
                            var memberErrObjString = await memberObjRequest.Content.ReadAsStringAsync();
                            Debug.WriteLine($"Error {memberObjRequest.StatusCode}: {memberErrObjString}");
                        }
                    }

                    var memberObjString = await memberObjRequest.Content.ReadAsStringAsync();
                    DiscordGuildMember memberObj = JsonConvert.DeserializeObject<DiscordGuildMember>(memberObjString);

                    bool successfulWhitelist = false;

                    if (memberObj.Roles.Contains(whitelistedRole))
                    {
                        successfulWhitelist = true;
                        Debug.WriteLine($"{playerName} player found, allowing connection.");
                        deferrals.done();
                    }

                    if (successfulWhitelist is false)
                    {
                        deferrals.done("You aren't whitelisted! If this is an error contact a member of Administration.");
                        Debug.WriteLine($"{playerName} attempted to connected to the server but was blocked because they don't have the \"Whitelisted\" role on discord.");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                deferrals.done($"Something went wrong while connecting to the server...");
                Debug.WriteLine($"{ex}");
            }

            if (usingInvision)
            {
                deferrals.defer();
                await Delay(0);
                deferrals.update("Hold on. We're making sure you're allowed here.");
                Debug.WriteLine($"^4Connecting user: ^7{playerName}^3 (Steam: {player.Identifiers["steam"] ?? "N/A"}, Discord: {player.Identifiers["discord"] ?? "N/A"}, IP: {player.Identifiers["ip"] ?? "N/A"})^0");

                if (player.Identifiers["steam"] is null || player.Identifiers["discord"] is null)
                {
                    deferrals.done($"We failed to find your {((player.Identifiers["steam"] is null && player.Identifiers["discord"] is null) ? "Steam Hex and Discord Id" : player.Identifiers["steam"] is null ? "Steam Hex" : "Discord Id")}. Make sure that application is open.");
                    Debug.WriteLine($"^7{playerName} rejected. ^3They didn't have either Steam or Discord open.^0");
                    return;
                }

                if (whitelistedSteamHexes.Contains(player.Identifiers["steam"].ToLower().Replace("steam:", "")))
                {
                    deferrals.done();
                    Debug.WriteLine($"^7{playerName} authenticated. ^2Allowing connection.^0");
                }
                else
                {
                    deferrals.done("You're not whitelisted!");
                    Debug.WriteLine($"^7{playerName} didn't authenticate. ^1Terminating connection.^0");
                    return;
                }
            }
        }
        #endregion
    }
}
