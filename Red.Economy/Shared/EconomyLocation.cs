using CitizenFX.Core;

namespace Red.Economy
{
    public class EconomyLocation
    {
        public int LocationId { get; set; }
        public Vector3 Position { get; set; }
        public int Type { get; set; }
        public bool IsATM { get; set; }
        public bool DisplayBlip { get; set; }
    }
}