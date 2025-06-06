using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using VInspector;

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

	public class UnitStatsManager : Singleton<UnitStatsManager>
	{
		[Header("Configuration"), SerializeField]

		private UnitStatsDatabase database;

		[Header("Status"), SerializeField]

		private bool isInitialized = false;

		private Dictionary<string, UnitStatsData> unitStatsLookup = new();
		private Coroutine autoRefreshCoroutine;

		public event System.Action<bool> OnDataLoaded; // bool = success
		public bool IsLoading => database != null && database.isLoading;
		public bool IsInitialized => isInitialized;
		public string LastLoadTime => database?.lastLoadTime ?? "";
		public string LastError => database?.lastError ?? "";

		private void Start()
		{
			LoadData();
			Debug.Log("load started");
		}

		/// <summary>
		/// Load data from Google Sheets
		/// </summary>
		public void LoadData()
		{
			if (database == null)
			{
				CreateDatabaseAsset();
				return;
			}

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
				Debug.Log("Unit stats data loaded successfully");
			}

			OnDataLoaded?.Invoke(success);
		}

		private void BuildLookupDictionary()
		{
			unitStatsLookup.Clear();

			if (database?.allUnits != null)
			{
				foreach (var unit in database.allUnits)
				{
					if (unit != null && !string.IsNullOrEmpty(unit.unitName)) unitStatsLookup[unit.unitName] = unit;
				}
			}

			isInitialized = true;
			Debug.Log($"Initialized UnitStatsManager with {unitStatsLookup.Count} units");
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
					Debug.Log("Auto-refreshing unit stats data...");
					LoadData();
				}
			}
		}

		/// <summary>
		/// Get unit stats by name
		/// </summary>
		/// <param name="unitName">Name of the unit</param>
		/// <returns>UnitStatsData or null if not found</returns>
		public UnitStatsData GetUnitStats(string unitName)
		{
			Debug.Log("trying " + unitName);
			unitName = unitName.Replace("(Clone)", "");
			if (unitStatsLookup.TryGetValue(unitName, out var stats))
			{
				Debug.Log("found it first time");
				return stats;
			}
			
			if (!IsLoading)
			{
				LoadData();
				if (unitStatsLookup.TryGetValue(unitName, out var stats2))
				{
					Debug.Log("found it second time");
					return stats2;
				}
			}
			Debug.Log($"Unit stats not found for: {unitName} using default stats");
			// Return default stats if not found
			 if(unitStatsLookup.TryGetValue(unitName, out var defaultName))
			 {
				 return defaultName;
			 }

			 Debug.LogWarning($"default stats not found for: {unitName}, returning null");
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
		[Button()]
		public void RefreshData()
		{
			LoadData();
		}

		private void OnDestroy()
		{
			if (autoRefreshCoroutine != null) StopCoroutine(autoRefreshCoroutine);
		}

#if UNITY_EDITOR
		[ContextMenu("Create Database Asset")]
		private void CreateDatabaseAsset()
		{
			if (database == null)
			{
				database = ScriptableObject.CreateInstance<UnitStatsDatabase>();
				UnityEditor.AssetDatabase.CreateAsset(database, "Assets/UnitStatsDatabase.asset");
				UnityEditor.AssetDatabase.SaveAssets();
				Debug.Log("Created UnitStatsDatabase asset");
			}
		}



		[ContextMenu("Load Data Now")]
		private void LoadDataEditor()
		{
			if (Application.isPlaying)
				LoadData();
			else
				Debug.Log("Can only load data in Play Mode");
		}
#endif
	}
}
