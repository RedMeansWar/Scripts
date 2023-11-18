using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharpConfig;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;
using static Red.Common.Client.Hud.HUD;
using static Red.Common.Client.Diagnostics.Log;

namespace Red.Breathalyzer.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected double bacLimit;
        protected bool startBac, displayNUI, bacBeenSet;
        #endregion

        #region Constructor
        public ClientMain()
        {
            ReadConfigFile();
        }
        #endregion

        #region Commands
        #endregion

        #region NUI Callbacks
        #endregion

        #region Methods
        private void ReadConfigFile()
        {
            var data = LoadResourceFile(GetCurrentResourceName(), "config.ini");

            if (Configuration.LoadFromString(data).Contains("Breathalyzer", "LegalLimit") == true)
            {
                Configuration loaded = Configuration.LoadFromString(data);

                bacLimit = loaded["Breathalyzer"]["LegalLimit"].DoubleValue;
            }
            else
            {
                Debug($"[Breathalyzer]: Config file has not been configured correctly.");
            }
        }
        #endregion

        #region Event Handlers
        #endregion

        #region Ticks
        #endregion
    }
}