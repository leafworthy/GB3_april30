using __SCRIPTS._PLAYER;
using __SCRIPTS._UNITS;

namespace __SCRIPTS._PICKUPS
{
	public class AmmoPickupEffect : PickupEffect
	{
		private int amount = 25;
		private AmmoInventory.AmmoType ammoType;
		public override void StartEffect(UnitStats stats)
		{
			base.StartEffect(stats);
			effectDuration = 0;
			var ammoHandler = stats.GetComponent<AmmoInventory>();
			ammoHandler.AddAmmoToReserve(ammoType, amount);
		}

		public AmmoPickupEffect(float _effectDuration, int _amount,   AmmoInventory.AmmoType _ammoType) : base(_effectDuration)
		{
			ammoType = _ammoType;
			amount = _amount;
		}
	}
}
