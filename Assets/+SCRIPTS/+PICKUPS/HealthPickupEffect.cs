namespace __SCRIPTS
{
	public class HealthPickupEffect : PickupEffect
	{
		private int amount;

		public override void StartEffect(IGetAttacked life)
		{
			effectDuration = 0;
			life.AddHealth(amount);
			base.StartEffect(life);
		}


		public HealthPickupEffect(float _effectDuration, int _amount) : base(_effectDuration)
		{
			amount = _amount;
		}
	}
}
