using HarmonyLib;
using LBoL.Core;
using LBoL.Core.Battle;
using DreamStartOfTurnMod_TopDeck.Source.BattleActions;

namespace DreamStartOfTurnMod_TopDeck.Source.Patches
{
    [HarmonyPatch(typeof(BattleController), nameof(BattleController.StartPlayerTurn))]
    class BattleControllerPatches
    {
        static void Postfix(BattleController __instance)
        {
            __instance.React(new Reactor(new DreamToTopDeckStacked()), null, ActionCause.TurnStart);
        }
    }
}
