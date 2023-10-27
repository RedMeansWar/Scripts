using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.CalmAI.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected uint player = Game.GenerateHashASCII("PLAYER");
        protected Ped PlayerPed = Game.PlayerPed;

        protected readonly IReadOnlyList<string> scenarioTypes = new List<string>
        {
            "WORLD_VEHICLE_MILITARY_PLANES_SMALL", "WORLD_VEHICLE_MILITARY_PLANES_BIG", "WORLD_VEHICLE_AMBULANCE", "WORLD_VEHICLE_POLICE_NEXT_TO_CAR", "WORLD_VEHICLE_POLICE_CAR", "WORLD_VEHICLE_POLICE_BIKE", "WORLD_VEHICLE_DRIVE_PASSENGERS_LIMITED"
        };

        protected readonly IReadOnlyList<string> scenarioGroups = new List<string>
        {
            "MP_POLICE", "ARMY_HELI", "POLICE_POUND1", "POLICE_POUND2", "POLICE_POUND3", "POLICE_POUND4", "POLICE_POUND5", "SANDY_PLANES", "ALAMO_PLANES", "GRAPESEED_PLANES", "LSA_PLANES", "NG_PLANES"
        };

        protected readonly IReadOnlyList<string> relationshipGroups = new List<string>
        {
            "AMBIENT_GANG_HILLBILLY", "AMBIENT_GANG_BALLAS", "AMBIENT_GANG_MEXICAN", "AMBIENT_GANG_FAMILY", "AMBIENT_GANG_MARABUNTE", "AMBIENT_GANG_SALVA", "GANG_1", "GANG_2", "GANG_9", "GANG_10", "FIREMAN", "MEDIC", "COP"
        };

        protected readonly IReadOnlyList<string> animalRelationGroups = new List<string>()
        {
            "HEN", "WILD_ANIMAL", "SHARK", "COUGAR", "GUARD_DOG", ""
        };

        protected readonly IReadOnlyList<string> suppressedModels = new List<string>
        {
            "police", "police2", "police3", "police4", "policeb", "policeold1", "policeold2", "policet", "polmav", "pranger", "sheriff", "sheriff2", "stockade3", "buffalo3", "fbi", "fbi2", "firetruk", "lguard", "ambulance", "riot", "shamal", "luxor", "luxor2", "jet", "lazer", "titan", "barracks", "barracks2", "crusader", "rhino", "airtug", "ripley", "cargobob", "cargobob2", "cargobob3", "cargobob4", "cargobob5", "buzzard", "besra", "volatus"
        };
        #endregion

        #region Constructor
        public ClientMain()
        {

            foreach (string scenarioType in scenarioTypes)
            {
                SetScenarioTypeEnabled(scenarioType, false);
            }

            foreach (string scenarioGroup in scenarioGroups)
            {
                SetScenarioGroupEnabled(scenarioGroup, false);
            }

            foreach (string relationshipGroup in relationshipGroups)
            {
                SetRelationshipBetweenGroups(1, Game.GenerateHashASCII(relationshipGroup), (uint)player);
            }

            foreach (string suppressedModel in suppressedModels)
            {
                SetVehicleModelIsSuppressed(Game.GenerateHashASCII(suppressedModel), true);
            }

            for (int i = 0; i < 15; i++)
            {
                EnableDispatchService(i, false);
            }
        }
        #endregion

        #region Ticks
        [Tick]
        private async Task ControlAiTick()
        {
            foreach (Ped ped in World.GetAllPeds())
            {
                ped.BlockPermanentEvents = true;
            }

            DistantCopCarSirens(false);

            SetPoliceIgnorePlayer(PlayerPed.Handle, true);
            SetEveryoneIgnorePlayer(PlayerPed.Handle, true);
            SetIgnoreLowPriorityShockingEvents(PlayerPed.Handle, true);

            SetBlockingOfNonTemporaryEvents(PlayerPed.Handle, true);
            SetPlayerCanBeHassledByGangs(PlayerPed.Handle, false);

            SetPlayerWantedLevel(PlayerId(), 0, false);
            SetPlayerWantedLevelNow(PlayerId(), false);
            SetPlayerWantedLevelNoDrop(PlayerId(), 0, false);
            SetMaxWantedLevel(0);

            await Delay(250);
        }
        #endregion
    }
}