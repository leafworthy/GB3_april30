using UnityEditor;
using UnityEngine;

namespace __SCRIPTS.Plugins.Editor
{
    /// <summary>
    /// Custom editor for the SmartZombieSpawningSystem to provide visualization and debugging tools.
    /// </summary>
    [CustomEditor(typeof(SmartZombieSpawningSystem))]
    public class SmartZombieSpawningSystemEditor : UnityEditor.Editor
    {
        private bool showSpawnRateCurve = true;
        private bool showSpawnPointsSettings = true;
    
        public override void OnInspectorGUI()
        {
            SmartZombieSpawningSystem spawner = (SmartZombieSpawningSystem)target;
        
            EditorGUILayout.Space();
        
            // Buttons for quick actions
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create New Spawn Point"))
            {
                CreateNewSpawnPoint(spawner);
            }
        
       
            EditorGUILayout.EndHorizontal();
        
            EditorGUILayout.Space();
        
            // Draw custom difficulty curve visualization
            showSpawnRateCurve = EditorGUILayout.Foldout(showSpawnRateCurve, "Difficulty Curve Over Time", true);
            if (showSpawnRateCurve)
            {
                DrawDifficultyCurve(spawner);
            }
            
            // Draw enemy unlock timeline
            DrawEnemyUnlockTimeline(spawner);
        
            EditorGUILayout.Space();
        
            // Draw default inspector
            DrawDefaultInspector();
        
            EditorGUILayout.Space();
        
            // Draw spawn points settings
            showSpawnPointsSettings = EditorGUILayout.Foldout(showSpawnPointsSettings, "Spawn Points", true);
            if (showSpawnPointsSettings)
            {
                DrawSpawnPointsSettings(spawner);
            }
        
            // Apply changes
            if (GUI.changed)
            {
                EditorUtility.SetDirty(spawner);
            }
        }
    
        private void DrawDifficultyCurve(SmartZombieSpawningSystem spawner)
        {
            float height = 100;
            Rect curveRect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth - 40, height);
        
            if (Event.current.type == EventType.Repaint)
            {
                // Draw background
                EditorGUI.DrawRect(curveRect, new Color(0.2f, 0.2f, 0.2f));
            
                // Draw grid lines
                Handles.color = new Color(0.3f, 0.3f, 0.3f);
            
                // Vertical lines (time progression)
                for (int i = 0; i <= 10; i += 2)
                {
                    float x = curveRect.x + (i / 10f) * curveRect.width;
                    Handles.DrawLine(new Vector3(x, curveRect.y), new Vector3(x, curveRect.y + curveRect.height));
                }
            
                // Horizontal lines (difficulty level)
                for (int i = 0; i <= 10; i += 2)
                {
                    float y = curveRect.y + curveRect.height - (i / 10f) * curveRect.height;
                    Handles.DrawLine(new Vector3(curveRect.x, y), new Vector3(curveRect.x + curveRect.width, y));
                }
            
                // Draw time labels
                GUI.color = Color.white;
                GUI.Label(new Rect(curveRect.x, curveRect.y + curveRect.height + 5, 50, 20), "Start");
                GUI.Label(new Rect(curveRect.x + curveRect.width * 0.5f - 20, curveRect.y + curveRect.height + 5, 50, 20), "Mid");
                GUI.Label(new Rect(curveRect.x + curveRect.width - 40, curveRect.y + curveRect.height + 5, 50, 20), "Max");
            
                // Draw the difficulty curve
                Handles.color = Color.yellow;
                Vector3 prevPoint = Vector3.zero;
            
                for (float t = 0; t <= 1; t += 0.01f)
                {
                    float value = spawner.GetCurrentDifficulty();
                    if (Application.isPlaying)
                    {
                        // If playing, show actual current difficulty
                        value = spawner.GetCurrentDifficulty();
                    }
                    else
                    {
                        // If not playing, show the curve shape
                        // We can't access the private difficultyOverTime curve, so show a representative curve
                        value = Mathf.SmoothStep(0f, 1f, t);
                    }
                    
                    float x = curveRect.x + t * curveRect.width;
                    float y = curveRect.y + curveRect.height - value * curveRect.height;
                
                    Vector3 newPoint = new Vector3(x, y);
                
                    if (t > 0)
                    {
                        Handles.DrawLine(prevPoint, newPoint);
                    }
                
                    prevPoint = newPoint;
                }
            
                // Draw current progress marker if playing
                if (Application.isPlaying)
                {
                    float currentDifficulty = spawner.GetCurrentDifficulty();
                    float gameTime = Time.time;
                    float difficultyRampTime = 300f; // Default value, can't access private field
                    float progress = Mathf.Clamp01(gameTime / difficultyRampTime);
                
                    float markerX = curveRect.x + progress * curveRect.width;
                    float markerY = curveRect.y + curveRect.height - currentDifficulty * curveRect.height;
                
                    Handles.color = Color.red;
                    float markerSize = 5f;
                
                    Handles.DrawLine(
                        new Vector3(markerX - markerSize, markerY - markerSize),
                        new Vector3(markerX + markerSize, markerY + markerSize)
                    );
                
                    Handles.DrawLine(
                        new Vector3(markerX - markerSize, markerY + markerSize),
                        new Vector3(markerX + markerSize, markerY - markerSize)
                    );
                
                    // Display current difficulty
                    EditorGUI.LabelField(
                        new Rect(markerX + 10, markerY - 20, 100, 20), 
                        $"Progress: {progress:F2}"
                    );
                
                    EditorGUI.LabelField(
                        new Rect(markerX + 10, markerY, 100, 20), 
                        $"Difficulty: {currentDifficulty:F2}"
                    );
                }
            }
        
            EditorGUILayout.Space();
        }
        
