namespace GangstaBean.Player.Upgrades.UpgradeS
{
	public class DamageUpgrade : Upgrade
	{
		public override string GetName() => "Damage Upgrade";
		public override string GetDescription => "Attack damage +10%";

		public override void CauseEffect(Player player)
		{
			base.CauseEffect(player);
			player.spawnedPlayerDefence.SetExtraMaxDamageFactor(.10f * level);
		}

	}
}