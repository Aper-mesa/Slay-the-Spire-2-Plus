using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class WahCho : CardModel
{
	public override bool GainsBlock => true;

	protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag>
	{
		CardTag.Wah,
		CardTag.Cho
	};

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[2]
	{
		new DamageVar(3m, ValueProp.Move),
		new BlockVar(3m, ValueProp.Move)
	});

	protected override IEnumerable<string> ExtraRunAssetPaths
	{
		get
		{
			List<string> list = new List<string>();
			list.AddRange(NWahVfx.AssetPaths);
			list.AddRange(NChoVfx.AssetPaths);
			return new _003C_003Ez__ReadOnlyList<string>(list);
		}
	}

	public WahCho()
		: base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
		await MelodyCmd.PlayMelodyVfx(Owner.Creature, CardTag.Wah);
		await MelodyCmd.PlayMelodyVfx(Owner.Creature, CardTag.Cho);
		await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
		await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
			.WithHitFx("vfx/vfx_attack_blunt")
			.Execute(choiceContext);
	}

	protected override void OnUpgrade()
	{
		DynamicVars.Damage.UpgradeValueBy(2m);
		DynamicVars.Block.UpgradeValueBy(2m);
	}
}
