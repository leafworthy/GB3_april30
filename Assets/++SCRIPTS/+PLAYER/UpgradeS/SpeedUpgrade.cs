using UnityEngine;

namespace __SCRIPTS.UpgradeS
{
	public class SpeedUpgrade : Upgrade
	{
		public override string GetDescription => "Walk speed + " +10*level + "%";

		public override string GetName() => "Speed Upgrade";

		public override void CauseEffect(Player player)
		{
			base.CauseEffect(player);
			Debug.Log("speed upgrade  caused effect " + .1f * level);
			player.spawnedPlayerDefence.SetExtraMaxSpeedFactor(.1f * level);
		}
	}
}