public class AmmoPickupEffect : PickupEffect
{
	private int amount = 25;
	private AmmoHandler.AmmoType ammoType;
	public override void StartEffect(UnitStats stats)
	{
		base.StartEffect(stats);
		effectDuration = 0;
		var ammoHandler = stats.GetComponent<AmmoHandler>();
		ammoHandler.AddAmmoToReserve(ammoType, amount);
	}

	public AmmoPickupEffect(float _effectDuration, int _amount,   AmmoHandler.AmmoType _ammoType) : base(_effectDuration)
	{
		ammoType = _ammoType;
		amount = _amount;
	}
}
