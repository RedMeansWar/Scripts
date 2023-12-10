using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Diagnostics.Log;
using static Red.Common.Client.Hud.HUD;

namespace Red.NearestPostal.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected readonly List<Postal> postals;
        protected string routedPostalCode, closestPostal;
        protected Blip blip;
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

                    for (int i = 0; i < postals.Count; i++)
                    {
                        postals[i] = new Postal(postals[i].PostalCode, postals[i].PostalLocation.X, postals[i].PostalLocation.Y);
                    }

                    Info($"Loaded {postals.Count} postal(s)");
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            Tick += DisplayPostalTick;
            Tick += UpdatePostalTick;

            Exports.Add("GetClosestPostal", GetClosestPostal);
        }
        #endregion

        #region Commands
        [Command("p")]
        private void PCommand(string[] args) => PostalCommand(args);

        [Command("postal")]
        private void PostalCommand(string[] args)
        {
            if (args.Length < 1 && blip is not null)
            {

                DisplayNotification($"~d~~h~Postals~h~~s~: Removed GPS Route for postal {routedPostalCode}.", true);
                blip.Delete();
                blip = null;
                routedPostalCode = "000";
                return;
            }

            if (args.Length < 1)
            {
                DisplayNotification($"~d~~h~Postals~h~~s~: You have no GPS route to remove.", true);
                return;
            }

            string input = args[0].ToUpper();
            Postal foundPostal = postals.FirstOrDefault(postal => input == postal.PostalCode);

            if (foundPostal is not null)
            {
                blip?.Delete();

                blip = World.CreateBlip(new(foundPostal.PostalLocation.X, foundPostal.PostalLocation.Y, 0f));
                blip.ShowRoute = true;
                blip.Color = (BlipColor)29;
                SetBlipRouteColour(blip.Handle, 29);
                blip.Name = $"Postal {foundPostal.PostalCode}";

                DisplayNotification($"~d~~h~Postals~h~~s~: You've programmed your GPS to postal {foundPostal.PostalCode}.", true);
            }
            else
            {
                DisplayNotification($"~d~~h~Postals~h~~s~: {input} Doesn't seem to exist.", true);
            }
        }
        #endregion

        #region Methods
        public string GetClosestPostal(Vector3 position)
        {
            Dictionary<string, float> results = new();

            foreach (Postal postal in postals)
            {
                float dist = Vector3.Distance((Vector3)postal.PostalLocation, position);

                if (!results.ContainsKey(postal.PostalCode))
                {
                    results.Add(postal.PostalCode, dist);
                }
            }

            return results.OrderBy(pair => pair.Value).First().Key;
        }
        #endregion

        #region Ticks
        private async Task DisplayPostalTick()
        {
            closestPostal = GetClosestPostal(Game.PlayerPed.Position);
            await Delay(1000);
        }

        private async Task UpdatePostalTick()
        {
            if (blip is not null && Vector2.DistanceSquared(new(Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y), new(blip.Position.X, blip.Position.Y)) < 5000f)
            {
                DisplayNotification("~d~~h~Postals~h~~s~: You've arrived at the postal.");
                blip.Delete();
                blip = null;
            }

            if (!HUDIsVisable)
            {
                await Delay(1000);
                return;
            }

            DrawText2d(1.133f, -0.044f, 0.4f, $"Nearest Postal: ~c~{closestPostal}", 255, 255, 255, 255);
        }
        #endregion
    }

    #region Classes
    public class Postal
    {
        public string PostalCode { get; set; }
        public Vector2 PostalLocation { get; set; }

        public Postal(string code, float x, float y)
        {
            PostalCode = code;
            PostalLocation = new(x, y);
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
            float y = (float)jObject["Y"];
            return new Vector2(x, y);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}