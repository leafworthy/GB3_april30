namespace __SCRIPTS.UpgradeS
{
	public class HealthUpgrade : Upgrade
	{
		public override string GetDescription => "Max health +10%";
		public override string GetName() => "Health Upgrade";

		public override void CauseEffect(Player player)
		{
			base.CauseEffect(player);
			player.spawnedPlayerLife.SetExtraMaxHealthFactor(.1f*level);
		}

		
	}
}