namespace __SCRIPTS
{
	public class KeyPickupEffect : PickupEffect
	{
		private int amount = 1;
		private AmmoInventory.AmmoType ammoType;

		public override void StartEffect(IGetAttacked life)
		{
			effectDuration = 0;
			life.player.GainKey();
		}

		public KeyPickupEffect(float _effectDuration, int _amount) : base(
			_effectDuration)
		{
			amount = _amount;
		}
	}
}
