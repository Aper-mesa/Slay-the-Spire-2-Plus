using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class Expose : CardModel
{
	private const string _powerKey = "Power";

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new DynamicVar("Power", 2m));

	public override IEnumerable<CardKeyword> CanonicalKeywords => new _003C_003Ez__ReadOnlySingleElementList<CardKeyword>(CardKeyword.Exhaust);

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlyArray<IHoverTip>(new IHoverTip[3]
	{
		HoverTipFactory.FromPower<VulnerablePower>(),
		HoverTipFactory.FromPower<ArtifactPower>(),
		HoverTipFactory.Static(StaticHoverTip.Block)
	});

	public Expose()
		: base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
		await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
		VfxCmd.PlayOnCreatureCenter(Owner.Creature, "vfx/vfx_flying_slash");
		int amount = DynamicVars["Power"].IntValue;
		await CreatureCmd.LoseBlock(choiceContext, cardPlay.Target, cardPlay.Target.Block, Owner.Creature);
		if (cardPlay.Target.HasPower<ArtifactPower>())
		{
			await PowerCmd.Remove<ArtifactPower>(cardPlay.Target);
		}
		await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Target, amount, Owner.Creature, this);
	}

	protected override void OnUpgrade()
	{
		DynamicVars["Power"].UpgradeValueBy(1m);
	}
}
