using System.Collections;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using VInspector;
#endif
namespace __SCRIPTS
{
	public enum UnitCategory
	{
		Character,
		Enemy,
		Obstacle,
		Thing
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

	public class UnitStatsManager : MonoBehaviour, IService
	{
		[Header("Configuration"), SerializeField]

		public UnitStatsDatabase database;

		[Header("Status"), SerializeField]

		private bool isInitialized = false;

		private Dictionary<string, UnitStatsData> unitStatsLookup = new();
		private Dictionary<string, string> nameCleaningCache = new();
		private Coroutine autoRefreshCoroutine;

		// Cache version to invalidate cached stats in Life components
		private int cacheVersion = 0;
		public int CacheVersion => cacheVersion;

		public event System.Action<bool> OnDataLoaded; // bool = success
		public bool IsLoading => database != null && database.isLoading;
		public bool IsInitialized => isInitialized;
		public string LastLoadTime => database?.lastLoadTime ?? "";
		public string LastError => database?.lastError ?? "";

		public void StartService()
		{
			LoadData();
		}


		private void LoadData()
		{


			// Try to use cached data first at runtime
			if (Application.isPlaying && database != null)
			{
				BuildLookupDictionary();
				OnDataLoaded?.Invoke(true);
				return;
			}

			if (database == null)
			{
				CreateDatabaseAsset();
				return;
			}
			Debug.Log("loading from google sheets");

			// Load from Google Sheets if no cached data or if explicitly requested
			database.OnDataLoaded += OnDatabaseLoaded;
			database.LoadFromGoogleSheets(this);
		}

		private void OnDatabaseLoaded(bool success)
		{
			database.OnDataLoaded -= OnDatabaseLoaded;

			if (success)
			{
				BuildLookupDictionary();
				StartAutoRefresh();

#if UNITY_EDITOR
				// Save the loaded data to the asset in editor
				UnityEditor.EditorUtility.SetDirty(database);
				UnityEditor.AssetDatabase.SaveAssets();
#else
#endif
			}

			OnDataLoaded?.Invoke(success);
		}

		private void BuildLookupDictionary()
		{
			unitStatsLookup.Clear();
			nameCleaningCache.Clear();
			cacheVersion++; // Increment cache version to invalidate cached stats

			if (database?.allUnits != null)
			{
				foreach (var unit in database.allUnits)
				{
					if (unit != null && !string.IsNullOrEmpty(unit.unitName)) unitStatsLookup[unit.unitName] = unit;
				}
			}

			isInitialized = true;
			#if UNITY_EDITOR || DEVELOPMENT_BUILD
			#endif
		}

		/// <summary>
		/// Ensure the manager is initialized, especially important in edit mode
		/// </summary>
		private void EnsureInitialized()
		{
			// If not initialized or lookup is empty, but we have database data
			if ((!isInitialized || unitStatsLookup.Count == 0) && database != null && database.allUnits != null && database.allUnits.Count > 0)
			{
				BuildLookupDictionary();
			}
		}

		private void StartAutoRefresh()
		{
			if (autoRefreshCoroutine != null) StopCoroutine(autoRefreshCoroutine);

			if (database?.googleSheetsConfig.autoRefreshInterval > 0)
				autoRefreshCoroutine = StartCoroutine(AutoRefreshCoroutine());
		}

		private IEnumerator AutoRefreshCoroutine()
		{
			while (database != null && database.googleSheetsConfig.autoRefreshInterval > 0)
			{
				yield return new WaitForSeconds(database.googleSheetsConfig.autoRefreshInterval);

				if (!IsLoading && database != null)
				{
					LoadData();
				}
			}
		}

		/// <summary>
		/// Get unit stats by name with improved name matching
		/// </summary>
		/// <param name="unitName">Name of the unit</param>
		/// <returns>UnitStatsData or null if not found</returns>
		public UnitStatsData GetUnitStats(string unitName)
		{
			#if UNITY_EDITOR || DEVELOPMENT_BUILD
			#endif
			if (string.IsNullOrEmpty(unitName)) return GetDefaultLifeStats();

			// Validate database is properly loaded
			if (!IsDatabaseReady())
			{
				// TODO: Replace with structured logging
				// Debug.LogWarning($"Database not ready for unit lookup: {unitName}. Using default Life stats.");
				return GetDefaultLifeStats();
			}

			// Cache the original name for debugging
			string originalName = unitName;

			// Clean the unit name with improved logic
			unitName = CleanUnitName(unitName);

			// Try to find the unit stats
			var foundStats = LookupUnitStats(unitName, originalName);

			// If not found, return default Life stats
			if (foundStats == null)
			{
				// TODO: Replace with structured logging
				// Debug.LogWarning($"Unit stats not found for: '{originalName}' (cleaned: '{unitName}'). Using default Life stats.");
				return GetDefaultLifeStats();
			}

			return foundStats;
		}

