using System;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.InteractionMenu.Server
{
    public class ServerMain : BaseScript
    {
        #region Variables
        protected readonly List<Speedzone> speedzones = new();

        protected readonly static List<int> speedZoneRadiuses = new()
        {
            25, 50, 75, 100
        };

        #endregion

        #region Event Handlers
        [EventHandler("playerDropped")]
        private void OnPlayerDropped([FromSource] Player player)
        {
            Speedzone playerZone = speedzones.FirstOrDefault(sz => sz.ServerId == int.Parse(player.Handle));

            if (playerZone is not null)
            {
                speedzones.Remove(playerZone);
                TriggerClientEvent("Menu:Client:updateSpeedzones", Json.Stringify(speedzones));
            }
        }

        [EventHandler("Menu:Server:getSpeedzones")]
        internal void OnGetAllData([FromSource] Player player)
        {
            Debug.WriteLine($"Loaded {speedzones.Count} speedzone(s)");

            if (speedzones.Count > 0)
            {
                player.TriggerEvent("Menu:Client:updateSpeedzones", Json.Stringify(speedzones));
            }
        }

        [EventHandler("Menu:Server:createSpeedzone")]
        internal void OnZoneCreated([FromSource] Player player, Vector3 zoneLoc, int zoneRadius, float zoneSpeed)
        {
            Speedzone playerZone = speedzones.FirstOrDefault(sz => sz.ServerId == int.Parse(player.Handle));

            if (speedzones.Any(sz => Vector3.Distance(new Vector3(sz.Position.X, sz.Position.Y, sz.Position.Z), zoneLoc) <= sz.Radius))
            {
                player.TriggerEvent("Menu:Client:showClientNotification", "~r~There is already a speed limit set for this location.");
            }
            else if (playerZone != null)
            {
                player.TriggerEvent("Menu:Client:showClientNotification", $"~r~You already have a speed zone placed.");
            }
            else
            {
                speedzones.Add(new Speedzone { Position = zoneLoc, Radius = zoneRadius, Speed = zoneSpeed, ServerId = int.Parse(player.Handle) });

                Debug.WriteLine($"Player: {player.Name} created a speedzone at: {new Vector3(player.Character.Position.X, player.Character.Position.Y, player.Character.Position.Z)}");
                player.TriggerEvent("Menu:Client:showClientNotification", $"~g~Speedzone created.");

                TriggerLatentClientEvent("Menu:Client:updateSpeedzones", 5000, Json.Stringify(speedzones));
            }
        }

        [EventHandler("Menu:Server:deleteSpeedzone")]
        internal void OnZoneDeleted([FromSource] Player player, Vector3 playerPos)
        {
            Speedzone closestZone = null;
            float closestDistance = speedZoneRadiuses.Last() + 1;

            foreach (Speedzone sz in speedzones)
            {
                float distance = Vector3.Distance(playerPos, new Vector3(sz.Position.X, sz.Position.Y, sz.Position.Z));

                if (distance <= sz.Radius && distance < closestDistance)
                {
                    closestZone = sz;
                    closestDistance = distance;
                }
            }

            if (closestZone != null)
            {
                speedzones.Remove(closestZone);
                TriggerLatentClientEvent("Menu:Client:updateSpeedzones", 5000, Json.Stringify(speedzones));
                player.TriggerEvent("Menu:Client:showClientNotification", $"~g~Speedzone Deleted.");
                Debug.WriteLine($"Player: {player.Name} deleted a speedzone.");
            }
            else
            {
                player.TriggerEvent("Menu:Client:showClientNotification", $"~r~You are not inside any active speed zones.");
            }
        }

        [EventHandler("Menu:Server:deleteProp")]
        private void OnDeleteProp(int netId)
        {
            Entity prop = Entity.FromNetworkId(netId);

            if (prop is null)
            {
                return;
            }

            DeleteEntity(prop.Handle);
        }
        #endregion
    }
}