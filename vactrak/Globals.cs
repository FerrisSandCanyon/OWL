namespace vactrak
{
    public static class Globals
    {
        // Global VTConfig class
        public static Class.VTConfig Config;

        // Global constant information
        public static class Info
        {
            // Version ========================
            public const int    vMajor = 0,
                                vMinor = 1,
                                vPatch = 0;

            public const string verStr  = "0.1.0",
                                cfgPath = "./config.json";
        }
    }
}
