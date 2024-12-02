public class PlayerStat
{
	public PlayerStat(StatType statType, float startingValue)
	{
		value = startingValue;
		type = statType;
	}
	public enum StatType
	{
		Kills,
		AttacksTotal,
		AttacksHit,
		Accuracy,
		TotalCash,
		Gas,
		Key
	}

	public readonly StatType type;
	public float value;

	public void ChangeStat(float change)
	{
		value += change;
	}
}