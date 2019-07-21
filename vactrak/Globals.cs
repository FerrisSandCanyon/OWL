using System.Collections.Generic;

namespace vactrak
{
    public static class Globals
    {
        public static Class.VTConfig                      Config;   // Global VTConfig class
        public static Dictionary<string, Class.VTAccount> Accounts; // Global Dictionary of all the account instances <special id, account class>

        // Global constant information
        public static class Info
        {
            // Version =========================================
            public const int    vMajor = 0,
                                vMinor = 1,
                                vPatch = 0;

            public const string verStr       = "0.1.0",
                                cfgPath      = "./config.json",
                                profilesPath = "./profiles";
        }
    }
}
