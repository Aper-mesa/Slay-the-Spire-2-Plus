using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Events;

public sealed class AbyssalBaths : EventModel
{
	private const int _baseDamage = 3;

	private const int _damageScaling = 1;

	private int _lingerCount;

	private int LingerCount
	{
		get
		{
			return _lingerCount;
		}
		set
		{
			AssertMutable();
			_lingerCount = value;
		}
	}

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[3]
	{
		new MaxHpVar(2m),
		new DamageVar(3m, ValueProp.Unblockable | ValueProp.Unpowered),
		new HealVar(10m)
	});

	protected override IReadOnlyList<EventOption> GenerateInitialOptions()
	{
		return new _003C_003Ez__ReadOnlyArray<EventOption>(new EventOption[2]
		{
			new EventOption(this, Immerse, "ABYSSAL_BATHS.pages.INITIAL.options.IMMERSE").ThatDoesDamage((decimal)DynamicVars.Damage.IntValue - DynamicVars.MaxHp.BaseValue),
			new EventOption(this, Abstain, "ABYSSAL_BATHS.pages.INITIAL.options.ABSTAIN")
		});
	}

	private async Task Immerse()
	{
		await OnImmerse();
		SetEventState(L10NLookup("ABYSSAL_BATHS.pages.IMMERSE.description"), new _003C_003Ez__ReadOnlyArray<EventOption>(new EventOption[2]
		{
			new EventOption(this, Linger, "ABYSSAL_BATHS.pages.ALL.options.LINGER").ThatDoesDamage((decimal)DynamicVars.Damage.IntValue - DynamicVars.MaxHp.BaseValue),
			new EventOption(this, ExitBaths, "ABYSSAL_BATHS.pages.ALL.options.EXIT_BATHS")
		}));
	}

	private async Task Abstain()
	{
		await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.IntValue);
		SetEventFinished(L10NLookup("ABYSSAL_BATHS.pages.ABSTAIN.description"));
	}

	private async Task Linger()
	{
		LingerCount++;
		if (LingerCount > 9)
		{
			LingerCount = 9;
		}
		await OnImmerse();
		decimal damage = (decimal)DynamicVars.Damage.IntValue - DynamicVars.MaxHp.BaseValue;
		if (WillKillPlayer(damage))
		{
			SetEventState(L10NLookup("ABYSSAL_BATHS.pages.DEATH_WARNING.description"), new _003C_003Ez__ReadOnlyArray<EventOption>(new EventOption[2]
			{
				new EventOption(this, Linger, "ABYSSAL_BATHS.pages.ALL.options.LINGER").ThatDoesDamage(damage),
				new EventOption(this, ExitBaths, "ABYSSAL_BATHS.pages.ALL.options.EXIT_BATHS")
			}));
			return;
		}
		SetEventState(L10NLookup($"ABYSSAL_BATHS.pages.LINGER{LingerCount}.description"), new _003C_003Ez__ReadOnlyArray<EventOption>(new EventOption[2]
		{
			new EventOption(this, Linger, "ABYSSAL_BATHS.pages.ALL.options.LINGER").ThatDoesDamage(damage),
			new EventOption(this, ExitBaths, "ABYSSAL_BATHS.pages.ALL.options.EXIT_BATHS")
		}));
	}

	private bool WillKillPlayer(decimal damage)
	{
		return (decimal)Owner.Creature.CurrentHp <= damage;
	}

	private Task ExitBaths()
	{
		SetEventFinished(L10NLookup("ABYSSAL_BATHS.pages.EXIT_BATHS.description"));
		return Task.CompletedTask;
	}

	private async Task OnImmerse()
	{
		await CreatureCmd.GainMaxHp(Owner.Creature, DynamicVars.MaxHp.BaseValue);
		await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars.Damage, null, null, null);
		DynamicVars.Damage.BaseValue += 1m;
	}
}
