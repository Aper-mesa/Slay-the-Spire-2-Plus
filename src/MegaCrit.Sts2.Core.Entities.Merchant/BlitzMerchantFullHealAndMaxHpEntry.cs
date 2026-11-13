using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;

namespace MegaCrit.Sts2.Core.Entities.Merchant;

public sealed class BlitzMerchantFullHealAndMaxHpEntry : BlitzMerchantEntry
{
	public override BlitzMerchantEntryType MerchantEntryType => BlitzMerchantEntryType.FullHealAndMaxHp;

	public BlitzMerchantFullHealAndMaxHpEntry(Player player)
		: base(player)
	{
	}

	protected override async Task<bool> OnTrySelect()
	{
		await GiveCurse();
		Creature creature = _player.Creature;
		await CreatureCmd.GainMaxHp(creature, 6m);
		await CreatureCmd.Heal(creature, creature.MaxHp - creature.CurrentHp);
		return true;
	}
}
