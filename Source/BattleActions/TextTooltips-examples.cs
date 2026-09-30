// using LBoL.Base;
// using LBoL.ConfigData;
// using LBoL.Core;
// using LBoL.Core.Battle;
// using LBoL.Core.Battle.BattleActions;
// using LBoL.Core.Battle.Interactions;
// using LBoL.Core.Cards;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Text.RegularExpressions;
// using YamlDotNet.Serialization.Schemas;


// namespace DreamStartOfTurnMod_TopDeck.Source.Tooltips
// {
// 	/// <summary>
// 	/// Tooltip examples
// 	/// </summary>
// 	public static class TextTooltips
// 	{
// 		private static readonly (string Marker, string Effect)[] effects =
// 		[
// 			("Mill", nameof(seMill)),
// 		("Tearlaments Fusion Summon", nameof(seTearFusionSummon)),
// 		("Summon", nameof(seSummon)),
// 		("Extra Library", nameof(seExtraLibrary)),
// 		("Fusion", nameof(seFusion)),
// 		("Synchro", nameof(seSynchro)),
// 		("Xyz", nameof(seXyz)),
// 		("Tuner", nameof(seTuner)),
// 		("Monster", nameof(seMonster)),
// 		("Trap", nameof(seTrap)),
// 		("Weak", nameof(Weak)),
// 		("Temporary Firepower Down", nameof(TempFirepowerNegative)),
// 		("Restraint", nameof(seRestraint)),
// 		("Lock", nameof(seLock)),
// 		("Pledge", nameof(sePledge)),
// 	];

// 		private static readonly (string Marker, Keyword Keyword)[] keywords =
// 		[
// 			("Block", Keyword.Block),
// 		("Barrier", Keyword.Shield),
// 		("Exile", Keyword.Exile),
// 		("Retain", Keyword.Retain),
// 		("Ethereal", Keyword.Ethereal),
// 		("Unplayable", Keyword.Forbidden),
// 		("temporarily cost", Keyword.TempMorph),
// 	];

// 		private static readonly HashSet<string> applied = [];

// 		private static bool Matches(string text, string term) =>
// 			Regex.IsMatch(text, @"\|(?:.:)?" + Regex.Escape(term));

// 		// Once per card Id, on its first Initialize (the loc is loaded by then). The config
// 		// object is shared by every card of that Id.
// 		public static void Apply(CardConfig config, bool extraLibrary = false)
// 		{
// 			if (applied.Contains(config.Id)) return;
// 			Dictionary<string, string> fields = YgoLoc.CardFields(config.Id, Locale.En);
// 			if (fields.Count == 0) return;
// 			applied.Add(config.Id);

// 			string text = string.Join("\n", fields.Values);
// 			List<string> related = [.. config.RelativeEffects ?? []];
// 			foreach ((string marker, string effect) in effects)
// 				if (Matches(text, marker) && !related.Contains(effect))
// 					related.Add(effect);
// 			if (fields.Keys.Any(k => k.StartsWith("Once.")) && !related.Contains(nameof(seOncePerRound)))
// 				related.Add(nameof(seOncePerRound));
// 			if (fields.Keys.Any(k => k.StartsWith("OnceBattle.")) && !related.Contains(nameof(seOncePerBattle)))
// 				related.Add(nameof(seOncePerBattle));
// 			// Extra Library monsters explain what that is, whether or not their text names it.
// 			if (extraLibrary && !related.Contains(nameof(seExtraLibrary)))
// 				related.Add(nameof(seExtraLibrary));
// 			config.RelativeEffects = related;
// 			config.UpgradedRelativeEffects = related;

// 			foreach ((string marker, Keyword keyword) in keywords)
// 				if (Matches(text, marker))
// 					config.RelativeKeyword |= keyword;
// 			config.UpgradedRelativeKeyword = config.RelativeKeyword;
// 			BepinexPlugin.log.LogDebug($"TextTooltips {config.Id}: effects [{string.Join(", ", related)}], keywords {config.RelativeKeyword}");
// 		}
// 	}
// }
