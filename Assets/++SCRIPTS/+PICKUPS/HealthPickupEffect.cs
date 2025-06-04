namespace GangstaBean.Pickups
{
	public class HealthPickupEffect : PickupEffect
	{
		private int amount;

		public override void StartEffect(Life life)
		{
			effectDuration = 0;
			var defence = life.GetComponent<Life>();
			defence.AddHealth(amount);
			base.StartEffect(life);
		}


		public HealthPickupEffect(float _effectDuration, int _amount) : base(_effectDuration)
		{
			amount = _amount;
		}
	}
}