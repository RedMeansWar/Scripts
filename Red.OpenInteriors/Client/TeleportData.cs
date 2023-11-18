using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Red.OpenInteriors.Client
{
    internal class TeleportData
    {
        public string text;
        public List<int> dest = new();
        public Vector3 coord;
        public float h;
        public bool veh;
    }
}
