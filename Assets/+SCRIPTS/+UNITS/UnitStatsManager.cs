using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace __SCRIPTS
{
	public enum UnitCategory
	{
		Character,
		Enemy,
		Obstacle,
		Thing,
		NPC
	}

	public enum DebrisType
	{
		blood,
		metal,
		wood,
		cloth,
		glass,
		stone,
		wall,
		none
	}

	public static class UnitStatsManager
	{
		private static UnitStatsDatabase database;
		private static Dictionary<string, UnitStatsData> unitStatsLookup = new();
		private static bool initialized;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void ResetStatics()
		{
			Debug.Log("resetting stats");
			initialized = true;
			database = Resources.Load<UnitStatsDatabase>("UnitStatsDatabase");
			unitStatsLookup = BuildLookupDictionary();
		}
		private static Dictionary<string, UnitStatsData> BuildLookupDictionary()
		{
			if(!initialized) ResetStatics();
			var _unitStatsLookup = new Dictionary<string, UnitStatsData>();

			foreach (var unit in database.allUnits)
			{
				if (unit != null && !string.IsNullOrEmpty(unit.unitName)) _unitStatsLookup[unit.unitName] = unit;
			}

			return _unitStatsLookup;
		}

		public static UnitStatsData GetUnitStats(string unitName)
		{
			if (!initialized) ResetStatics();
			if(!Application.isPlaying)ResetStatics();
			if (string.IsNullOrEmpty(unitName))
			{
				return GetDefaultLifeStats();
			}

			unitName = CleanUnitName(unitName);

			var foundStats = LookupUnitStats(unitName);
			if (foundStats != null)
				foundStats.hasData = true;
			return foundStats;
		}

		private static UnitStatsData LookupUnitStats(string cleanedName)
		{
			if (!initialized) ResetStatics();
			if (cleanedName is "Life" or "stats")
			{
				if (unitStatsLookup.TryGetValue("DefaultThing", out var lifeStats))
					return lifeStats;
			}

			if (unitStatsLookup.TryGetValue(cleanedName, out var stats)) return stats;

			var fuzzyMatch = FindFuzzyMatch(cleanedName);
			Debug.Log(" fuzzy match for " + cleanedName + " is " + (fuzzyMatch != null ? fuzzyMatch.unitName : "null"));
			return fuzzyMatch ?? GetDefaultLifeStats();
		}

		private static UnitStatsData GetDefaultLifeStats() {
			if (!initialized) ResetStatics();
			var result = unitStatsLookup.GetValueOrDefault("DefaultThing");
			Debug.Log(result);
			return unitStatsLookup.GetValueOrDefault("DefaultThing");
		}

		private static string CleanUnitName(string unitName)
		{
			if (string.IsNullOrEmpty(unitName)) return unitName;

			var cleanedName = unitName;

			// 1. Remove Unity's "(Clone)" suffix
			cleanedName = cleanedName.Replace("(Clone)", "");

			// 2. Remove Unity/FBX import variants like "Variant", "Variant Variant", etc.
			cleanedName = Regex.Replace(cleanedName, @"(\s+Variant)+$", "");

			// 3. Remove Blender-style ".001", ".002" suffixes
			cleanedName = Regex.Replace(cleanedName, @"\.\d+$", "");

			// 4. Remove trailing digits (e.g. "Enemy123" -> "Enemy")
			cleanedName = Regex.Replace(cleanedName, @"\d+$", "");

			// 5. Remove common separator + number cases: "Enemy_01", "Enemy-02"
			cleanedName = Regex.Replace(cleanedName, @"[_\- ]\d+$", "");

			// 6. Normalize whitespace (collapse multiple spaces -> single space)
			cleanedName = Regex.Replace(cleanedName, @"\s{2,}", " ");

			// 7. Trim leading/trailing spaces and special chars
			cleanedName = cleanedName.Trim(' ', '_', '-', '.');

			// 8. Normalize Unicode (handles weird invisible characters)
			cleanedName = cleanedName.Normalize();

			return cleanedName;
		}

		private static UnitStatsData FindFuzzyMatch(string cleanedName)
		{
			var lowerName = cleanedName.ToLower();

			// Try door patterns based on actual database entries
			if (lowerName.Contains("door"))
			{
				// Try specific door types from database
				if (unitStatsLookup.TryGetValue("Door_Wood", out var doorWood)) return doorWood;
				if (unitStatsLookup.TryGetValue("Door_Metal", out var doorMetal)) return doorMetal;
				// Fallback to generic door
				if (unitStatsLookup.TryGetValue("door", out var doorGeneric)) return doorGeneric;
				if (unitStatsLookup.TryGetValue("Door", out doorGeneric)) return doorGeneric;
			}

			// Try gate patterns
			if (lowerName.Contains("gate") || lowerName.Contains("garage"))
			{
				// Try specific gate types from database
				if (unitStatsLookup.TryGetValue("Gate_Wood", out var gateWood)) return gateWood;
				if (unitStatsLookup.TryGetValue("Gate_Metal", out var gateMetal)) return gateMetal;
			}

			// Try wall patterns based on actual database entries
			if (lowerName.Contains("wall"))
			{
				// Try specific wall types from database
				if (unitStatsLookup.TryGetValue("Wall_Wood", out var wallWood)) return wallWood;
				if (unitStatsLookup.TryGetValue("Wall_Metal", out var wallMetal)) return wallMetal;
				// Fallback to generic wall
				if (unitStatsLookup.TryGetValue("wall", out var wallGeneric)) return wallGeneric;
				if (unitStatsLookup.TryGetValue("Wall", out wallGeneric)) return wallGeneric;
			}

			// Try partial matches (first word matching)
			var words = cleanedName.Split(' ');
			if (words.Length > 0)
			{
				var firstWord = words[0];
				if (unitStatsLookup.TryGetValue(firstWord, out var partialMatch)) return partialMatch;
			}

			return null;
		}
	}
}
