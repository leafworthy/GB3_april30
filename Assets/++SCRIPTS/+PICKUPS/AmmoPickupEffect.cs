public class AmmoPickupEffect : PickupEffect
{
	private int amount = 25;
	private AmmoInventory.AmmoType ammoType;
	public override void StartEffect(Life life)
	{
		base.StartEffect(life);
		effectDuration = 0;
		var ammoHandler = life.GetComponent<AmmoInventory>();
		ammoHandler.AddAmmoToReserve(ammoType, amount);
	}

	public AmmoPickupEffect(float _effectDuration, int _amount,   AmmoInventory.AmmoType _ammoType) : base(_effectDuration)
	{
		ammoType = _ammoType;
		amount = _amount;
	}
}