using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class ShowOff : CardModel
{
	private const int _baseAim = 5;

	private const string _baseAimKey = "BaseAim";

	private int _currentAim = 5;

	private int _increasedAim;

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlySingleElementList<IHoverTip>(HoverTipFactory.FromPower<AimPower>());

	public override IEnumerable<CardKeyword> CanonicalKeywords => new _003C_003Ez__ReadOnlySingleElementList<CardKeyword>(CardKeyword.Exhaust);

	[SavedProperty]
	public int CurrentAim
	{
		get
		{
			return _currentAim;
		}
		set
		{
			AssertMutable();
			_currentAim = value;
			DynamicVars["AimPower"].BaseValue = _currentAim;
		}
	}

	[SavedProperty]
	public int IncreasedAim
	{
		get
		{
			return _increasedAim;
		}
		set
		{
			AssertMutable();
			_increasedAim = value;
		}
	}

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[3]
	{
		new IntVar("BaseAim", 5m),
		new PowerVar<AimPower>(CurrentAim),
		new PowerVar<ShowOffPower>(2m)
	});

	public ShowOff()
		: base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await PowerCmd.Apply<AimPower>(choiceContext, Owner.Creature, DynamicVars["AimPower"].BaseValue, Owner.Creature, this);
		ShowOffPower showOffPower = await PowerCmd.Apply<ShowOffPower>(choiceContext, Owner.Creature, DynamicVars["ShowOffPower"].BaseValue, Owner.Creature, this);
		if (showOffPower != null)
		{
			showOffPower.ShowOffInstance = this;
		}
	}

	protected override void OnUpgrade()
	{
		DynamicVars["ShowOffPower"].UpgradeValueBy(1m);
		UpdateAim();
		DynamicVars["AimPower"].UpgradeValueBy(0m);
	}

	public void IncreaseAim(int amount)
	{
		IncreasedAim += amount;
		(DeckVersion as ShowOff)?.IncreaseAim(amount);
		UpdateAim();
	}

	protected override void AfterDowngraded()
	{
		UpdateAim();
	}

	private void UpdateAim()
	{
		CurrentAim = DynamicVars["BaseAim"].IntValue + IncreasedAim;
	}
}
