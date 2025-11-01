using UnityEngine;

namespace __SCRIPTS
{
	public class CashPickupEffect : PickupEffect
	{
		private int amount;

		public override void StartEffect(Life life)
		{
			effectDuration = 0;
			var stats = ServiceLocator.Get<PlayerStatsManager>();
			stats.ChangeStat(life.player,PlayerStat.StatType.TotalCash,amount);
			base.StartEffect(life);
		}


		public CashPickupEffect(float _effectDuration, int _amount) : base(_effectDuration)
		{
			amount = _amount;
		}
	}
}
