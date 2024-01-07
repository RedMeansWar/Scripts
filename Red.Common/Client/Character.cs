using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Red.Common.Client
{
    public class Character // Mainly used for framework dependent scripts.
    {
        public long CharacterId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DoB { get; set; }
        public string Gender { get; set; }
        public int Cash { get; set; }
        public int Bank { get; set; }
        public string Department { get; set; }
    }
}