		/// <summary>
		/// Check if database is properly loaded and ready for lookups
		/// </summary>
		private bool IsDatabaseReady()
		{
			// Ensure we're initialized in edit mode
			EnsureInitialized();

			// Check if we have a database with data and lookup is built
			bool hasDatabase = database != null && database.allUnits != null && database.allUnits.Count > 0;
			bool hasLookup = unitStatsLookup != null && unitStatsLookup.Count > 0;
			bool initialized = isInitialized;

			#if UNITY_EDITOR || DEVELOPMENT_BUILD
			#endif

			return hasDatabase && hasLookup && initialized;
		}

		/// <summary>
		/// Perform the actual unit stats lookup with all fallback logic
		/// </summary>
		private UnitStatsData LookupUnitStats(string cleanedName, string originalName)
		{
			// Special case for Life components - return Life stats directly
			if (cleanedName == "Life" || cleanedName == "life")
			{
				if (unitStatsLookup.TryGetValue("Life", out var lifeStats))
				{
					return lifeStats;
				}
			}

			// Try exact match first
			if (unitStatsLookup.TryGetValue(cleanedName, out var stats))
			{
				#if UNITY_EDITOR || DEVELOPMENT_BUILD
				#endif
				return stats;
			}

			// Try fuzzy matching for complex names
			var fuzzyMatch = FindFuzzyMatch(cleanedName);
			if (fuzzyMatch != null)
			{
				#if UNITY_EDITOR || DEVELOPMENT_BUILD
				#endif
				return fuzzyMatch;
			}

			// No match found
			return null;
		}

		/// <summary>
		/// Get default Life stats as fallback
		/// </summary>
		private UnitStatsData GetDefaultLifeStats()
		{
			// If database is ready, try to get Life stats
			if (IsDatabaseReady() && unitStatsLookup.TryGetValue("Life", out var lifeStats))
			{
				return lifeStats;
			}

			// Database not ready or no Life entry - return null to indicate we need to handle this
			// TODO: Replace with structured logging
			// Debug.LogError("No default Life stats available - database may not be loaded");
			return null;
		}

		/// <summary>
		/// Clean unit name by removing common suffixes and patterns with memoization
		/// </summary>
		private string CleanUnitName(string unitName)
		{
			if (string.IsNullOrEmpty(unitName)) return unitName;

			// Check cache first
			if (nameCleaningCache.TryGetValue(unitName, out var cachedResult))
			{
				return cachedResult;
			}

			string cleanedName = unitName;

			// Remove (Clone) suffix
			cleanedName = cleanedName.Replace("(Clone)", "");

			// Remove trailing numbers
			cleanedName = System.Text.RegularExpressions.Regex.Replace(cleanedName, @"\d+$", "");

			// Remove "Variant" suffixes (including multiple ones)
			cleanedName = System.Text.RegularExpressions.Regex.Replace(cleanedName, @"(\s+Variant)+$", "");

			// Trim whitespace
			cleanedName = cleanedName.Trim();

			// Cache the result
			nameCleaningCache[unitName] = cleanedName;

			return cleanedName;
		}

		/// <summary>
		/// Find fuzzy match for complex door/wall names
		/// </summary>
		private UnitStatsData FindFuzzyMatch(string cleanedName)
		{
			if (!isInitialized || unitStatsLookup == null) return null;

			string lowerName = cleanedName.ToLower();

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
			string[] words = cleanedName.Split(' ');
			if (words.Length > 0)
			{
				string firstWord = words[0];
				if (unitStatsLookup.TryGetValue(firstWord, out var partialMatch))
				{
					return partialMatch;
				}
			}

			return null;
		}

		/// <summary>
		/// Get all units of a specific category
		/// </summary>
		/// <param name="category">Category to filter by</param>
		/// <returns>List of units in that category</returns>
		public List<UnitStatsData> GetUnitsByCategory(UnitCategory category)
		{
			var result = new List<UnitStatsData>();

			if (database?.allUnits != null)
			{
				foreach (var unit in database.allUnits)
				{
					if (unit != null && unit.category == category) result.Add(unit);
				}
			}

			return result;
		}

		/// <summary>
		/// Get all unit names
		/// </summary>
		/// <returns>Array of all unit names</returns>
		public string[] GetAllUnitNames() => new List<string>(unitStatsLookup.Keys).ToArray();

		/// <summary>
		/// Check if a unit exists
		/// </summary>
		/// <param name="unitName">Name to check</param>
		/// <returns>True if unit exists</returns>
		public bool HasUnit(string unitName) => isInitialized && unitStatsLookup.ContainsKey(unitName);

		/// <summary>
		/// Manually refresh data from Google Sheets
		/// </summary>
		 #if UNITY_EDITOR
		[Button()]
		#endif
		public void RefreshData()
		{
			LoadData();
		}

		/// <summary>
		/// Force reload from Google Sheets (bypasses cache)
		/// </summary>
		//[Button()]


