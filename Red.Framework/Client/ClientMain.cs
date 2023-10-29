using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpConfig;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using Red.Framework.Client.Misc;

namespace Red.Framework.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected string frameworkName;
        protected Ped PlayerPed = Game.PlayerPed;
        #endregion

        #region Constructor
        public ClientMain()
        {
            ReadConfigFile();
        }
        #endregion

        #region Methods
        private void ReadConfigFile()
        {
            var data = LoadResourceFile(GetCurrentResourceName(), "config.ini");

            if (Configuration.LoadFromString(data).Contains("Framework", "FrameworkName") == true)
            {
                Configuration loaded = Configuration.LoadFromString(data);
                frameworkName = loaded["Framework"]["FrameworkName"].StringValue;
            }
            else
            {
                FrameworkLog.Error($"Config file has not been configured correctly.");
            }
        }
        #endregion

        #region NUI Methods
        #endregion

        #region Event Handlers
        #endregion

        #region Ticks
        #endregion

        #region Discord
        #endregion
    }
}
