using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YamlDotNet.Serialization.Schemas;

namespace DreamStartOfTurnMod_TopDeck.Source.BattleActions
{
    /// <summary>
    /// The start of turn action that moves all Dream cards from 
    /// the Draw and Discard zones to the player's hand.
    /// </summary>
    public sealed class DreamToTopDeckStacked : SimpleAction
    {
        /// <summary>
        /// Copypasted mostly from LBoL.Core.Battle.BattleActions.DreamCardsToHandAction,
        /// but without the part where it removes the Dream keyword from all cards after choosing.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<Phase> GetPhases()
        {

            // Apparently Dream cards can not only be in the discard pile, but also in the draw pile.
            // So we need to check both zones.
            List<Card> dreamCardsInDrawAndDiscard = Battle.DrawZone
                .Union(Battle.DiscardZone)
                .Where(card => card.IsDreamCard)
                .ToList();

            var hasDreamCards = dreamCardsInDrawAndDiscard.Count > 0;
            if (hasDreamCards)
            {
                // create Dream card selection interaction
                // allowing the player to select up to 1 Dream card to move to their hand
                SelectCardInteraction selectDreamCardsInteraction = new SelectCardInteraction(0, 1, dreamCardsInDrawAndDiscard, SelectedCardHandling.DoNothing)
                {
                    Description = "SelectCard.DreamCardsToHand".Localize(true)
                };

                yield return base.CreatePhase("Select", delegate
                {
                    this.React(new InteractionAction(selectDreamCardsInteraction, false));
                }, false);

                // if the player selected a Dream card, this current version will move it to their hand
                // the planned intent of this adjustment is to change this to place the card on
                // top of the deck instead of moving it to the hand
                IReadOnlyList<Card> selectedDreamCard = selectDreamCardsInteraction.SelectedCards;
                if (selectedDreamCard.Count > 0)
                {
                    yield return base.CreatePhase("MoveToTopOfDeck", delegate
                    {
                        foreach (Card currentDreamCard in selectedDreamCard)
                        {
                            this.React(new MoveCardToDrawZoneAction(currentDreamCard, DrawZoneTarget.Top));

                            // set the chosen card's dream card status to false so that
                            // it is no longer a Dream card when it is stacked on top of
                            // the deck
                            currentDreamCard.IsDreamCard = false;
                        }
                    }, false);
                }
            }
            yield break;
        }
    }
}
