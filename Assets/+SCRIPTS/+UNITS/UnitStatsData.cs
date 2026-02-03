using System;
using System.Collections.Generic;
using UnityEngine;

namespace __SCRIPTS
{
	[Serializable]
	public class UnitStatsData
	{
		[Header("Basic Info")] public string unitName;
		public UnitCategory category;
		public DebrisType debrisType;
		public Color debrisTint;

		[Header("Combat Properties")] public bool isPlayerSwingHittable = true;

		public bool isInvincible;

		[Header("Core Stats")] public float healthMax;
		public float moveSpeed;
		public float dashSpeed;
		public float jumpSpeed;
		public float aggroRange;

		[Header("Attack 1")] public float attack1Damage;
		public float attack1Rate;
		public float attack1Range;

		[Header("Attack 2")] public float attack2Damage;
		public float attack2Rate;
		public float attack2Range;

		[Header("Attack 3")] public float attack3Damage;
		public float attack3Rate;
		public float attack3Range;

		[Header("Attack 4")] public float attack4Damage;
		public float attack4Rate;
		public float attack4Range;

		[Header("Rewards")] public float experienceGiven;

		public bool hasData;
		public bool isDefault;

		// Constructor for creating from CSV data
		public UnitStatsData(Dictionary<string, string> csvRow)
		{
			hasData = true;

			unitName = Get(csvRow, "Name");

			// Enums
			if (Enum.TryParse(Get(csvRow, "Category"), out UnitCategory cat))
				category = cat;

			if (Enum.TryParse(Get(csvRow, "DebrisType"), out DebrisType debris))
				debrisType = debris;

			TryColor(csvRow, "DebrisTint", out var tint);
			debrisTint = tint;

			// Booleans
			isPlayerSwingHittable = IsYes(csvRow, "IsPlayerSwingHittable");

			isInvincible = IsYes(csvRow, "IsInvincible");

			// Core stats
			TryFloat(csvRow, "HealthMax", out healthMax);
			TryFloat(csvRow, "MoveSpeed", out moveSpeed);
			TryFloat(csvRow, "DashSpeed", out dashSpeed);
			TryFloat(csvRow, "JumpSpeed", out jumpSpeed);
			TryFloat(csvRow, "AggroRange", out aggroRange);

			// Attacks
			TryFloat(csvRow, "Attack1Damage", out attack1Damage);
			TryFloat(csvRow, "Attack1Rate", out attack1Rate);
			TryFloat(csvRow, "Attack1Range", out attack1Range);

			TryFloat(csvRow, "Attack2Damage", out attack2Damage);
			TryFloat(csvRow, "Attack2Rate", out attack2Rate);
			TryFloat(csvRow, "Attack2Range", out attack2Range);

			TryFloat(csvRow, "Attack3Damage", out attack3Damage);
			TryFloat(csvRow, "Attack3Rate", out attack3Rate);
			TryFloat(csvRow, "Attack3Range", out attack3Range);

			TryFloat(csvRow, "Attack4Damage", out attack4Damage);
			TryFloat(csvRow, "Attack4Rate", out attack4Rate);
			TryFloat(csvRow, "Attack4Range", out attack4Range);

			// Rewards
			TryFloat(csvRow, "ExperienceGiven", out experienceGiven);
		}

		#region Helpers

		static string Get(Dictionary<string, string> row, string key) =>
			row.TryGetValue(key, out var value) ? value : string.Empty;

		static bool IsYes(Dictionary<string, string> row, string key) =>
			row.TryGetValue(key, out var value) && value.Trim().Equals("YES", StringComparison.OrdinalIgnoreCase);

		static void TryFloat(Dictionary<string, string> row, string key, out float result)
		{
			result = 0f;
			if (row.TryGetValue(key, out var value))
				float.TryParse(value, out result);
		}

		static void TryColor(Dictionary<string, string> row, string key, out Color result)
		{
			result = Color.white;
			if (row.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value))
				ColorUtility.TryParseHtmlString(value.StartsWith("#") ? value : "#" + value, out result);
		}

		#endregion
	}

	[Serializable]
	public class GoogleSheetsConfig
	{
		[Tooltip("Your Google Sheets document ID (found in the URL)")]
		public string documentId = "1TD3K4_Ni7r12HFs2ZjdgauYoS8Ppt7xaG43M0DLJYfk";

		[Tooltip("The sheet name/tab (usually 'Sheet1')")]
		public string sheetName = "Sheet1";

		[Tooltip("Auto-load data on start")] public bool autoLoadOnStart = true;

		[Tooltip("Auto-refresh interval in seconds (0 = no auto-refresh)")]
		public float autoRefreshInterval;

		public string GetCSVUrl() =>
			$"https://docs.google.com/spreadsheets/d/{documentId}/gviz/tq?tqx=out:csv&sheet={sheetName}";
	}
}
