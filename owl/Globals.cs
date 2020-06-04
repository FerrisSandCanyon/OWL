using System.Collections.Generic;
using System.Threading;
using System;

namespace owl
{
    public static class Globals
    {
        public static Class.ProfileInfo CurrentProfile   = null; // Global ProfileInfo of all the account instances and extra information about the profile
        public static Class.Config      Config           = null; // Global Config class
        public static Class.Account     LastAccountLogin = null; // Stores the account class of the last account logged in to
        public static Class.AppUpdate   AppUpdateInfo    = new Class.AppUpdate();

        public static FormMain hFormMain = null; // Handle to our main form for access and invoking

        public static Thread ParserQueue = null, // Handle to the parser queue thread that manages all the running parser threads
                             hMainThread = null; // Handle to the main Thread

        public static string passKey = "";   // Store the passkey. No intention for deep security other than simple obfuscation to steer away from plain text.
        public static readonly string Charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"; // Predefined charset

        public static bool IsLoggingIn = false;

        // Global constant information
        public static class Info
        {
            public static readonly int    vMajor         = 0,
                                          vMinor         = 6,
                                          vPatch         = 3,
                                          vProfileFormat = 1;

            public static readonly string verStr         = $"{vMajor.ToString()}.{vMinor.ToString()}.{vPatch.ToString()}",
                                          cfgPath        = "./config.json",
                                          profilesPath   = "./profiles",
                                          updateInfoLink = "https://raw.githubusercontent.com/FerrisSandCanyon/OWL/master/update_info.json";
        }
    }
}
