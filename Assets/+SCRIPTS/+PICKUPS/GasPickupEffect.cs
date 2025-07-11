namespace __SCRIPTS
{
	public class GasPickupEffect : PickupEffect
	{
		private int amount = 1;
		private AmmoInventory.AmmoType ammoType;

		public override void StartEffect(Life life)
		{
			effectDuration = 0;
			var stats = ServiceLocator.Get<PlayerStats>();

			stats.ChangeStat(PlayerStat.StatType.Gas, amount);
		}

		public GasPickupEffect(float _effectDuration, int _amount) : base(
			_effectDuration)
		{
			amount = _amount;
		}
	}
}
