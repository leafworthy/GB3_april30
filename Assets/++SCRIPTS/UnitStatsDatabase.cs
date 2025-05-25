 using System;
 using System.Collections;
 using System.Collections.Generic;
 using __SCRIPTS;
 using UnityEngine;
 using UnityEngine.Networking;

 [CreateAssetMenu(menuName = "My Assets/UnitStatsDatabase")]
    public class UnitStatsDatabase : ScriptableObject
    {
        [Header("Google Sheets Configuration")]
        public GoogleSheetsConfig googleSheetsConfig = new GoogleSheetsConfig();

        [Header("Database")]
        public List<UnitStatsData> allUnits = new List<UnitStatsData>();

        [Header("Status")]
        public bool isLoading = false;
        public string lastLoadTime = "";
        public string lastError = "";

        public event System.Action<bool> OnDataLoaded; // bool = success

        public void LoadFromGoogleSheets(MonoBehaviour coroutineRunner)
        {
            if (string.IsNullOrEmpty(googleSheetsConfig.documentId))
            {
                Debug.LogError("Google Sheets Document ID is not set!");
                return;
            }

            coroutineRunner.StartCoroutine(LoadDataCoroutine());
        }

        private IEnumerator LoadDataCoroutine()
        {
            isLoading = true;
            lastError = "";

            string url = googleSheetsConfig.GetCSVUrl();
            Debug.Log($"Loading data from: {url}");

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                // Set timeout
                request.timeout = 30;

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        ParseCSVData(request.downloadHandler.text);
                        lastLoadTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        Debug.Log($"Successfully loaded {allUnits.Count} units from Google Sheets");
                        OnDataLoaded?.Invoke(true);
                    }
                    catch (System.Exception e)
                    {
                        lastError = $"Parse error: {e.Message}";
                        Debug.LogError($"Error parsing CSV data: {e.Message}");
                        OnDataLoaded?.Invoke(false);
                    }
                }
                else
                {
                    lastError = $"Network error: {request.error}";
                    Debug.LogError($"Error loading from Google Sheets: {request.error}");
                    OnDataLoaded?.Invoke(false);
                }
            }

            isLoading = false;
        }

        private void ParseCSVData(string csvText)
        {
            allUnits.Clear();

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
                    Array.Resize(ref values, headers.Length);
                    for (int j = 0; j < values.Length; j++)
                    {
                        if (values[j] == null) values[j] = "";
                    }
                }

                // Create dictionary for this row
                Dictionary<string, string> rowData = new Dictionary<string, string>();
                for (int j = 0; j < headers.Length && j < values.Length; j++)
                {
                    rowData[headers[j].Trim()] = values[j]?.Trim() ?? "";
                }

                // Skip rows without a name
                if (!rowData.ContainsKey("Name") || string.IsNullOrEmpty(rowData["Name"]))
                    continue;

                // Create UnitStatsData
                UnitStatsData unitStats = new UnitStatsData(rowData);
                allUnits.Add(unitStats);
            }
        }

        private string[] ParseCSVLine(string line)
        {
            List<string> result = new List<string>();
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
    }
