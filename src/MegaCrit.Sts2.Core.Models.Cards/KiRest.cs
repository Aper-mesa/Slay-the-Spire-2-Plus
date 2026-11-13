using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class KiRest : CardModel
{
	protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Ki };

	protected override bool ShouldGlowGoldInternal => HasWahOrChoMelody;

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlyArray<IHoverTip>(new IHoverTip[4]
	{
		EnergyHoverTip,
		HoverTipFactory.Static(StaticHoverTip.WahMelody),
		HoverTipFactory.Static(StaticHoverTip.ChoMelody),
		HoverTipFactory.FromKeyword(CardKeyword.Retain)
	});

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new EnergyVar(5));

	protected override IEnumerable<string> ExtraRunAssetPaths
	{
		get
		{
			List<string> list = new List<string>();
			list.AddRange(NKiVfx.AssetPaths);
			return new _003C_003Ez__ReadOnlyList<string>(list);
		}
	}

	private bool HasWahOrChoMelody
	{
		get
		{
			if (!MelodyCmd.PlayedWahThisTurn(Owner))
			{
				return MelodyCmd.PlayedChoThisTurn(Owner);
			}
			return true;
		}
	}

	public KiRest()
		: base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await MelodyCmd.PlayMelodyVfx(Owner.Creature, CardTag.Ki);
		await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
		await PowerCmd.Apply<EnergyNextTurnPower>(choiceContext, Owner.Creature, DynamicVars.Energy.IntValue, Owner.Creature, this);
		if (HasWahOrChoMelody)
		{
			await PowerCmd.Apply<RetainHandPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
		}
	}

	protected override void OnUpgrade()
	{
		DynamicVars.Energy.UpgradeValueBy(1m);
	}
}
