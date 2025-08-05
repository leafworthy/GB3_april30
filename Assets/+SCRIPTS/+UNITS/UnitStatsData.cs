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

		[Header("Combat Properties")] public bool isPlayerSwingHittable = true;
		public bool showLifeBar = false;
		public bool isInvincible = false;

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

		public bool hasData = false;

		// Constructor for creating from CSV data
		public  UnitStatsData(Dictionary<string, string> csvRow)
		{
			unitName = csvRow["Name"];
			hasData = true;
			// Parse category
			if (System.Enum.TryParse<UnitCategory>(csvRow["Category"], out var cat))
				category = cat;

			// Parse debris type
			if (System.Enum.TryParse<DebrisType>(csvRow["DebrisType"], out var debris))
				debrisType = debris;

			// Parse boolean values
			isPlayerSwingHittable = csvRow["IsPlayerSwingHittable"].ToUpper() == "YES";
			showLifeBar = csvRow["ShowLifeBar"].ToUpper() == "YES";
			isInvincible = csvRow["IsInvincible"].ToUpper() == "YES";

			// Parse float values with error handling
			float.TryParse(csvRow["HealthMax"], out healthMax);
			float.TryParse(csvRow["MoveSpeed"], out moveSpeed);
			float.TryParse(csvRow["DashSpeed"], out dashSpeed);
			float.TryParse(csvRow["JumpSpeed"], out jumpSpeed);
			float.TryParse(csvRow["AggroRange"], out aggroRange);

			// Parse attack stats
			float.TryParse(csvRow["Attack1Damage"], out attack1Damage);
			float.TryParse(csvRow["Attack1Rate"], out attack1Rate);
			float.TryParse(csvRow["Attack1Range"], out attack1Range);

			float.TryParse(csvRow["Attack2Damage"], out attack2Damage);
			float.TryParse(csvRow["Attack2Rate"], out attack2Rate);
			float.TryParse(csvRow["Attack2Range"], out attack2Range);

			float.TryParse(csvRow["Attack3Damage"], out attack3Damage);
			float.TryParse(csvRow["Attack3Rate"], out attack3Rate);
			float.TryParse(csvRow["Attack3Range"], out attack3Range);

			float.TryParse(csvRow["Attack4Damage"], out attack4Damage);
			float.TryParse(csvRow["Attack4Rate"], out attack4Rate);
			float.TryParse(csvRow["Attack4Range"], out attack4Range);
		}
	}

	[System.Serializable]
	public class GoogleSheetsConfig
	{
		[Tooltip("Your Google Sheets document ID (found in the URL)")]
		public string documentId = "1TD3K4_Ni7r12HFs2ZjdgauYoS8Ppt7xaG43M0DLJYfk";

		[Tooltip("The sheet name/tab (usually 'Sheet1')")]
		public string sheetName = "Sheet1";

		[Tooltip("Auto-load data on start")] public bool autoLoadOnStart = true;

		[Tooltip("Auto-refresh interval in seconds (0 = no auto-refresh)")]
		public float autoRefreshInterval = 0f;

		public string GetCSVUrl() =>
			$"https://docs.google.com/spreadsheets/d/{documentId}/gviz/tq?tqx=out:csv&sheet={sheetName}";
	}
}
