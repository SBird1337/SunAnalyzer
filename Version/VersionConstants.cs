namespace SunAnalyzer.Version {
    public static class VersionConstants {
        public const int BROKEN_SEAL_INIT_COUNT = 6;
        public const int LOST_AGE_INIT_COUNT = 7;

        public const int MAP_CODE_BASE_ADDRESS = 0x02008000;

        public static readonly string[] INIT_FUNCTION_NAMES = {"MapCodeInit", "EntranceInit", "ExitInit", "NpcInit", "EventInit", "Init6", "Init7"};
    }
}