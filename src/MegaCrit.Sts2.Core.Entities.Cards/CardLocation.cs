using MegaCrit.Sts2.Core.Entities.Players;

namespace MegaCrit.Sts2.Core.Entities.Cards;

public record struct CardLocation(Player player, PileType pileType, CardPilePosition position)
{
	public Player player = player;

	public PileType pileType = pileType;

	public CardPilePosition position = position;
}
