using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class SovereignBlade : CardModel
{
	private const int _baseDamage = 10;

	private const string _sovereignBladeSfx = "event:/sfx/characters/regent/regent_sovereign_blade";

	private decimal _currentDamage = 10m;

	private decimal _currentRepeats = 1m;

	private bool _createdThroughForge;

	protected override IEnumerable<string> ExtraRunAssetPaths => NSovereignBladeVfx.AssetPaths;

	/// <remarks>
	/// Dynamic TargetType based on whether the player has <see cref="T:MegaCrit.Sts2.Core.Models.Powers.SeekingEdgePower" />.
	/// </remarks>
	public override TargetType TargetType
	{
		get
		{
			if (!HasSeekingEdge)
			{
				return TargetType.AnyEnemy;
			}
			return TargetType.AllEnemies;
		}
	}

	/// <remarks>
	/// Dynamic GainsBlock based on whether the player has <see cref="T:MegaCrit.Sts2.Core.Models.Powers.ParryPower" />.
	/// </remarks>
	public override bool GainsBlock => GetOwnerParryAmount(this) > 0m;

	private decimal CurrentDamage
	{
		get
		{
			return _currentDamage;
		}
		set
		{
			AssertMutable();
			_currentDamage = value;
		}
	}

	private decimal CurrentRepeats
	{
		get
		{
			return _currentRepeats;
		}
		set
		{
			AssertMutable();
			_currentRepeats = value;
		}
	}

	public bool CreatedThroughForge
	{
		get
		{
			return _createdThroughForge;
		}
		set
		{
			AssertMutable();
			_createdThroughForge = value;
		}
	}

	public override IEnumerable<CardKeyword> CanonicalKeywords => new _003C_003Ez__ReadOnlySingleElementList<CardKeyword>(CardKeyword.Retain);

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[5]
	{
		new DamageVar(10m, ValueProp.Move),
		new CalculationBaseVar(0m),
		new CalculationExtraVar(1m),
		new CalculatedBlockVar(ValueProp.Move).WithMultiplier((CardModel card, Creature? _) => GetOwnerParryAmount(card)),
		new RepeatVar(1)
	});

	private bool HasSeekingEdge
	{
		get
		{
			if (IsMutable && Owner != null)
			{
				return Owner.Creature.HasPower<SeekingEdgePower>();
			}
			return false;
		}
	}

	public SovereignBlade()
		: base(2, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		string animName = ((Owner.Character is Regent) ? "sovereignBladeTrigger" : "Cast");
		float delay = ((Owner.Character is Regent) ? 0.25f : Owner.Character.CastAnimDelay);
		AttackCommand attackCommand = DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).WithHitCount(DynamicVars.Repeat.IntValue)
			.WithAttackerAnim(animName, delay)
			.WithAttackerFx(null, "event:/sfx/characters/regent/regent_sovereign_blade");
		if (HasSeekingEdge)
		{
			attackCommand = attackCommand.TargetingAllOpponents(CombatState).BeforeDamage(() =>
			{
				IReadOnlyList<Creature> hittableEnemies = CombatState.HittableEnemies;
				if (hittableEnemies.Count <= 0)
				{
					return Task.CompletedTask;
				}
				NSovereignBladeVfx vfxNode = GetVfxNode(Owner, this);
				NCreature nCreature = NCombatRoom.Instance?.GetCreatureNode(hittableEnemies[0]);
				if (vfxNode != null && nCreature != null)
				{
					vfxNode.Attack(nCreature.VfxSpawnPosition);
				}
				return Task.CompletedTask;
			}).WithHitFx("vfx/vfx_giant_horizontal_slash", null, "slash_attack.mp3");
		}
		else
		{
			ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
			attackCommand = attackCommand.Targeting(cardPlay.Target).BeforeDamage(() =>
			{
				NSovereignBladeVfx vfxNode = GetVfxNode(Owner, this);
				NCreature nCreature = NCombatRoom.Instance?.GetCreatureNode(cardPlay.Target);
				if (vfxNode != null && nCreature != null)
				{
					vfxNode.Attack(nCreature.VfxSpawnPosition);
				}
				return Task.CompletedTask;
			}).WithHitVfxNode(NBigSlashVfx.Create)
				.WithHitVfxNode(NBigSlashImpactVfx.Create);
		}
		await attackCommand.Execute(choiceContext);
		if (GetOwnerParryAmount(this) > 0m)
		{
			await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.CalculatedBlock.Calculate(cardPlay.Target), DynamicVars.CalculatedBlock.Props, cardPlay);
		}
	}

	protected override void OnUpgrade()
	{
		EnergyCost.UpgradeBy(-1);
	}

	protected override void AfterCloned()
	{
		base.AfterCloned();
		CreatedThroughForge = false;
	}

	protected override void AfterDowngraded()
	{
		base.AfterDowngraded();
		DynamicVars.Damage.BaseValue = CurrentDamage;
		DynamicVars.Repeat.BaseValue = CurrentRepeats;
	}

	public override void AfterTransformedFrom()
	{
		RemoveSovereignBladeNode();
	}

	public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
	{
		if (card != this)
		{
			return Task.CompletedTask;
		}
		if ((!CreatedThroughForge && oldPileType == PileType.None) || oldPileType == PileType.Exhaust)
		{
			ForgeCmd.PlayCombatRoomForgeVfx(Owner, this);
		}
		if (card.Pile.Type == PileType.Exhaust)
		{
			RemoveSovereignBladeNode();
		}
		return Task.CompletedTask;
	}

	public void AddDamage(decimal amount)
	{
		DynamicVars.Damage.BaseValue += amount;
		CurrentDamage = DynamicVars.Damage.BaseValue;
	}

	public void SetRepeats(decimal amount)
	{
		DynamicVars.Repeat.BaseValue = amount;
		CurrentRepeats = DynamicVars.Repeat.BaseValue;
	}

	public static NSovereignBladeVfx? GetVfxNode(Player player, CardModel card)
	{
		CardModel originalCard = card.DupeOf ?? card;
		return (NCombatRoom.Instance?.GetCreatureNode(player.Creature))?.GetChildren().OfType<NSovereignBladeVfx>().FirstOrDefault((NSovereignBladeVfx b) => b.Card == originalCard);
	}

	private void RemoveSovereignBladeNode()
	{
		GetVfxNode(Owner, this)?.RemoveSovereignBlade();
	}

	private static decimal GetOwnerParryAmount(CardModel card)
	{
		if (!card.IsMutable)
		{
			return 0m;
		}
		if (card.Owner == null)
		{
			return 0m;
		}
		if (card.Pile == null)
		{
			return 0m;
		}
		if (!card.Pile.IsCombatPile)
		{
			return 0m;
		}
		return card.Owner.Creature.GetPowerAmount<ParryPower>();
	}
}
