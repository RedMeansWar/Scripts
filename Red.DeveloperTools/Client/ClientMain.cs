using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.DeveloperTools.Client
{
    public class ClientMain : BaseScript
    {
        public ClientMain()
        {
            Debug.WriteLine("Hi from Red.DeveloperTools.Client!");
        }

        [Tick]
        public Task OnTick()
        {
            DrawRect(0.5f, 0.5f, 0.5f, 0.5f, 255, 255, 255, 150);

            return Task.FromResult(0);
        }
    }
}