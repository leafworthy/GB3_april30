public class CashPickupEffect : PickupEffect
{
	private int amount;

	public override void StartEffect(UnitStats stats)
	{
		effectDuration = 0;
		stats.ChangeStat(StatType.cash,amount);
		base.StartEffect(stats);
	}


	public CashPickupEffect(float _effectDuration, int _amount) : base(_effectDuration)
	{
		amount = _amount;
	}
}