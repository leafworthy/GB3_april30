using System;

public class CashPickupEffect : PickupEffect
{
	private int amount;
	public static event Action<UnitStats, int> OnCashPickup;

	public override void StartEffect(UnitStats stats)
	{
		effectDuration = 0;
		stats.player.ChangePlayerStat(PlayerStat.StatType.TotalCash,amount);
		OnCashPickup?.Invoke(stats,amount);
		base.StartEffect(stats);
	}


	public CashPickupEffect(float _effectDuration, int _amount) : base(_effectDuration)
	{
		amount = _amount;
	}
}