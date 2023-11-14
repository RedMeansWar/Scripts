using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;

namespace Red.Radar.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected static Vehicle targetedVehicle;
        protected float vehicleSpeed = targetedVehicle.Speed;
        #endregion

        #region Constructor
        public ClientMain()
        {
            // NUI Callbacks
            RegisterNuiCallback("displayNUI", new Action<IDictionary<string, object>, CallbackDelegate>(OpenUI));
        }
        #endregion

        #region Methods
        private void GetVehicleSpeed()
        {
        }
        #endregion

        #region NUI Methods
        private void OpenUI(IDictionary<string, object> data, CallbackDelegate result)
        {
            SetNuiFocus(true, true);

            SendNuiMessage(Json.Stringify(new
            {
                type = "DISPLAY_NUI"
            }));
        }
        #endregion
    }
}