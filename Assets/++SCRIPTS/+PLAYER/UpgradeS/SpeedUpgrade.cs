public class SpeedUpgrade : Upgrade
{
	public override int GetCost()
	{
		return 150;
	}

	public override string GetName()
	{
		return "Speed Upgrade";
	}

	public override void CauseEffect(Player player)
	{
		base.CauseEffect(player);
		player.spawnedPlayerDefence.SetExtraMaxSpeed(50 * level);
	}
}