		private void OnDestroy()
		{
			if (autoRefreshCoroutine != null) StopCoroutine(autoRefreshCoroutine);
		}

#if UNITY_EDITOR
		[Button()]

		public void ForceReloadFromGoogleSheets()
		{
			if (database == null)
			{
				CreateDatabaseAsset();
				return;
			}

			database.OnDataLoaded += OnDatabaseLoaded;
			database.LoadFromGoogleSheets(this);
		}
		[ContextMenu("Create Database Asset")]
		private void CreateDatabaseAsset()
		{
			if (database == null)
			{
				database = ScriptableObject.CreateInstance<UnitStatsDatabase>();
				UnityEditor.AssetDatabase.CreateAsset(database, "Assets/UnitStatsDatabase.asset");
				UnityEditor.AssetDatabase.SaveAssets();
			}
		}

		[ContextMenu("Load Data Now (Editor)")]
		//[Button("Load Data Now (Editor)")]
		private void LoadDataEditor()
		{
			if (Application.isPlaying)
			{
				ForceReloadFromGoogleSheets();
			}
			else
			{
				LoadDataInEditMode();
			}
		}

		private async void LoadDataInEditMode()
		{
			if (database == null)
			{
				CreateDatabaseAsset();
				return;
			}

			database.isLoading = true;
			database.lastError = "";

			string url = database.googleSheetsConfig.GetCSVUrl();

			try
			{
				using (var client = new System.Net.Http.HttpClient())
				{
					client.Timeout = System.TimeSpan.FromSeconds(30);
					var response = await client.GetStringAsync(url);

					// Parse the CSV data directly in edit mode
					ParseCSVDataInEditor(response);
					database.lastLoadTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

					// Save to asset
					UnityEditor.EditorUtility.SetDirty(database);
					UnityEditor.AssetDatabase.SaveAssets();

				}
			}
			catch (System.Exception e)
			{
				database.lastError = $"Error: {e.Message}";
				// TODO: Replace with structured logging
				// Debug.LogError($"Error loading from Google Sheets: {e.Message}");
			}
			finally
			{
				database.isLoading = false;
			}
		}

		private void ParseCSVDataInEditor(string csvText)
		{
			database.allUnits.Clear();

			string[] lines = csvText.Split('\n');
			if (lines.Length <= 1) return;

			// Parse header
			string[] headers = ParseCSVLine(lines[0]);

			// Parse data rows
			for (int i = 1; i < lines.Length; i++)
			{
				if (string.IsNullOrWhiteSpace(lines[i])) continue;

				string[] values = ParseCSVLine(lines[i]);
				if (values.Length < headers.Length)
				{
					// Pad with empty strings if row is shorter
					System.Array.Resize(ref values, headers.Length);
					for (int j = 0; j < values.Length; j++)
					{
						if (values[j] == null) values[j] = "";
					}
				}

				// Create dictionary for this row
				System.Collections.Generic.Dictionary<string, string> rowData = new System.Collections.Generic.Dictionary<string, string>();
				for (int j = 0; j < headers.Length && j < values.Length; j++)
				{
					rowData[headers[j].Trim()] = values[j]?.Trim() ?? "";
				}

				// Skip rows without a name
				if (!rowData.ContainsKey("Name") || string.IsNullOrEmpty(rowData["Name"]))
					continue;

				// Create UnitStatsData
				UnitStatsData unitStats = new UnitStatsData(rowData);
				database.allUnits.Add(unitStats);
			}
		}

		private string[] ParseCSVLine(string line)
		{
			var result = new System.Collections.Generic.List<string>();
			bool inQuotes = false;
			string currentField = "";

			for (int i = 0; i < line.Length; i++)
			{
				char c = line[i];

				if (c == '"')
				{
					inQuotes = !inQuotes;
				}
				else if (c == ',' && !inQuotes)
				{
					result.Add(currentField);
					currentField = "";
				}
				else
				{
					currentField += c;
				}
			}

			result.Add(currentField);
			return result.ToArray();
		}

		[ContextMenu("Clear Cached Data")]
		 #if UNITY_EDITOR
		[Button()]
		#endif
		private void ClearCachedData()
		{
			if (database != null)
			{
				database.allUnits.Clear();
				UnityEditor.EditorUtility.SetDirty(database);
				UnityEditor.AssetDatabase.SaveAssets();
			}

			if (Application.isPlaying)
			{
				unitStatsLookup.Clear();
				isInitialized = false;
			}
		}

		[ContextMenu("Test Name Cleaning")]
		 #if UNITY_EDITOR
		[Button()]
		#endif
		private void TestNameCleaning()
		{
			string[] testNames = {
				"R door exterior",
				"L door gb interior Variant Variant Variant",
				"R wall gb interior 2",
				"door normal closed(Clone)",
				"wall tile exterior 123"
			};

			foreach (string name in testNames)
			{
				string cleaned = CleanUnitName(name);
			}
		}
#endif
	}
}
