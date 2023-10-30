using System;
using Newtonsoft.Json;

namespace Red.Framework
{
    public class Character
    {
        public long CharacterId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DoB { get; set; }
        public string Gender { get; set; }
        public string Department { get; set; }
    }
}