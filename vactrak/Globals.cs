using System.Collections.Generic;

namespace vactrak
{
    public static class Globals
    {
        public static Class.VTConfig                      Config         = null;    // Global VTConfig class
        public static Dictionary<string, Class.VTAccount> CurrentProfile = null;    // Global Dictionary of all the account instances <special id, account class>
        
        public static string                              passKey        = "";      // Store the passkey

        // I know that bad actors can simply take a peek at the memory and find the user's password but...
        // It's not really *THAT* unconventional since the whole encryption is for keeping the json profiles away from plain text and the whole hashing is just for verification that the password is indeed correct.
        

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
