public class HealthPickupEffect : PickupEffect
{
	private int amount;

	public override void StartEffect(UnitStats stats)
	{
		effectDuration = 0;
		var defence = stats.GetComponent<Life>();
		defence.AddHealth(amount);
		base.StartEffect(stats);
	}


	public HealthPickupEffect(float _effectDuration, int _amount) : base(_effectDuration)
	{
		amount = _amount;
	}
}