using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace MegaCrit.Sts2.Core.Models.Relics;

public class Wuppy : RelicModel
{
	public const int weakHp = 30;

	public const int strongHp = 60;

	private const string _currentHpVarKey = "CurrentHp";

	private bool _currentHpSet;

	private int _currentHp;

	private int _maxHp;

	private Creature? _combatWuppy;

	public override bool ShowCounter => true;

	public override int DisplayAmount => CurrentHp;

	public override RelicRarity Rarity => RelicRarity.Event;

	public override bool AddsPet => true;

	public override LocString Title
	{
		get
		{
			if (IsCanonical)
			{
				return base.Title;
			}
			if (MaxHp < 60)
			{
				return base.Title;
			}
			return RelicModel.L10NLookup("WUPPY_STRONG.title");
		}
	}

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[2]
	{
		new MaxHpVar(0m),
		new DynamicVar("CurrentHp", 0m)
	});

	[SavedProperty]
	public int CurrentHp
	{
		get
		{
			return _currentHp;
		}
		set
		{
			AssertMutable();
			_currentHp = value;
			_currentHpSet = true;
			DynamicVars["CurrentHp"].BaseValue = value;
			InvokeDisplayAmountChanged();
			Status = ((_currentHp == 0) ? RelicStatus.Disabled : RelicStatus.Normal);
		}
	}

	[SavedProperty]
	public int MaxHp
	{
		get
		{
			return _maxHp;
		}
		set
		{
			AssertMutable();
			_maxHp = value;
			DynamicVars.MaxHp.BaseValue = value;
		}
	}

	public Creature? CombatWuppy
	{
		get
		{
			return _combatWuppy;
		}
		private set
		{
			AssertMutable();
			_combatWuppy = value;
		}
	}

	public override async Task AfterObtained()
	{
		if (MaxHp == 0)
		{
			MaxHp = 60;
		}
		if (!_currentHpSet)
		{
			CurrentHp = MaxHp;
		}
		if (Owner.RunState.CurrentRoom is CombatRoom)
		{
			await SummonPet();
		}
	}

	public override async Task BeforeCombatStart()
	{
		await SummonPet();
	}

	private async Task SummonPet()
	{
		MonsterModel monster = ModelDb.Monster<MegaCrit.Sts2.Core.Models.Monsters.Wuppy>().ToMutable();
		CombatWuppy = Owner.Creature.CombatState.CreateCreature(monster, Owner.Creature.Side, null);
		await PowerCmd.Apply<WuppysLoyaltyPower>(new ThrowingPlayerChoiceContext(), CombatWuppy, 1m, null, null);
		await CreatureCmd.SetMaxHp(CombatWuppy, MaxHp);
		await CreatureCmd.SetCurrentHp(CombatWuppy, CurrentHp);
		await PlayerCmd.AddPet(CombatWuppy, Owner);
	}

	public override Task AfterCurrentHpChanged(Creature creature, decimal delta)
	{
		if (creature != CombatWuppy)
		{
			return Task.CompletedTask;
		}
		CurrentHp = CombatWuppy.CurrentHp;
		return Task.CompletedTask;
	}

	public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
	{
		if (player != Owner)
		{
			return false;
		}
		if (options.Any((RestSiteOption o) => o is RestoreRestSiteOption))
		{
			return false;
		}
		options.Add(new RestoreRestSiteOption(Owner));
		return true;
	}
}
