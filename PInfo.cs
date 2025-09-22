using HarmonyLib;

namespace LbolDreamStartOfTurnMod
{
    public static class PInfo
    {
        public const string GUID = "rokk.lbol.gameplay.DreamStartOfTurn";
        public const string Name = "DreamStartOfTurn";
        public const string version = "1.0.1";
        public static readonly Harmony harmony = new Harmony(GUID);

    }
}
