namespace UpgradeS
{
	public class HealthUpgrade : Upgrade
	{
		public override string GetDescription => "Max health +10%";
		public override string GetName() => "Health Upgrade";
		public override int GetCost() => 500 * level;

		public override void CauseEffect(Player player)
		{
			base.CauseEffect(player);
			player.spawnedPlayerDefence.SetExtraMaxHealthFactor(.1f*level);
		}

		
	}
}