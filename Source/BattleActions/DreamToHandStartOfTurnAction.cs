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

using LBoL.EntityLib.Cards.Character.Sakuya;
using LBoL.EntityLib.Cards.Character.Koishi;


using YamlDotNet.Serialization.Schemas;
using Mono.Cecil;
using Mono.Cecil.Cil;
using LBoL.Base.Extensions;

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
                // todo: check if the draw card has a swap draw/discard effect that happens first
                // because that would change what dream card to pick for top deck setup
                // alternatively, we can just check if the card is specifically BackToFuture, but that would be a hardcoded solution and not a general one.
                var drawCardsInHand = Battle.HandZone.Where(HasPotentialDrawAction).ToList();
                var cardsByType = drawCardsInHand.ToLookup(
                    card => card is BackToFuture);

                List<Card> backToFutureCardsInHand = [.. cardsByType[true]];
                drawCardsInHand = [.. cardsByType[false]];
                // we want to exclude KoishiPlayDiscard from the list of cards that can be played on 
                // top of the deck, because its Actions method plays the card from the discard pile.
                var playOnTopKoishiCardsInHand = Battle.HandZone.Where(HasPlayCardOnTop).Where(card => card is not KoishiPlayDiscard).ToList();

                // apparently the Dream keyword is not set on cards in the hand
                // !!! `Dream` is an entirely different keyword than `DreamCard` so keep this in mind
                // !!! `Dream` *should* be on the card but actually `Dream` is a relative effect, so we
                // !!! need to check the card's config for the relative keyword instead of checking
                // !!! the card's keywords directly; i'm guessing it's done this way because `Dream`
                // !!! can be upgraded to be of a higher `Dream` value since it is `Dream X`
                // !!! btw, RelativeKeyword is a single string, but the one string can contain multiple keywords!!!!
                // !!! WHYYYYYYYYY
                var dreamCardsInHand = Battle.HandZone.Where(card => card.ConfigRelativeKeywords.HasFlag(Keyword.Dream)).ToList().AsReadOnly();

                var followUpCardsInDrawPile = Battle.DrawZone.Where(card => card.HasKeyword(Keyword.FollowCard)).ToList().AsReadOnly();

                var followUpTriggersCardsInHand = Battle.HandZone
                    .Where(card => card.ConfigRelativeKeywords.HasFlag(Keyword.FollowAttack))
                    .ToList();

                Console.WriteLine($"\nCards in Hand:\n{string.Join(
                    $"{Environment.NewLine}{Environment.NewLine}",
                    Battle.HandZone.Select(card =>
                        $"{card.Name}:{Environment.NewLine}" +
                        $"- Keywords: {string.Join(", ", card.Keywords)}{Environment.NewLine}" +
                        $"- ConfigRelativeKeywords: {string.Join(", ", card.ConfigRelativeKeywords)}{Environment.NewLine}" +
                        $"- RelativeKeyword: {string.Join(", ", card.Config.RelativeKeyword)}{Environment.NewLine}" +
                        $"- UpgradedRelativeKeyword: {string.Join(", ", card.Config.UpgradedRelativeKeyword)}"))}");

                var drawCardNames = FormatNames(drawCardsInHand);
                var playOnTopNames = FormatNames(playOnTopKoishiCardsInHand);
                var dreamCardNames = FormatNames(dreamCardsInHand);
                var followUpCardNames = FormatNames(followUpCardsInDrawPile);
                var followUpTriggersCardNames = FormatNames(followUpTriggersCardsInHand);

                var drawString = drawCardNames.Any() ? $"Dra-Hnd: {drawCardsInHand.Count}; {drawCardNames}" : "";
                var playOnTopString = playOnTopNames.Any() ? $"PlayTop-Hnd: {playOnTopKoishiCardsInHand.Count}; {playOnTopNames}" : "";
                var dreamString = dreamCardNames.Any() ? $"Dr-Hnd: {dreamCardsInHand.Count}; {dreamCardNames}" : "";
                var followUpString = followUpCardNames.Any() ? $"FolUp-Dra: {followUpCardsInDrawPile.Count}; {followUpCardNames}" : "";
                var followUpTriggersString = followUpTriggersCardNames.Any() ? $"FolUpTrig-Hnd: {followUpTriggersCardsInHand.Count}; {followUpTriggersCardNames}" : "";

                var sections = new[]
                {
                    drawString,
                    playOnTopString,
                    dreamString,
                    followUpString,
                    followUpTriggersString
                }.Where(section => !string.IsNullOrEmpty(section));

                var joinedSections = string.Concat(sections.Select((section, index) =>
                    (index == 0 ? "" : index % 2 == 1 ? " \\ " : " \\ ") + section));

                var dreamCardSelectionDescription =
                    $"Dream Phase: Select Dream Card to stack.\n{joinedSections}";

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

            static string FormatNames(IEnumerable<Card> cards)
            {
                return string.Join(", ", cards
                    .Select(card => card.Name.Split(' ')[0])
                    .GroupBy(name => name)
                    .Select(group => group.Count() == 1
                        ? group.Key
                        : $"{group.Key}-x{group.Count()}"));
            }
        }

        private static bool HasPotentialDrawAction(Card card)
        {
            (bool flowControl, bool value) = SetupForReadingActionsMethod(card, out ModuleDefinition module, out MethodDefinition moveNext);
            if (!flowControl)
            {
                return value;
            }

            return moveNext.Body.Instructions.Any(instruction =>
                instruction.OpCode == OpCodes.Newobj &&
                instruction.Operand is MethodReference constructor &&
                (constructor.DeclaringType.FullName == typeof(DrawManyCardAction).FullName ||
                 constructor.DeclaringType.FullName == typeof(DrawCardsToSpecificAction).FullName));
        }

        private static bool HasPlayCardOnTop(Card card)
        {
            (bool flowControl, bool value) = SetupForReadingActionsMethod(card, out ModuleDefinition module, out MethodDefinition moveNext);
            if (!flowControl)
            {
                return value;
            }

            return moveNext.Body.Instructions.Any(instruction =>
                instruction.OpCode == OpCodes.Newobj &&
                instruction.Operand is MethodReference constructor &&
                (constructor.DeclaringType.FullName == typeof(PlayCardAction).FullName));
        }

        private static (bool flowControl, bool value) SetupForReadingActionsMethod(Card card, out ModuleDefinition module, out MethodDefinition moveNext)
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

            module = null;
            moveNext = null;

            if (stateMachineType == null)
                return (flowControl: false, value: false);
            module = ModuleDefinition.ReadModule(card.GetType().Assembly.Location);
            var stateMachineName = stateMachineType.FullName.Replace('+', '/');
            var stateMachine = FindTypes(module.Types)
                .FirstOrDefault(type => type.FullName == stateMachineName);

            moveNext = stateMachine?.Methods
                .FirstOrDefault(method => method.Name == "MoveNext" && method.HasBody);
            if (moveNext == null)
                return (flowControl: false, value: false);
            return (flowControl: true, value: default);
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
