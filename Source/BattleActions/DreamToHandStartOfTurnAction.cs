using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.CompilerServices;

using LBoL.Base;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;

using YamlDotNet.Serialization.Schemas;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace DreamStartOfTurnMod_TopDeck.Source.BattleActions
{
    /// <summary>
    /// The start of turn action that selects one Dream card from
    /// discard to place on top of the deck. Occurs after draw step,
    /// (which is also after any abilities that play at the start of the turn,
    /// like Awakened's True Form, which plays the top card of the draw pile
    /// at the start of the turn).
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
            List<Card> cardsWithDreamEffectAppliedInDrawAndDiscard = [.. Battle.DrawZone
                .Union(Battle.DiscardZone)
                .Where(card => card.IsDreamCard)];

            var hasCardsWithDreamEffect = cardsWithDreamEffectAppliedInDrawAndDiscard.Count > 0;
            if (hasCardsWithDreamEffect)
            {
                // cards in the players hand that can draw any amount
                // this is useful to see what dream card to pick for top deck setup
                // if no draw cards avaliable, then it changes what dream card to stack on top.
                var drawCardsInHand = Battle.HandZone.Where(HasPotentialDrawAction).ToList().AsReadOnly();
                // apparently the Dream keyword is not set on cards in the hand
                // !!! `Dream` is an entirely different keyword than `DreamCard` so keep this in mind
                // !!! `Dream` *should* be on the card but actually `Dream` is a relative effect, so we
                // !!! need to check the card's config for the relative keyword instead of checking
                // !!! the card's keywords directly; i'm guessing it's done this way because `Dream`
                // !!! can be upgraded to be of a higher `Dream` value since it is `Dream X`
                // !!! btw, RelativeKeyword is a single string, but the one string can contain multiple keywords!!!!
                // !!! WHYYYYYYYYY
                // var dreamCardsInHand = Battle.HandZoneAndPlayArea.Where(HasDreamAction).ToList().AsReadOnly();
                // var dreamCardsInHand = Battle.HandZoneAndPlayArea.Where(card => card.ConfigRelativeKeywords == Keyword.Dream).ToList().AsReadOnly();
                var dreamCardsInHand = Battle.HandZone.Where(card => card.ConfigRelativeKeywords.HasFlag(Keyword.Dream)).ToList().AsReadOnly();
                var followUpCardsInDrawPile = Battle.DrawZone.Where(card => card.HasKeyword(Keyword.FollowCard)).ToList().AsReadOnly();

                Console.WriteLine($"Cards in Hand:\n{string.Join(
                    $"{Environment.NewLine}{Environment.NewLine}",
                    Battle.HandZone.Select(card =>
                        $"{card.Name}:{Environment.NewLine}" +
                        $"- Keywords: {string.Join(", ", card.Keywords)}{Environment.NewLine}" +
                        $"- ConfigRelativeKeywords: {string.Join(", ", card.ConfigRelativeKeywords)}{Environment.NewLine}" +
                        $"- RelativeKeyword: {string.Join(", ", card.Config.RelativeKeyword)}{Environment.NewLine}" +
                        $"- UpgradedRelativeKeyword: {string.Join(", ", card.Config.UpgradedRelativeKeyword)}"))}");

                var drawCardNames = string.Join(", ", drawCardsInHand.Select(GetFirstWordFromName()));
                var dreamCardNames = string.Join(", ", dreamCardsInHand.Select(GetFirstWordFromName()));
                var followUpCardNames = string.Join(", ", followUpCardsInDrawPile.Select(GetFirstWordFromName()));

                var drawString = drawCardNames.Any() ? $"Draw-Hand: {drawCardsInHand.Count}; {drawCardNames}" : "";
                var dreamString = dreamCardNames.Any() ? $" || Dream-Hand: {dreamCardsInHand.Count}; {dreamCardNames}" : "";
                var followUpString = followUpCardNames.Any() ? $" || FollowUp-Draw: {followUpCardsInDrawPile.Count}; {followUpCardNames}" : "";

                var dreamCardSelectionDescription =
                    $"Dream Phase: Select Dream Card to stack.\n{drawString}{dreamString}{followUpString}";

                // create Dream top deckcard selection interaction
                // allowing the player to select up to 1 Dream card to place on top of their deck
                SelectCardInteraction selectDreamCardsInteraction = new(0, 1, cardsWithDreamEffectAppliedInDrawAndDiscard, SelectedCardHandling.DoNothing)
                {
                    // Description = "SelectCard.DreamCardsToHand".Localize(true)
                    // !!! make this a localized string in the future, but for now just hardcode it
                    // still need to figure out how to add a custom localized string
                    // to be able to use for this card interaction UI `key: SelectCard.DreamCardsToTopOfDeck`
                    Description = dreamCardSelectionDescription
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

            static Func<Card, string> GetFirstWordFromName()
            {
                return card => card.Name.Split(" ").First();
            }
        }

        private static bool HasPotentialDrawAction(Card card)
        {
            var actionsMethod = card.GetType().GetMethod(
                "Actions",
                BindingFlags.Instance | BindingFlags.NonPublic,
                binder: null,
                types: [typeof(UnitSelector), typeof(ManaGroup), typeof(Interaction)],
                modifiers: null);

            var stateMachineType = actionsMethod?
                .GetCustomAttribute<IteratorStateMachineAttribute>()?
                .StateMachineType;

            if (stateMachineType == null)
                return false;

            using var module = ModuleDefinition.ReadModule(card.GetType().Assembly.Location);

            var stateMachineName = stateMachineType.FullName.Replace('+', '/');
            var stateMachine = FindTypes(module.Types)
                .FirstOrDefault(type => type.FullName == stateMachineName);

            var moveNext = stateMachine?.Methods
                .FirstOrDefault(method => method.Name == "MoveNext" && method.HasBody);

            if (moveNext == null)
                return false;

            return moveNext.Body.Instructions.Any(instruction =>
                instruction.OpCode == OpCodes.Newobj &&
                instruction.Operand is MethodReference constructor &&
                (constructor.DeclaringType.FullName == typeof(DrawManyCardAction).FullName ||
                 constructor.DeclaringType.FullName == typeof(DrawCardsToSpecificAction).FullName));
        }

        private static bool HasDreamAction(Card card)
        {
            var actionsMethod = card.GetType().GetMethod(
                "Actions",
                BindingFlags.Instance | BindingFlags.NonPublic,
                binder: null,
                types: [typeof(UnitSelector), typeof(ManaGroup), typeof(Interaction)],
                modifiers: null);

            var stateMachineType = actionsMethod?
                .GetCustomAttribute<IteratorStateMachineAttribute>()?
                .StateMachineType;

            if (stateMachineType == null)
                return false;

            using var module = ModuleDefinition.ReadModule(card.GetType().Assembly.Location);

            var stateMachineName = stateMachineType.FullName.Replace('+', '/');
            var stateMachine = FindTypes(module.Types)
                .FirstOrDefault(type => type.FullName == stateMachineName);

            var moveNext = stateMachine?.Methods
                .FirstOrDefault(method => method.Name == "MoveNext" && method.HasBody);

            if (moveNext == null)
                return false;

            return moveNext.Body.Instructions.Any(instruction =>
                instruction.OpCode == OpCodes.Newobj &&
                instruction.Operand is MethodReference constructor &&
                (constructor.DeclaringType.FullName == typeof(DreamCardsAction).FullName));
        }

        private static IEnumerable<TypeDefinition> FindTypes(
            IEnumerable<TypeDefinition> types)
        {
            foreach (var type in types)
            {
                yield return type;

                foreach (var nestedType in FindTypes(type.NestedTypes))
                    yield return nestedType;
            }
        }
    }
}
