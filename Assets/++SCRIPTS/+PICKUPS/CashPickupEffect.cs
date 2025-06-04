using UnityEngine;

namespace GangstaBean.Pickups
{
	public class CashPickupEffect : PickupEffect
	{
		private int amount;

		public override void StartEffect(Life life)
		{
			effectDuration = 0;
			Debug.Log("cash pickup effect");
			PlayerStatsManager.I.ChangeStat(life.player, PlayerStat.StatType.TotalCash,amount);
			base.StartEffect(life);
		}


		public CashPickupEffect(float _effectDuration, int _amount) : base(_effectDuration)
		{
			amount = _amount;
		}
	}
}