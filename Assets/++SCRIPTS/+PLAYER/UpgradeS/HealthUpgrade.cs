public class HealthUpgrade : Upgrade
{
	public override int GetCost()
	{
		return 200;
	}
	public override void CauseEffect(Player player)
	{
		base.CauseEffect(player);
		player.spawnedPlayerDefence.SetExtraMaxHealth(100*level);
	}

	public override string GetName()
	{
		return "Health Upgrade";
	}
}