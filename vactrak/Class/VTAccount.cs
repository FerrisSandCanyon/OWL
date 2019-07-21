using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vactrak.Class
{
    public class VTAccount
    {
        public string SteamURL, Username, Password, Note;
        public bool   Banned;
        public ulong  CooldownDelta;
    }
}
