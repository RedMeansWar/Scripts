using System;
using SharpConfig;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using System.Collections.Generic;

namespace Red.Breathalyzer.Server
{
    public class ServerMain : BaseScript
    {
        #region Variables
        protected static double limit;
        #endregion

        #region Methods
        private void ReadConfigFile()
        {
            var data = LoadResourceFile(GetCurrentResourceName(), "config.ini");

            if (Configuration.LoadFromString(data).Contains("Breathalyzer", "LegalLimit") == true)
            {
                Configuration loaded = Configuration.LoadFromString(data);

                limit = loaded["Breathalyzer"]["LegalLimit"].DoubleValue;
            }
            else
            {
                Debug.WriteLine($"[Breathalyzer]: Config file has not been configured correctly.");
            }
        }
        #endregion
    }
}