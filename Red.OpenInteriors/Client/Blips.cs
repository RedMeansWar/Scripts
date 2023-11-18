using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Red.OpenInteriors.Client
{
    internal class Blips
    {
        public string Name { get; set; }
        public BlipColor BlipColor { get; set; }
        public BlipSprite BlipSprite { get; set; }
        public Vector3 InteriorCoords { get; set; }

        public Blips(string name, BlipColor color, BlipSprite sprite, Vector3 interiorCoords)
        {
            Name = name;
            BlipColor = color;
            BlipSprite = sprite;
            InteriorCoords = interiorCoords;
        }
    }
}
