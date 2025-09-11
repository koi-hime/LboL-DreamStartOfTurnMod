using HarmonyLib;

namespace LbolDreamStartOfTurnMod
{
    public static class PInfo
    {
        //Rename the variable below to prevent conflicts between mod.
        public const string GUID = "rokk.lbol.gameplay.DreamStartOfTurn";
        public const string Name = "DreamStartOfTurn";
        public const string version = "0.0.1";
        public static readonly Harmony harmony = new Harmony(GUID);

    }
}
