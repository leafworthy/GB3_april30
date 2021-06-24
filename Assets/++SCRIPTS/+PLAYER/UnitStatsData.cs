using System.Collections.Generic;
using UnityEngine;

namespace _SCRIPTS
{
	[CreateAssetMenu(menuName = "My Assets/UnitStats")]
	public class UnitStatsData : ScriptableObject
	{
		public List<UnitStat> baseStats = new List<UnitStat>();

		public bool isPlayer;
	}
}
