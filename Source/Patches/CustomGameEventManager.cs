using HarmonyLib;
using LBoL.Core;
using LBoL.Core.Battle;
using LbolDreamStartOfTurnMod.BattleActions;

namespace LbolDreamStartOfTurnMod.Patches
{
    [HarmonyPatch(typeof(BattleController), nameof(BattleController.StartPlayerTurn))]
    class BattleControllerPatches
    {
        static void Postfix(BattleController __instance)
        {
            //__instance.ResolveAction(new DreamToHandStartOfTurnAction());
            __instance.React(new Reactor(new DreamToHandStartOfTurnAction()), null, ActionCause.TurnStart);
        }
    }
}