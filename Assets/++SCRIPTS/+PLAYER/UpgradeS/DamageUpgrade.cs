namespace UpgradeS
{
	public class DamageUpgrade : Upgrade
	{
		public override string GetName() => "Damage Upgrade";
		public override string GetDescription => "Attack damage +10%";
		public override int GetCost() => 250 * level;

		public override void CauseEffect(Player player)
		{
			base.CauseEffect(player);
			player.spawnedPlayerDefence.SetExtraMaxDamageFactor(.10f * level);
		}

	}
}