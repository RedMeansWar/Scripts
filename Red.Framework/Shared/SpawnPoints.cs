using CitizenFX.Core;

namespace Red.Framework
{
    public class SpawnPoint
    {
        public string SpawnName { get; set; }
        public Vector3 Location { get; set; }
        public float Heading { get; set; }

        public SpawnPoint(string spawnName, Vector3 location, float heading)
        {
            SpawnName = spawnName;
            Location = location;
            Heading = heading;
        } 
    }
}