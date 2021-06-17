using _SCRIPTS;

public class HealthPickupEffect : PickupEffect
{
	private int amount = 25;

	public override void StartEffect(UnitStats stats)
	{
		effectDuration = 0;
		var defence = stats.gameObject.GetComponent<DefenceHandler>();
		if (defence is null) return;
		defence.AddHealth(amount);
	}

	protected override void StopEffect()
	{
		base.StopEffect();
	}

	public HealthPickupEffect(float _effectDuration, int _amount) : base(_effectDuration)
	{
		amount = _amount;
	}
}
