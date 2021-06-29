using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "My Assets/UnitStats")]
public class UnitStatsData : ScriptableObject
{
	public List<UnitStat> baseStats = new List<UnitStat>();

	public bool isPlayer;
}
