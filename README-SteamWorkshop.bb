[h1]Runaway Dream[/h1]

A small buff to the Dream mechanic, new adjustments to make it a top deck mechanic instead.
Original Author, Rokk; updated by こい姫.

[list][*]Permission granted to fork from Rokk[/list]

[h2]Update Log[/h2]
[list]
[*] v1.0.5: Added these counters for Dream card in hand and follow-up cards in the draw pile.
[*] v1.0.4: Added a count of the number of draw cards and names of the draw cards in hand to the card selection interaction description, so that the player can see how many draw cards they have in hand when selecting a Dream card to place on top of their deck.
[*] v1.0.3: Hard coded localized string for the card selection interaction description, since I don't know how to add a custom localized string to the Dream card interaction UI yet. Will need to figure out how to do that in the future.
[*] v1.0.2: Adjusted this to be a top deck mechanic instead.
[*] v1.0.1: Fixed false dependency on Sideloader causing the mod to not load without Sideloader.
[*] v1.0.0: Initial Release.
[/list]

[h2]How it works in Vanilla[/h2]

Dream is a mechanic that was introduced with Koishi Komeiji, and has some synergies with it but could use more still. Whenever you Dream X, the top X cards in the draw pile are essentially discarded and then gain the Dream keyword. On the next reshuffle, you may select [i]one[/i] (1) card to put in your hand. All cards then lose Dream, and the ones you didn't pick go into the draw pile like normal.

This is an interesting mechanic that is currently under utilized since draw pile information is difficult to acquire, due to the fact that Koishi does not have frequent Scry access. Not being able to access the Dream cards normally until the reshuffle can be quite underwhelming as it is currently, since there's only a few innate way for Koishi to fetch discard cards with Black Mana, and 1 more reliable card in the Blue Koishi card pool.

[h2]How it works with this mod[/h2]

In addition to the usual Dream mechanics, you now also get to pick one Dream card to place on top of your draw pile at the start of each turn, if you have at least one. This happens on any character at the start of your turn, as long as you have a card with the Dream keyword.

This does [i]not[/i] remove the Dream keyword from other cards, so you can do it again next turn or during the next reshuffle. This extra retrieve step happens after the Draw step in your turn, so if you shuffle the deck at the start of your turn, you still only get to retrieve one Dream card as per usual, since the vanilla behavior removes the Dream keyword from all cards when it happens. Effectively, this means you get to always retrieve one Dream card at the start of your turn, regardless of whether or not a reshuffle has happened.

This rewards bigger decks and improved flexibility and in addition, this adjustment this will allow anyone who uses Dream (and especially Koishi) to more readily utlize top deck mechanics that enchance playing cards from the deck, such as follow-ups, emotion setup, and free/doubled card plays. It is more synergistic and flavor themed compared to adding to hand every turn, since follow up cards want to be in the deck to be played. Several other cards want to be played for free or multiple times as well, so this excels at enabling setup for these bonus effects. Koishi's mind and dreams happen in the subconscious, after all~

Additionally, the Dream Express card can now more reliably place a high cost card to deal more damage for the AoE effect due to this top deck mechanic, if no draw was done during the turn. I was considering adding the card to the hand or making the top deck card undreamable, but I want to see how this plays out first before doing other adjustments.
