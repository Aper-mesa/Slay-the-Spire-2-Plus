using Godot;
using MegaCrit.Sts2.Core.Models.Cards;

namespace MegaCrit.Sts2.Core.Models.CardPools;

public sealed class TokenCardPool : CardPoolModel
{
	public override string Title => "token";

	public override string EnergyColorName => "colorless";

	public override string CardFrameMaterialPath => "card_frame_colorless";

	public override Color DeckEntryCardColor => Colors.White;

	public override bool IsColorless => true;

	protected override CardModel[] GenerateAllCards()
	{
		return new CardModel[39]
		{
			ModelDb.Card<Amnesia>(),
			ModelDb.Card<Death>(),
			ModelDb.Card<Disintegration>(),
			ModelDb.Card<Fuel>(),
			ModelDb.Card<GiantRock>(),
			ModelDb.Card<Joy>(),
			ModelDb.Card<Judgement>(),
			ModelDb.Card<Justice>(),
			ModelDb.Card<Luminesce>(),
			ModelDb.Card<MindRot>(),
			ModelDb.Card<MinionDiveBomb>(),
			ModelDb.Card<MinionSacrifice>(),
			ModelDb.Card<MinionStrike>(),
			ModelDb.Card<Shiv>(),
			ModelDb.Card<Sloth>(),
			ModelDb.Card<Soul>(),
			ModelDb.Card<SovereignBlade>(),
			ModelDb.Card<SweepingGaze>(),
			ModelDb.Card<Strength>(),
			ModelDb.Card<Temperance>(),
			ModelDb.Card<TheChariot>(),
			ModelDb.Card<TheDevil>(),
			ModelDb.Card<TheFool>(),
			ModelDb.Card<TheLovers>(),
			ModelDb.Card<TheEmperor>(),
			ModelDb.Card<TheEmpress>(),
			ModelDb.Card<TheHangedMan>(),
			ModelDb.Card<TheHermit>(),
			ModelDb.Card<TheHierophant>(),
			ModelDb.Card<TheHighPriestess>(),
			ModelDb.Card<TheMagician>(),
			ModelDb.Card<TheMoon>(),
			ModelDb.Card<TheShot>(),
			ModelDb.Card<TheStar>(),
			ModelDb.Card<TheSun>(),
			ModelDb.Card<TheTower>(),
			ModelDb.Card<TheWorld>(),
			ModelDb.Card<WasteAway>(),
			ModelDb.Card<WheelOfFortune>()
		};
	}
}
