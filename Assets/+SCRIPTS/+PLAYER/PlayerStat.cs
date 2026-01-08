using System;
using UnityEngine;

namespace __SCRIPTS
{
	[Serializable]
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
			Key,
			None,
			DaysSurvived,
			UpgradesPurchased,
			TimeSurvived,
			Experience,
			Level
		}

		public  StatType type;
		[SerializeField]private float value;

		public void SetStat(float newValue)
		{
			value = newValue;
		}

		public float GetStatAmount() => value;

		public void IncreaseStat(float change)
		{

			value += change;
		}
	}
}
