using UnityEngine;

namespace UpgradeS
{
	public class SpeedUpgrade : Upgrade
	{
		public override string GetDescription => "Walk speed +10%";
		public override int GetCost() => 250 * level;

		public override string GetName() => "Speed Upgrade";

		public override void CauseEffect(Player player)
		{
			base.CauseEffect(player);
			Debug.Log("speed upgrade  caused effect " + 2 * level);
			player.spawnedPlayerDefence.SetExtraMaxSpeedFactor(2 * level);
		}
	}
}