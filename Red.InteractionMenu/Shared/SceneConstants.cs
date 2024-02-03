using CitizenFX.Core;

namespace Red.InteractionMenu
{
    public class SceneProp
    {
        public string Name { get; set; }
        public string Model { get; set; }
        public Vector3 Position { get; set; }
        public float Heading { get; set; }
    }

    public class SpeedZone
    {
        public Vector3 Position { get; set; }
        public int Radius { get; set; }
        public float Speed { get; set; }
        public int Blip { get; set; }
        public int Zone { get; set; }
        public int ServerId { get; set; }
    }
}