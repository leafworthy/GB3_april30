public class DamageUpgrade : Upgrade
{
	public override int GetCost()
	{
		return 250;
	}
	public override void CauseEffect(Player player)
	{
		base.CauseEffect(player);
		player.spawnedPlayerDefence.SetExtraMaxDamage(10 * level);
	}

	public override string GetName()
	{
		return "Damage Upgrade";
	}
}