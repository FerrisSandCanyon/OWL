using System.Collections.Generic;
using System.Threading;
using System;

namespace owl
{
    public static class Globals
    {
        public static Class.ProfileInfo CurrentProfile  = null; // Global ProfileInfo of all the account instances and extra information about the profile
        public static Class.Config      Config          = null; // Global Config class

        public static FormMain hFormMain = null; // Handle to our main form for access and invoking

        public static Thread ParserQueue = null, // Handle to the parser queue thread that manages all the running parser threads
                             hMainThread = null; // Handle to the main Thread

        public static string passKey = "";   // Store the passkey. No intention for deep security other than simple obfuscation to steer away from plain text.
        public const  string Charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"; // Predefined charset

        public static int RunningThreads
        {
            get
            {
                return RunningThreadsCount;
            }

            set
            {
                if (RunningThreads > 0)
                    RunningThreadsCount = RunningThreads;
            }
        }

        private static int RunningThreadsCount = 0; // Number of parser threads that are running

        // Global constant information
        public static class Info
        {
            public const int    vMajor         = 0,
                                vMinor         = 3,
                                vPatch         = 0,
                                vProfileFormat = 1;

            public const string verStr       = "0.3.0",
                                cfgPath      = "./config.json",
                                profilesPath = "./profiles";
        }
    }
}
