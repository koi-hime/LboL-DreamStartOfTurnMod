# Runaway Dream

A small buff to the Dream mechanic, new adjustments to make it a top deck mechanic instead.
Original Author, Rokk; updated by こい姫.

- Permission granted to fork from Rokk

## How it works in Vanilla

Dream is a mechanic primarily used by Koishi Komeiji. Whenever you Dream X, the top X cards in the draw pile are essentially discarded and then gain the Dream keyword. On the next reshuffle, you may select *one* (1) card to put in your hand. All cards then lose Dream, and the ones you didn't pick go into the draw pile like normal. This is a very weak mechanic unless you have a way to know what's in your draw pile and in which order, and Koishi does not have easy access to Scry. Tossing away a whole bunch of cards to pick out just one from your deck is quite weak, especially because you could probably have drawn this card sooner by just not using Dream!

## How it works with this mod

In addition to the usual Dream mechanics, you now also get to pick one Dream card to place on top of our deck at the start of each turn, if you have at least one. This happens on any character at the start of your turn, as long as you have a card with the Dream keyword.

This does *not* remove the Dream keyword from other cards, so you can do it again next turn or during the next reshuffle. This extra retrieve step happens after the Draw step in your turn, so if you shuffle the deck at the start of your turn, you still only get to retrieve one Dream card as per usual, since the vanilla behavior removes the Dream keyword from all cards when it happens. Effectively, this means you get to always retrieve one Dream card at the start of your turn, regardless of whether or not a reshuffle has happened.

This rewards bigger decks and improved flexibility and in addition, this adjustment this will allow anyone who uses Dream (and especially Koishi) to more readily utlize top deck mechanics that enchance playing cards from the deck, such as follow-ups, emotion setup, and free/doubled card plays. It is more synergistic as well as flavor themed than just adding to hand, since follow up cards want to be in the deck to be played, and several cards want to be double played as well, and as well for free. Koishi and Dreams happen in the subconscious, after all~

Additionally, the Dream Express card can now more reliably place a high cost card to deal more damage for the AoE effect due to this top deck mechanic, if no draw was done during the turn. I was considering adding the card to the hand or making the top deck card not able to be dreamed, but I want to see how this plays out first before doing other adjustments.

### Installation

I recommend using Thunderstore and r2modman or the Steam Workshop to install this mod (links below). For manual installation, grab the latest zip from the Releases section. Requires BepInEx since this is a Harmony mod.

### Building

Clone this project, make sure the References are set up correctly (manually edit the `.csproj` to change the game dir), and then open the solution with Visual Studio. I use VS2022.

### Links

[Thunderstore page for Runaway Dream](https://thunderstore.io/c/touhou-lost-branch-of-legend/p/Rokk/DreamStartOfTurn/)

[Steam Workshop page for Runaway Dream](https://steamcommunity.com/sharedfiles/filedetails/?id=3572912894)
