using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Cards.Mocks;

public sealed class MockSkillCard : MockCardModel
{
	private struct PowerApplication
	{
		public Type powerType;

		public int amount;

		public TargetType targetType;
	}

	private struct CardCreation
	{
		public CardModel canonicalCard;

		public int amount;
	}

	private const string _drawKey = "Draw";

	private const string _discardKey = "Discard";

	private int _blockCount = 1;

	private TargetType _targetType = TargetType.Self;

	private List<PowerApplication> _powerApplications = new List<PowerApplication>();

	private List<CardCreation> _cardCreations = new List<CardCreation>();

	public override CardType Type => CardType.Skill;

	public override TargetType TargetType => _targetType;

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[7]
	{
		new BlockVar(5m, ValueProp.Move),
		new CardsVar("Draw", 0),
		new CardsVar("Discard", 0),
		new EnergyVar(0),
		new ForgeVar(0),
		new StarsVar(0),
		new SummonVar(0m)
	});

	protected override void DeepCloneFields()
	{
		base.DeepCloneFields();
		_powerApplications = _powerApplications.ToList();
		_cardCreations = _cardCreations.ToList();
	}

	public override MockSkillCard MockBlock(int block)
	{
		AssertMutable();
		DynamicVars.Block.BaseValue = block;
		return this;
	}

	public MockSkillCard MockBlockCount(int blockCount)
	{
		AssertMutable();
		_blockCount = blockCount;
		return this;
	}

	public MockSkillCard MockDraw(int cards)
	{
		AssertMutable();
		DynamicVars["Draw"].BaseValue = cards;
		return this;
	}

	public MockSkillCard MockSummon(int summons)
	{
		AssertMutable();
		DynamicVars.Summon.BaseValue = summons;
		return this;
	}

	public MockSkillCard MockDiscard(int cards)
	{
		AssertMutable();
		DynamicVars["Discard"].BaseValue = cards;
		return this;
	}

	public MockSkillCard MockForge(decimal forge)
	{
		AssertMutable();
		DynamicVars.Forge.BaseValue = forge;
		return this;
	}

	public MockSkillCard MockStarGain(decimal stars)
	{
		AssertMutable();
		DynamicVars.Stars.BaseValue = stars;
		return this;
	}

	public MockSkillCard MockEnergyGain(decimal energy)
	{
		AssertMutable();
		DynamicVars.Energy.BaseValue = energy;
		return this;
	}

	public MockSkillCard MockPower<TPower>(int amount, TargetType targetType) where TPower : PowerModel
	{
		AssertMutable();
		if (_powerApplications.Any((PowerApplication a) => a.targetType != targetType))
		{
			throw new InvalidOperationException("Cannot have multiple power applications with different target types.");
		}
		_targetType = targetType;
		_powerApplications.Add(new PowerApplication
		{
			powerType = typeof(TPower),
			amount = amount,
			targetType = targetType
		});
		return this;
	}

	public MockSkillCard MockCreateCards<TCard>(int amount) where TCard : CardModel
	{
		AssertMutable();
		_cardCreations.Add(new CardCreation
		{
			canonicalCard = ModelDb.Card<TCard>(),
			amount = amount
		});
		return this;
	}

	protected override int GetBaseBlock()
	{
		return DynamicVars.Block.IntValue;
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		if (_mockSelfHpLoss > 0)
		{
			await CreatureCmd.Damage(choiceContext, Owner.Creature, _mockSelfHpLoss, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this, cardPlay);
		}
		for (int i = 0; i < _blockCount; i++)
		{
			await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
		}
		if (DynamicVars["Draw"].IntValue > 0)
		{
			await CardPileCmd.Draw(choiceContext, DynamicVars["Draw"].IntValue, Owner);
		}
		if (DynamicVars["Discard"].IntValue > 0)
		{
			await CardCmd.Discard(choiceContext, await CardSelectCmd.FromHandForDiscard(prefs: new CardSelectorPrefs(new LocString("cards", "MOCK_SKILL_CARD.discardSelectionPrompt"), DynamicVars["Discard"].IntValue), context: choiceContext, player: Owner, filter: null, source: this));
		}
		if (DynamicVars.Forge.BaseValue > 0m)
		{
			await ForgeCmd.Forge(DynamicVars.Forge.BaseValue, Owner, this);
		}
		if (DynamicVars.Stars.BaseValue > 0m)
		{
			await PlayerCmd.GainStars(DynamicVars.Stars.BaseValue, Owner);
		}
		if (DynamicVars.Summon.BaseValue > 0m)
		{
			await OstyCmd.Summon(choiceContext, Owner, DynamicVars.Summon.BaseValue, this);
		}
		if (DynamicVars.Energy.BaseValue > 0m)
		{
			await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
		}
		foreach (PowerApplication application in _powerApplications)
		{
			foreach (Creature item in application.targetType switch
			{
				TargetType.AllEnemies => (IEnumerable<Creature>)CombatState.Enemies, 
				TargetType.AnyEnemy => new _003C_003Ez__ReadOnlySingleElementList<Creature>(cardPlay.Target), 
				TargetType.Self => new _003C_003Ez__ReadOnlySingleElementList<Creature>(Owner.Creature), 
				_ => throw new ArgumentOutOfRangeException("targetType", application.targetType, null), 
			})
			{
				PowerModel power = ModelDb.GetById<PowerModel>(ModelDb.GetId(application.powerType)).ToMutable();
				await PowerCmd.Apply(choiceContext, power, item, application.amount, Owner.Creature, this);
			}
		}
		foreach (CardCreation cardCreation in _cardCreations)
		{
			CardModel canonicalCard = cardCreation.canonicalCard;
			if (!(canonicalCard is Shiv))
			{
				if (canonicalCard is Soul)
				{
					await Soul.CreateInHand(Owner, cardCreation.amount, CombatState);
					continue;
				}
				List<CardModel> list = new List<CardModel>();
				for (int j = 0; j < cardCreation.amount; j++)
				{
					list.Add(CombatState.CreateCard(cardCreation.canonicalCard, Owner));
				}
				await CardPileCmd.AddGeneratedCardsToCombat(list, PileType.Hand, Owner);
			}
			else
			{
				await Shiv.CreateInHand(Owner, cardCreation.amount, CombatState);
			}
		}
		if (_mockExtraLogic != null)
		{
			await _mockExtraLogic(this);
		}
	}

	protected override void OnUpgrade()
	{
		if (_mockUpgradeLogic != null)
		{
			_mockUpgradeLogic(this);
		}
		else
		{
			DynamicVars.Block.UpgradeValueBy(3m);
		}
	}
}
