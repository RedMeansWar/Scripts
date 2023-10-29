using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.Common;
using Dapper;
using static CitizenFX.Core.Native.API;
using MySql.Data.MySqlClient;
using SharpConfig;

namespace Red.Framework.Server.Misc
{
    public class Helper
    {
        #region Variables
        #endregion

        #region Methods
        public static string GetPlayerLicense(int player)
        {
            return "";
        }


        public static int PlayerId()
        {
            return 1;
        }
        #endregion
    }
}
