namespace __SCRIPTS.UpgradeS
{
	public class DamageUpgrade : Upgrade
	{
		public override string GetName() => "Damage Upgrade";
		public override string GetDescription => "Attack damage +10%";

		public override void CauseEffect(Player player)
		{
			base.CauseEffect(player);
			player.spawnedPlayerLife.SetExtraMaxDamageFactor(.10f * level);
		}

	}
}