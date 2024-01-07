using System;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace Red.SpikesStrips.Server
{
    public class ServerMain : BaseScript
    {
        public ServerMain()
        {
            Debug.WriteLine("Hi from Red.SpikesStrips.Server!");
        }

        [Command("hello_server")]
        public void HelloServer()
        {
            Debug.WriteLine("Sure, hello.");
        }
    }
}