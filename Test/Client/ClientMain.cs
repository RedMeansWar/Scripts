using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Test.Client
{
    public class ClientMain : BaseScript
    {
        [Command("test")]
        private void CommandTest()
        {
            Exports["test2"].ExportMessage(true);
        }
    }
}