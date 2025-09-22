using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LbolDreamStartOfTurnMod.BattleActions
{
    public sealed class DreamToHandStartOfTurnAction : SimpleAction
    {
        // Copypasted mostly from LBoL.Core.Battle.BattleActions.DreamCardsToHandAction,
        // but without the part where it removes the Dream keyword from all cards after choosing.
        public override IEnumerable<Phase> GetPhases()
        {
            List<Card> list = base.Battle.DrawZone
                .Union(base.Battle.DiscardZone)
                .Where((Card card) => card.IsDreamCard)
                .ToList<Card>();

            if (list.Count > 0)
            {
                SelectCardInteraction interaction = new SelectCardInteraction(0, 1, list, SelectedCardHandling.DoNothing)
                {
                    Description = "SelectCard.DreamCardsToHand".Localize(true)
                };
                yield return base.CreatePhase("Select", delegate
                {
                    this.React(new InteractionAction(interaction, false), null, null);
                }, false);
                IReadOnlyList<Card> selected = interaction.SelectedCards;
                if (selected.Count > 0)
                {
                    yield return base.CreatePhase("MoveToHand", delegate
                    {
                        foreach (Card card3 in selected)
                        {
                            this.React(new MoveCardAction(card3, CardZone.Hand), null, null);
                        }
                    }, false);
                }
            }
            yield break;
        }
    }
}
