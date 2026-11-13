using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace MegaCrit.Sts2.Core.Commands;

public static class MelodyCmd
{
	public static bool PlayedWahThisTurn(Player player)
	{
		return PlayedThisTurn(player, CardTag.Wah);
	}

	public static bool PlayedChoThisTurn(Player player)
	{
		return PlayedThisTurn(player, CardTag.Cho);
	}

	public static bool PlayedKiThisTurn(Player player)
	{
		return PlayedThisTurn(player, CardTag.Ki);
	}

	public static bool PlayedThisTurn(Player player, CardTag cardTag)
	{
		if (cardTag != CardTag.Wah && cardTag != CardTag.Cho && cardTag != CardTag.Ki)
		{
			throw new InvalidOperationException($"{cardTag} is not a valid song tag");
		}
		return CombatManager.Instance.History.CardPlaysFinished.Any((CardPlayFinishedEntry e) => e.HappenedThisTurn(player.Creature.CombatState) && e.CardPlay.Card.Tags.Contains(cardTag) && e.CardPlay.Player == player);
	}

	/// <summary>
	/// Whether this is the player's first card play with the tag this turn.
	/// Reads CardPlaysStarted and compares by identity instead of counting: the current play is already recorded as
	/// started when AfterCardPlayed runs, and Lost Lute autoplays a card from its own AfterCardPlayed, so a nested play
	/// can land in the history before the outer play's hook sees it. A count reads 2 for both.
	/// </summary>
	public static bool IsFirstPlayThisTurn(CardPlay cardPlay, CardTag cardTag)
	{
		if (cardTag != CardTag.Wah && cardTag != CardTag.Cho && cardTag != CardTag.Ki)
		{
			throw new InvalidOperationException($"{cardTag} is not a valid song tag");
		}
		CardPlay cardPlay2 = CombatManager.Instance.History.CardPlaysStarted.FirstOrDefault((CardPlayStartedEntry e) => e.HappenedThisTurn(cardPlay.Player.Creature.CombatState) && e.CardPlay.Card.Tags.Contains(cardTag) && e.CardPlay.Player == cardPlay.Player)?.CardPlay;
		return cardPlay2 == cardPlay;
	}

	public static int MelodiesThisTurn(Player player)
	{
		int num = 0;
		num += (PlayedWahThisTurn(player) ? 1 : 0);
		num += (PlayedChoThisTurn(player) ? 1 : 0);
		return num + (PlayedKiThisTurn(player) ? 1 : 0);
	}

	/// <summary>
	/// Returns the number of melody cards the player has played this turn.
	/// Unlike played this turn, this does not double count cards with multiple melody tags.
	/// </summary>
	/// <param name="player">Player who we are counting melody cards for</param>
	/// <returns></returns>
	public static int MelodyCardsPlayedThisTurn(Player player)
	{
		return CombatManager.Instance.History.CardPlaysFinished.Count((CardPlayFinishedEntry e) => e.HappenedThisTurn(player.Creature.CombatState) && (e.CardPlay.Card.Tags.Contains(CardTag.Wah) || e.CardPlay.Card.Tags.Contains(CardTag.Cho) || e.CardPlay.Card.Tags.Contains(CardTag.Ki)) && e.CardPlay.Player == player);
	}

	public static async Task PlayMelodyVfx(Creature creature, CardTag cardTag)
	{
		Node2D node2D = cardTag switch
		{
			CardTag.Wah => (Node2D)NWahVfx.Create(creature),
			CardTag.Cho => NChoVfx.Create(creature),
			CardTag.Ki => NKiVfx.Create(creature),
			_ => throw new InvalidOperationException($"{cardTag} is not a valid Melodies card tag"),
		};
		if (node2D != null)
		{
			NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(node2D);
			await Cmd.CustomScaledWait(0.3f, 0.5f);
		}
	}
}
