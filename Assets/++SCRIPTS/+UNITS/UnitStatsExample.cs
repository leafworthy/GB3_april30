using System.Collections.Generic;
using UnityEngine;

namespace GangstaBean.Units
{
	public class UnitStatsExample : MonoBehaviour
	{
		[SerializeField] private string unitNameToTest = "BrockLee";

		private void Start()
		{
			// Example of how to use the UnitStatsManager
			StartCoroutine(TestUnitStats());
		}

		private System.Collections.IEnumerator TestUnitStats()
		{
			// Wait a frame to ensure UnitStatsManager is initialized
			yield return null;

			// Get specific unit stats
			UnitStatsData brockLee = UnitStatsManager.I.GetUnitStats(unitNameToTest);
			if (brockLee != null)
			{
				Debug.Log($"{brockLee.unitName} - Health: {brockLee.healthMax}, Speed: {brockLee.moveSpeed}");
				Debug.Log(
					$"Attack 1 - Damage: {brockLee.attack1Damage}, Rate: {brockLee.attack1Rate}, Range: {brockLee.attack1Range}");
			}

			// Get all enemies
			List<UnitStatsData> enemies = UnitStatsManager.I.GetUnitsByCategory(UnitCategory.Enemy);
			Debug.Log($"Found {enemies.Count} enemies");

			// List all unit names
			string[] allUnits = UnitStatsManager.I.GetAllUnitNames();
			Debug.Log($"All units: {string.Join(", ", allUnits)}");
		}
	}
}