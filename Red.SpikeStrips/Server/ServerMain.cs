using System;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace Red.SpikeStrips.Server
{
    public class ServerMain : BaseScript
    {
        public ServerMain()
        {
            Debug.WriteLine("Hi from Red.SpikeStrips.Server!");
        }

        [Command("hello_server")]
        public void HelloServer()
        {
            Debug.WriteLine("Sure, hello.");
        }
    }
}