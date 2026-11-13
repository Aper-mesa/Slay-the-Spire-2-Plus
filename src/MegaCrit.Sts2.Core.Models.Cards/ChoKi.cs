using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class ChoKi : CardModel
{
	public override bool GainsBlock => true;

	protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag>
	{
		CardTag.Cho,
		CardTag.Ki
	};

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlySingleElementList<IHoverTip>(HoverTipFactory.FromPower<StretchPower>());

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new BlockVar(8m, ValueProp.Move));

	protected override IEnumerable<string> ExtraRunAssetPaths
	{
		get
		{
			List<string> list = new List<string>();
			list.AddRange(NChoVfx.AssetPaths);
			list.AddRange(NKiVfx.AssetPaths);
			return new _003C_003Ez__ReadOnlyList<string>(list);
		}
	}

	public ChoKi()
		: base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
		await MelodyCmd.PlayMelodyVfx(Owner.Creature, CardTag.Cho);
		await MelodyCmd.PlayMelodyVfx(Owner.Creature, CardTag.Ki);
		int stretchAmount = Owner.Creature.GetPowerAmount<StretchPower>();
		await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
		if (stretchAmount > 0)
		{
			await PowerCmd.Apply<StretchPower>(choiceContext, Owner.Creature, stretchAmount, Owner.Creature, this);
		}
	}

	protected override void OnUpgrade()
	{
		DynamicVars.Block.UpgradeValueBy(3m);
	}
}
