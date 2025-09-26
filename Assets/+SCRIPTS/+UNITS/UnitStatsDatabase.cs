using System;
using System.Collections;
using System.Collections.Generic;
using __SCRIPTS;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu(menuName = "My Assets/UnitStatsDatabase")]
public class UnitStatsDatabase : ScriptableObject
{
	[Header("Google Sheets Configuration")]
	public GoogleSheetsConfig googleSheetsConfig = new();

	[Header("Database")] public List<UnitStatsData> allUnits = new();

	[Header("Status")] public bool isLoading;
	public string lastLoadTime = "";
	public string lastError = "";

	public event Action<bool> OnDataLoaded; // bool = success

	// Internal coroutine runner - singleton pattern
	private static CoroutineRunner coroutineRunner;



	// Overloaded method - backwards compatible
	public void LoadFromGoogleSheets(MonoBehaviour _coroutineRunner = null)
	{
		if (string.IsNullOrEmpty(googleSheetsConfig.documentId))
		{
			OnDataLoaded?.Invoke(false);
			return;
		}

		if (_coroutineRunner != null)
			_coroutineRunner.StartCoroutine(LoadDataCoroutine());
		else
			RunAndCleanup(LoadDataCoroutine());

	}

	private static CoroutineRunner GetCoroutineRunner()
	{
		if (coroutineRunner == null)
		{
			var runnerObject = new GameObject("UnitStatsDatabase_CoroutineRunner");
			coroutineRunner = runnerObject.AddComponent<CoroutineRunner>();
		}

		return coroutineRunner;
	}

	/// <summary>
	/// Helper to run a coroutine and then auto-destroy the runner when done.
	/// </summary>
	private static void RunAndCleanup(IEnumerator routine)
	{
		var runner = GetCoroutineRunner();
		runner.StartCoroutine(RunWithCleanup(routine));
	}

	private static IEnumerator RunWithCleanup(IEnumerator routine)
	{
		yield return routine;

		if (coroutineRunner != null)
		{
			Debug.Log("huh");
			DestroyImmediate(coroutineRunner.gameObject);
			coroutineRunner = null;
		}
	}

[Button]
	public void LoadFromGoogleSheets()
	{
		LoadFromGoogleSheets(null);
	}

	private IEnumerator LoadDataCoroutine()
	{
		isLoading = true;
		lastError = "";

		var url = googleSheetsConfig.GetCSVUrl();

		using (var request = UnityWebRequest.Get(url))
		{
			// Set timeout
			request.timeout = 30;

			yield return request.SendWebRequest();

			if (request.result == UnityWebRequest.Result.Success)
			{
				try
				{
					ParseCSVData(request.downloadHandler.text);
					lastLoadTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
					Debug.Log($"Successfully loaded {allUnits.Count} units from Google Sheets");
					OnDataLoaded?.Invoke(true);
				}
				catch (Exception e)
				{
					lastError = $"Parse error: {e.Message}";
					OnDataLoaded?.Invoke(false);
				}
			}
			else
			{
				lastError = $"Network error: {request.error}";
				OnDataLoaded?.Invoke(false);
			}
		}

		isLoading = false;
	}

	private void ParseCSVData(string csvText)
	{
		allUnits.Clear();

		var lines = csvText.Split('\n');
		if (lines.Length <= 1) return;

		// Parse header
		var headers = ParseCSVLine(lines[0]);

		// Parse data rows
		for (var i = 1; i < lines.Length; i++)
		{
			if (string.IsNullOrWhiteSpace(lines[i])) continue;

			var values = ParseCSVLine(lines[i]);
			if (values.Length < headers.Length)
			{
				// Pad with empty strings if row is shorter
				Array.Resize(ref values, headers.Length);
				for (var j = 0; j < values.Length; j++)
				{
					if (values[j] == null)
						values[j] = "";
				}
			}

			// Create dictionary for this row
			var rowData = new Dictionary<string, string>();
			for (var j = 0; j < headers.Length && j < values.Length; j++)
				rowData[headers[j].Trim()] = values[j]?.Trim() ?? "";

			// Skip rows without a name
			if (!rowData.ContainsKey("Name") || string.IsNullOrEmpty(rowData["Name"]))
				continue;

			// Create UnitStatsData
			var unitStats = new UnitStatsData(rowData);
			allUnits.Add(unitStats);
		}
	}

	private string[] ParseCSVLine(string line)
	{
		var result = new List<string>();
		var inQuotes = false;
		var currentField = "";

		for (var i = 0; i < line.Length; i++)
		{
			var c = line[i];

			if (c == '"')
				inQuotes = !inQuotes;
			else if (c == ',' && !inQuotes)
			{
				result.Add(currentField);
				currentField = "";
			}
			else
				currentField += c;
		}

		result.Add(currentField);
		return result.ToArray();
	}

	// Clean up when the application quits
	private void OnDestroy()
	{
		if (coroutineRunner == null) return;
		if (Application.isPlaying) DestroyImmediate(coroutineRunner.gameObject);
		coroutineRunner = null;
	}

	private class CoroutineRunner : MonoBehaviour
	{
		private void Awake()
		{
			// Hide in hierarchy to reduce clutter
			gameObject.hideFlags = HideFlags.HideInHierarchy;
		}
	}

#if UNITY_EDITOR
	// Editor utility methods
	[UnityEditor.MenuItem("Tools/Stats/Force Reload All Databases")]
	private static void ForceReloadAllDatabases()
	{
		var databases = Resources.FindObjectsOfTypeAll<UnitStatsDatabase>();
		foreach (var database in databases)
		{
			database.LoadFromGoogleSheets();
		}
	}

	// Context menu for individual database assets
	[ContextMenu("Reload from Google Sheets")]
	private void ReloadFromGoogleSheets()
	{
		LoadFromGoogleSheets();
	}
#endif
}