        private void DrawEnemyUnlockTimeline(SmartZombieSpawningSystem spawner)
        {
            EditorGUILayout.LabelField("Enemy Unlock Timeline", EditorStyles.boldLabel);
            
            // Get unlock times using reflection since they're private
            var toastUnlockTime = GetPrivateField<float>(spawner, "toastUnlockTime");
            var coneUnlockTime = GetPrivateField<float>(spawner, "coneUnlockTime");
            var donutUnlockTime = GetPrivateField<float>(spawner, "donutUnlockTime");
            var cornUnlockTime = GetPrivateField<float>(spawner, "cornUnlockTime");
            var difficultyRampTime = GetPrivateField<float>(spawner, "difficultyRampTime");
            
            float maxTime = Mathf.Max(toastUnlockTime, coneUnlockTime, donutUnlockTime, cornUnlockTime, difficultyRampTime);
            if (maxTime <= 0) maxTime = 300f; // Fallback
            
            float height = 60;
            Rect timelineRect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth - 40, height);
            
            if (Event.current.type == EventType.Repaint)
            {
                // Draw background
                EditorGUI.DrawRect(timelineRect, new Color(0.2f, 0.2f, 0.2f));
                
                // Draw time markers
                Handles.color = new Color(0.3f, 0.3f, 0.3f);
                for (int i = 0; i <= 10; i++)
                {
                    float time = (i / 10f) * maxTime;
                    float x = timelineRect.x + (i / 10f) * timelineRect.width;
                    Handles.DrawLine(new Vector3(x, timelineRect.y), new Vector3(x, timelineRect.y + timelineRect.height));
                    
                    // Draw time labels
                    GUI.color = Color.white;
                    GUI.Label(new Rect(x - 15, timelineRect.y + timelineRect.height + 5, 30, 20), $"{time:F0}s");
                }
                
                // Draw enemy unlock points
                var enemies = new[]
                {
                    ("Toast", toastUnlockTime, Color.yellow),
                    ("Cone", coneUnlockTime, Color.green),
                    ("Donut", donutUnlockTime, Color.magenta),
                    ("Corn", cornUnlockTime, Color.red)
                };
                
                foreach (var enemy in enemies)
                {
                    if (enemy.Item2 <= maxTime)
                    {
                        float x = timelineRect.x + (enemy.Item2 / maxTime) * timelineRect.width;
                        float y = timelineRect.y + timelineRect.height * 0.5f;
                        
                        // Draw unlock marker
                        Handles.color = enemy.Item3;
                        Handles.DrawSolidDisc(new Vector3(x, y), Vector3.forward, 4f);
                        
                        // Draw enemy name
                        GUI.color = enemy.Item3;
                        GUI.Label(new Rect(x - 20, y - 25, 40, 20), enemy.Item1);
                    }
                }
                
                // Draw current time marker if playing
                if (Application.isPlaying)
                {
                    float gameStartTime = GetPrivateField<float>(spawner, "gameStartTime");
                    float currentGameTime = Time.time - gameStartTime;
                    
                    if (currentGameTime <= maxTime)
                    {
                        float x = timelineRect.x + (currentGameTime / maxTime) * timelineRect.width;
                        
                        Handles.color = Color.white;
                        Handles.DrawLine(new Vector3(x, timelineRect.y), new Vector3(x, timelineRect.y + timelineRect.height));
                        
                        // Draw current time
                        GUI.color = Color.white;
                        GUI.Label(new Rect(x - 15, timelineRect.y - 20, 30, 20), $"{currentGameTime:F1}s");
                    }
                }
            }
            
            EditorGUILayout.Space();
        }
        
        private T GetPrivateField<T>(object obj, string fieldName)
        {
            var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                return (T)field.GetValue(obj);
            }
            return default(T);
        }
    
        private void DrawSpawnPointsSettings(SmartZombieSpawningSystem spawner)
        {
            SmartZombieSpawnPoint[] spawnPoints = spawner.GetComponentsInChildren<SmartZombieSpawnPoint>();
        
            if (spawnPoints.Length == 0)
            {
                EditorGUILayout.HelpBox("No spawn points found. Create some using the button above.", MessageType.Info);
                return;
            }
        
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
                EditorGUILayout.BeginHorizontal();
                spawnPoints[i].gameObject.SetActive(EditorGUILayout.Toggle(GUIContent.none, spawnPoints[i].gameObject.activeSelf, GUILayout.Width(20)));
                EditorGUILayout.LabelField($"Spawn Point {i + 1}: {spawnPoints[i].name}", EditorStyles.boldLabel);
            
                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    Selection.activeGameObject = spawnPoints[i].gameObject;
                }
            
                EditorGUILayout.EndHorizontal();
            
                // Display spawn point properties
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField($"Active: {spawnPoints[i].IsActive()}");
                EditorGUILayout.LabelField($"Position: {spawnPoints[i].transform.position}");
                EditorGUI.indentLevel--;
            
                EditorGUILayout.EndVertical();
            
                EditorGUILayout.Space();
            }
        }
    
        private void CreateNewSpawnPoint(SmartZombieSpawningSystem spawner)
        {
            GameObject newPoint = new GameObject($"SpawnPoint_{spawner.transform.childCount + 1}");
            newPoint.transform.parent = spawner.transform;
            newPoint.transform.position = spawner.transform.position;
        
            SmartZombieSpawnPoint spawnPoint = newPoint.AddComponent<SmartZombieSpawnPoint>();
        
            // Set defaults - the new spawn point has its own default settings
            spawnPoint.SetActive(true);
        
            // Select the new spawn point
            Selection.activeGameObject = newPoint;
        
            Undo.RegisterCreatedObjectUndo(newPoint, "Create Spawn Point");
        }
    

    
    }
}