using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Hud.HUD;
using static Red.Common.Client.Diagnostics.Log;

namespace Red.NearestPostal.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected readonly List<Postal> postals;
        protected string routedPostal, closestPostal;
        protected Blip blip;
        protected Ped PlayerPed = Game.PlayerPed;
        #endregion

        #region Constructor
        public ClientMain()
        {
            try
            {
                string json = LoadResourceFile(GetCurrentResourceName(), "postals.json") ?? "[]";

                if (!string.IsNullOrWhiteSpace(json))
                {
                    postals = JsonConvert.DeserializeObject<List<Postal>>(json, new JsonSerializerSettings
                    {
                        Converters = new List<JsonConverter> { new Vector2Converter() }
                    });

                    Info($"Loaded {postals.Count} postals");
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            Tick += UpdateClosestPostalTick;
            Tick += DisplayPostalTick;

            Exports.Add("getClosestPostal", GetClosestPostal); // for custom chat
        }
        #endregion

        #region Commands
        [Command("postal")]
        private void PostalCommand(string[] args)
        {
            if (args.Length < 1 && blip is not null)
            {
                Screen.ShowNotification($"~d~~h~Postals~h~~s~: Removed GPS route for postal {routedPostal}.", true);
                blip.Delete();
                blip = null;
                routedPostal = "000";
                return;
            }

            if (args.Length < 1)
            {
                Screen.ShowNotification("~d~~h~Postals~h~~s~: You have no GPS route to remove.", true);
                return;
            }

            string input = args[0].ToUpper();
            Postal foundPostal = postals.FirstOrDefault(postal => input == postal.Code);

            if (foundPostal is not null)
            {
                blip?.Delete();

                blip = World.CreateBlip(new(foundPostal.Location.X, foundPostal.Location.Y, 0f));
                blip.ShowRoute = true;
                blip.Color = (BlipColor)29;
                SetBlipRouteColour(blip.Handle, 29);
                blip.Name = $"Postal {foundPostal.Code}";
                routedPostal = foundPostal.Code;

                Screen.ShowNotification($"~d~~h~Postals~h~~s~: You've programmed your GPS to postal {foundPostal.Code}.", true);
            }
            else
            {
                Screen.ShowNotification($"~d~~h~Postals~h~~s~: Postal {input} doesn't seem to exist.", true);
            }
        }

        [Command("p")]
        private void PCommand(string[] args) => PostalCommand(args);
        #endregion

        #region Methods
        public string GetClosestPostal(Vector3 position)
        {
            Dictionary<string, float> results = new();

            foreach (Postal postal in postals)
            {
                float distance = Vector3.DistanceSquared((Vector3)postal.Location, position);
                
                if (!results.ContainsKey(postal.Code))
                {
                    results.Add(postal.Code, distance);
                }
            }

            return results.OrderBy(pair => pair.Value).First().Key;
        }
        #endregion

        #region Ticks
        private async Task DisplayPostalTick()
        {
            if (blip is not null && Vector2.DistanceSquared(new(PlayerPed.Position.X, PlayerPed.Position.Y), new(blip.Position.X, blip.Position.Y)) < 5000f)
            {
                Screen.ShowNotification("~d~~h~Postals~h~~s~: You've arrived at the postal.", true);
                blip.Delete();
                blip = null;
            }

            if (!HUDIsVisable)
            {
                await Delay(1000);
                return;
            }

            DrawText2d(1.133f, -0.065f, 0.43f, $"Nearby Postal: ~c~{closestPostal}", 255, 255, 255, 255);
        }

        private async Task UpdateClosestPostalTick()
        {
            closestPostal = GetClosestPostal(PlayerPed.Position);
            await Delay(1000);
        }
        #endregion
    }

    #region Classes
    public class Postal
    {
        public string Code { get; set; }
        public Vector2 Location { get; set; }

        public Postal(string code, float x, float y)
        {
            Code = code;
            Location = new(x, y);
        }
    }

    public class Vector2Converter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector2);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);
            float x = (float)jObject["X"];
            float y= (float)jObject["Y"];
            return new Vector2(x, y);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}