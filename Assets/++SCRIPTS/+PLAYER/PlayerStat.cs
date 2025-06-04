using System;
using UnityEngine;

namespace GangstaBean.Player
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
			UpgradesPurchased
		}

		public  StatType type;
		[SerializeField]private float value;
	
		public void SetStat(float newValue)
		{
			value = newValue;
		}
	
		public float GetStatAmount() => value;

		public void ChangeStat(float change)
		{
			Debug.Log("chaging stat " + type + " by " + change);
			value += change;
		}
	}
}