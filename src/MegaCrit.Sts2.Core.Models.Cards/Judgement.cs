using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class Judgement : CardModel
{
	public const string ThresholdKey = "Threshold";

	public override IEnumerable<CardKeyword> CanonicalKeywords => new _003C_003Ez__ReadOnlySingleElementList<CardKeyword>(CardKeyword.Exhaust);

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new IntVar("Threshold", 30m));

	public Judgement()
		: base(0, CardType.Skill, CardRarity.Token, TargetType.AnyEnemy)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
		if (cardPlay.Target.IsAlive && cardPlay.Target.CurrentHp <= DynamicVars["Threshold"].IntValue)
		{
			NDebugAudioManager.Instance?.Play("heavy_attack.mp3");
			cardPlay.Target.GetVfxContainer()?.AddChildSafely(NBigSlashVfx.Create(cardPlay.Target));
			cardPlay.Target.GetVfxContainer()?.AddChildSafely(NBigSlashImpactVfx.Create(cardPlay.Target));
			await CreatureCmd.SetCurrentHp(cardPlay.Target, 0m);
		}
	}

	protected override void OnUpgrade()
	{
		DynamicVars["Threshold"].UpgradeValueBy(10m);
	}
}
