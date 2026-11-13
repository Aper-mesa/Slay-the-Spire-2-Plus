using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class ChoFirm : CardModel
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlySingleElementList<IHoverTip>(ArrowHoverTip);

	protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Cho };

	protected override bool ShouldGlowGoldInternal => ArrowCmd.PlayerHasAnyArrows(Owner);

	public override bool GainsBlock => true;

	protected override IEnumerable<string> ExtraRunAssetPaths
	{
		get
		{
			List<string> list = new List<string>();
			list.AddRange(NChoVfx.AssetPaths);
			return new _003C_003Ez__ReadOnlyList<string>(list);
		}
	}

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new BlockVar(6m, ValueProp.Move));

	public ChoFirm()
		: base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
		await MelodyCmd.PlayMelodyVfx(Owner.Creature, CardTag.Cho);
		int blockGains = 1;
		if (ArrowCmd.PlayerHasAnyArrows(Owner))
		{
			blockGains++;
		}
		for (int i = 0; i < blockGains; i++)
		{
			await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
		}
	}

	protected override void OnUpgrade()
	{
		DynamicVars.Block.UpgradeValueBy(2m);
	}
}
