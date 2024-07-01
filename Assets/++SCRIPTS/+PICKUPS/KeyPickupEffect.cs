using __SCRIPTS._PLAYER;
using __SCRIPTS._UNITS;

namespace __SCRIPTS._PICKUPS
{
	public class KeyPickupEffect : PickupEffect
	{
		private int amount = 1;
		private AmmoInventory.AmmoType ammoType;

		public override void StartEffect(UnitStats stats)
		{
			effectDuration = 0;
			stats.player.GainKey();
		}

		public KeyPickupEffect(float _effectDuration, int _amount) : base(
			_effectDuration)
		{
			amount = _amount;
		}
	}
}