public class GasPickupEffect : PickupEffect
{
	private int amount = 1;
	private AmmoInventory.AmmoType ammoType;

	public override void StartEffect(Life life)
	{
		effectDuration = 0;
		life.player.ChangePlayerStat(PlayerStat.StatType.Gas, amount);
	}

	public GasPickupEffect(float _effectDuration, int _amount) : base(
		_effectDuration)
	{
		amount = _amount;
	}
}