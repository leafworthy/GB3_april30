using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom editor for the SmartZombieSpawningSystem to provide visualization and debugging tools.
/// </summary>
[CustomEditor(typeof(SmartZombieSpawningSystem))]
public class SmartZombieSpawningSystemEditor : Editor
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
        
        // Draw custom spawn rate curve visualization
        showSpawnRateCurve = EditorGUILayout.Foldout(showSpawnRateCurve, "Spawn Rate by Time of Day", true);
        if (showSpawnRateCurve)
        {
            DrawSpawnRateCurve(spawner);
        }
        
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
    
    private void DrawSpawnRateCurve(SmartZombieSpawningSystem spawner)
    {
        float height = 100;
        Rect curveRect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth - 40, height);
        
        if (Event.current.type == EventType.Repaint)
        {
            // Draw background
            EditorGUI.DrawRect(curveRect, new Color(0.2f, 0.2f, 0.2f));
            
            // Draw grid lines
            Handles.color = new Color(0.3f, 0.3f, 0.3f);
            
            // Vertical lines (time of day)
            for (int i = 0; i <= 24; i += 6)
            {
                float x = curveRect.x + (i / 24f) * curveRect.width;
                Handles.DrawLine(new Vector3(x, curveRect.y), new Vector3(x, curveRect.y + curveRect.height));
            }
            
            // Horizontal lines (spawn rate)
            for (int i = 0; i <= 10; i += 2)
            {
                float y = curveRect.y + curveRect.height - (i / 10f) * curveRect.height;
                Handles.DrawLine(new Vector3(curveRect.x, y), new Vector3(curveRect.x + curveRect.width, y));
            }
            
            // Draw time labels
            GUI.color = Color.white;
            GUI.Label(new Rect(curveRect.x, curveRect.y + curveRect.height + 5, 50, 20), "12 AM");
            GUI.Label(new Rect(curveRect.x + curveRect.width * 0.25f - 20, curveRect.y + curveRect.height + 5, 50, 20), "6 AM");
            GUI.Label(new Rect(curveRect.x + curveRect.width * 0.5f - 20, curveRect.y + curveRect.height + 5, 50, 20), "12 PM");
            GUI.Label(new Rect(curveRect.x + curveRect.width * 0.75f - 20, curveRect.y + curveRect.height + 5, 50, 20), "6 PM");
            GUI.Label(new Rect(curveRect.x + curveRect.width - 40, curveRect.y + curveRect.height + 5, 50, 20), "12 AM");
            
            // Draw the curve
            Handles.color = Color.yellow;
            Vector3 prevPoint = Vector3.zero;
            
            for (float t = 0; t <= 1; t += 0.01f)
            {
                float value = spawner.spawnRateCurve.Evaluate(t);
                float x = curveRect.x + t * curveRect.width;
                float y = curveRect.y + curveRect.height - value * curveRect.height;
                
                Vector3 newPoint = new Vector3(x, y);
                
                if (t > 0)
                {
                    Handles.DrawLine(prevPoint, newPoint);
                }
                
                prevPoint = newPoint;
            }
            
            // Draw current time marker if day/night cycle is available
            if (Application.isPlaying)
            {
                float timeOfDay = DayNightCycle.I.GetCurrentDayFraction();
                float currentRate = spawner.spawnRateCurve.Evaluate(timeOfDay);
                
                float markerX = curveRect.x + timeOfDay * curveRect.width;
                float markerY = curveRect.y + curveRect.height - currentRate * curveRect.height;
                
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
                
                // Display current time and rate
                EditorGUI.LabelField(
                    new Rect(markerX + 10, markerY - 20, 100, 20), 
                    $"Time: {timeOfDay:F2}"
                );
                
                EditorGUI.LabelField(
                    new Rect(markerX + 10, markerY, 100, 20), 
                    $"Rate: {currentRate:F2}"
                );
            }
        }
        
        EditorGUILayout.Space();
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
            spawnPoints[i].isProximityBased = EditorGUILayout.Toggle("Proximity Based", spawnPoints[i].isProximityBased);
            spawnPoints[i].spawnAreaSize = EditorGUILayout.Vector2Field("Spawn Area Size", spawnPoints[i].spawnAreaSize);
            spawnPoints[i].showDebugVisuals = EditorGUILayout.Toggle("Show Debug Visuals", spawnPoints[i].showDebugVisuals);
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
        
        // Set defaults
        spawnPoint.spawnAreaSize = new Vector2(3, 3);
        
        // Select the new spawn point
        Selection.activeGameObject = newPoint;
        
        Undo.RegisterCreatedObjectUndo(newPoint, "Create Spawn Point");
    }
    

    
}