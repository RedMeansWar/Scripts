using System;
using System.Collections.Generic;
using System.Linq;
using Red.Common.Server;
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

        [EventHandler("Menu:Server:getAllSpeedzones")]
        private void OnGetAllSpeedzones([FromSource] Player player)
        {
            if (speedzones.Count > 0)
            {
                player.TriggerEvent("Menu:Client:updateSpeedzones", Json.Stringify(speedzones));
            }
        }

        [EventHandler("Menu:Server:createSpeedzone")]
        private void OnSpeedzoneCreated([FromSource] Player player, Vector3 zonePos, int zoneRadius, float zoneSpeed)
        {
            if (speedzones.Any(zone => Vector3.Distance(zone.Position, zonePos) <= zone.Radius))
            {
                player.TriggerEvent("Menu:Client:showClientNotification", "~r~~h~Error~h~~s~: There is already a speedzone near you.");
            }
            else
            {
                speedzones.Add(new Speedzone { Position = zonePos, Radius = zoneRadius, Speed = zoneSpeed, ServerId = int.Parse(player.Handle) });
                TriggerClientEvent("Menu:Client:updateSpeedzones", Json.Stringify(speedzones));
                player.TriggerEvent("Menu:Client:showClientNotification", "~g~~h~Success~h~~s~: Speedzone created.");
            }
        }

        [EventHandler("Menu:Server:deleteSpeedzone")]
        private void OnSpeedzoneDeleted([FromSource] Player player, Vector3 playerPos)
        {
            Speedzone closestZone = null;
            float closestDistance = speedZoneRadiuses.Last() + 1;

            foreach (Speedzone zone in speedzones)
            {
                float distance = Vector3.Distance(playerPos, zone.Position);

                if (distance <= zone.Radius && distance < closestDistance)
                {
                    closestZone = zone;
                    closestDistance = distance;
                }
            }

            if (closestZone is not null)
            {
                speedzones.Remove(closestZone);
                TriggerClientEvent("Menu:Client:updateSpeedzones", Json.Stringify(speedzones));
                player.TriggerEvent("Menu:Client:showClientNotification", "~g~~h~Success~h~~s~: Speedzone deleted.");
            }
            else
            {
                player.TriggerEvent("Menu:Client:showClientNotification", "~r~~h~Error~h~~s~: You must be inside the speedzone you wish to delete.");
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