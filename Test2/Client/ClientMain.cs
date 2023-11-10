using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Test2.Client
{
    public class ClientMain : BaseScript
    {
        public ClientMain()
        {
            Exports.Add("ExportMessage", ExportMessage);
        }

        public bool ExportMessage(bool isTrue)
        {
            if (isTrue is true)
            {
                Debug.WriteLine("hi");
            }
            else
            {
                Debug.WriteLine("no");
            }

            return isTrue;
        }
    }
}