using System.Collections.Generic;
using UnityEngine;

public class LevelLoadInteraction : TimedInteraction
{
    [Header("Destination Settings")]
    [SerializeField] private GameScene.Type destinationScene;
    [SerializeField] private bool useLevelTransition = true;
    [SerializeField] private float interactionTime = 1.5f;
    
    [Header("Display Text")]
    [Tooltip("Custom display name for the destination (overrides SceneDefinition)")]
    [SerializeField] private string customDestinationName = "";
    
    [Tooltip("Display format used when prompting player (e.g. 'Go to {0}')")]
    [SerializeField] private string promptFormat = "Go to {0}";
    
    [Header("Spawn Point")]
    [Tooltip("ID of the exit spawn point in this scene")]
    [SerializeField] private string exitPointId;
    
    [Tooltip("ID of the entry spawn point in the destination scene")]
    [SerializeField] private string entryPointId;
    
    [Header("Persistent Characters")]
    [Tooltip("Keep character instances between scenes instead of respawning")]
    [SerializeField] private bool persistCharacters = true;
    
    // Cache of spawn points and destination name
    private SpawnPoint exitPoint;
    private string destinationDisplayName;
    
    // Cached player reference for saying lines
    private Player interactingPlayer;
    
    // Reference to interaction title component
    private InteractionTitle interactionTitle;
    
    private void Awake()
    {
        // Find our configured exit point (if any) in this scene
        if (!string.IsNullOrEmpty(exitPointId))
        {
            var spawnPoints = FindObjectsOfType<SpawnPoint>(true);
            
            foreach (var spawnPoint in spawnPoints)
            {
                if (spawnPoint.id == exitPointId)
                {
                    exitPoint = spawnPoint;
                    
                    // Auto-configure if exit point is valid
                    if (exitPoint != null)
                    {
                        // Set destination to match exit point if not already set
                        if (destinationScene == GameScene.Type.None)
                        {
                            destinationScene = exitPoint.destinationScene;
                        }
                        
                        // Set entry point ID if not already set
                        if (string.IsNullOrEmpty(entryPointId))
                        {
                            entryPointId = exitPoint.connectedSpawnPointId;
                        }
                    }
                    break;
                }
            }
        }
        
        // Get the destination display name
        UpdateDestinationDisplayName();
    }
    
    private void UpdateDestinationDisplayName()
    {
        // This method should only be called in play mode
        if (!Application.isPlaying)
        {
            #if UNITY_EDITOR
            // In edit mode, use the edit-mode specific method instead
            UpdateDisplayNameEditMode();
            #endif
            return;
        }
        
        // If a custom name is provided, use that
        if (!string.IsNullOrEmpty(customDestinationName))
        {
            destinationDisplayName = customDestinationName;
        }
        else
        {
            // Try to get the name from SceneDefinitionManager
            if (SceneDefinitionManager.Instance != null)
            {
                destinationDisplayName = SceneDefinitionManager.Instance.GetSceneDisplayName(destinationScene);
            }
            else
            {
                // Fallback to formatted scene type name
                destinationDisplayName = FormatSceneTypeName(destinationScene.ToString());
            }
        }
        
        // Update the interaction title if available
        if (interactionTitle != null)
        {
            interactionTitle.SetTitle(string.Format(promptFormat, destinationDisplayName));
        }
    }
    
    // Convert CamelCase to spaced words (e.g., "MainMenu" to "Main Menu")
    private string FormatSceneTypeName(string typeName)
    {
        string result = "";
        for (int i = 0; i < typeName.Length; i++)
        {
            if (i > 0 && char.IsUpper(typeName[i]))
            {
                result += " ";
            }
            result += typeName[i];
        }
        return result;
    }
    
    protected override void Start()
    {
        // Set the total time required to complete the interaction
        totalTime = interactionTime;
        
        // Try to get or add the InteractionTitle component (only in play mode)
        interactionTitle = GetComponent<InteractionTitle>();
        if (interactionTitle == null && Application.isPlaying)
        {
            interactionTitle = gameObject.AddComponent<InteractionTitle>();
        }
        
        // Update the destination display name if needed
        if (string.IsNullOrEmpty(destinationDisplayName))
        {
            UpdateDestinationDisplayName();
        }
        
        base.Start();
        
        // Subscribe to the completion event
        OnTimeComplete += OnLevelLoadComplete;
        
        // Subscribe to action press/release events to handle player saying
        OnActionPress += OnPlayerBeginInteraction;
        OnActionRelease += OnPlayerCancelInteraction;
    }
    
    private void OnPlayerBeginInteraction(Player player)
    {
        interactingPlayer = player;
        
        // Make the player say where they're going
        if (player != null && player.SpawnedPlayerGO != null)
        {
            // Only say line if we have a player sayer component
            var sayer = player.GetComponentInChildren<PlayerSayer>();
            if (sayer != null)
            {
                sayer.Say(string.Format(promptFormat, destinationDisplayName));
            }
        }
    }
    
    private void OnPlayerCancelInteraction(Player player)
    {
        interactingPlayer = null;
    }

    private void OnLevelLoadComplete(Player player)
    {
        // Verify exit point is valid
        if (exitPoint == null && !string.IsNullOrEmpty(exitPointId))
        {
            Debug.LogWarning($"LevelLoadInteraction: Exit point {exitPointId} not found in scene", this);
        }
        
        // Register the transition with LevelTransition (only in play mode)
        if (Application.isPlaying && LevelTransition.I != null)
        {
            // Use the spawn point ID (which is the exit point from this scene)
            if (!string.IsNullOrEmpty(exitPointId))
            {
                // Configure the transition with connected points
                LevelTransition.I.SetTransitionId(exitPointId);
                Debug.Log($"LevelLoadInteraction: Transitioning from {exitPointId} to {entryPointId} in {destinationScene}");
            }
        }
        
        // Set character persistence flag
        if (persistCharacters)
        {
            // Flag to keep character instances
            PlayerManager.ShouldPersistCharacters = true;
        }
        
        // Load the destination scene with transition effect
        SceneLoader.I.SetDestinationScene(destinationScene, useLevelTransition);
    }
    
    // Find an appropriate SpawnPoint in the scene
    public void AutoConfigureSpawnPoints()
    {
        if (string.IsNullOrEmpty(exitPointId))
        {
            // Look for spawn points near this interaction
            SpawnPoint nearestSpawnPoint = null;
            float nearestDistance = float.MaxValue;
            
            var spawnPoints = FindObjectsOfType<SpawnPoint>(true);
            
            foreach (var spawnPoint in spawnPoints)
            {
                // Only consider exit points
                if (spawnPoint.pointType != SpawnPointType.Exit && spawnPoint.pointType != SpawnPointType.Both)
                    continue;
                    
                float dist = Vector3.Distance(transform.position, spawnPoint.transform.position);
                if (dist < nearestDistance)
                {
                    nearestDistance = dist;
                    nearestSpawnPoint = spawnPoint;
                }
            }
            
            if (nearestSpawnPoint != null && nearestDistance < 5f) // Only auto-configure if within 5 units
            {
                exitPointId = nearestSpawnPoint.id;
                destinationScene = nearestSpawnPoint.destinationScene;
                entryPointId = nearestSpawnPoint.connectedSpawnPointId;
                
                // Cache the exit point
                exitPoint = nearestSpawnPoint;
                
                // Update display name
                UpdateDestinationDisplayName();
                
                #if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
                #endif
            }
        }
    }
    
    // Visualize the connection in the editor
    private void OnDrawGizmos()
    {
        #if UNITY_EDITOR
        // If we don't have a cached exit point, try to find it
        if (exitPoint == null && !string.IsNullOrEmpty(exitPointId))
        {
            var spawnPoints = FindObjectsOfType<SpawnPoint>(true);
            foreach (var point in spawnPoints)
            {
                if (point.id == exitPointId)
                {
                    exitPoint = point;
                    break;
                }
            }
        }
        
        // If we have an exit point, draw a connection
        if (exitPoint != null)
        {
            // Draw connection line to this interaction
            Gizmos.color = new Color(1f, 0.8f, 0.2f, 0.8f); // Golden yellow
            Gizmos.DrawLine(transform.position, exitPoint.transform.position);
            
            // Draw a small sphere at this interaction position
            Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.8f); // Light blue
            Gizmos.DrawSphere(transform.position, 0.3f);
            
            // Label the interaction
            string labelText = string.Format(promptFormat, destinationDisplayName);
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, labelText);
        }
        else if (!string.IsNullOrEmpty(destinationScene.ToString()) && destinationScene != GameScene.Type.None) 
        {
            // Just show destination if we don't have a linked spawn point
            Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.8f); // Light blue
            Gizmos.DrawSphere(transform.position, 0.3f);
            
            // Update display name if needed
            if (string.IsNullOrEmpty(destinationDisplayName))
            {
                UpdateDestinationDisplayName();
            }
            
            // Label with the destination
            string labelText = string.Format(promptFormat, destinationDisplayName);
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, labelText);
        }
        #endif
    }
    
    #if UNITY_EDITOR
    private void OnValidate()
    {
        // In edit mode, use a direct update method that doesn't rely on singletons
        UpdateDisplayNameEditMode();
        
        // If the destination scene is set but no exit point ID is set, try to auto-configure
        if (destinationScene != GameScene.Type.None && string.IsNullOrEmpty(exitPointId) && Application.isEditor && !Application.isPlaying)
        {
            AutoConfigureSpawnPoints();
        }
    }
    
    // Special edit-mode only method for updating display name without singletons
    private void UpdateDisplayNameEditMode()
    {
        // If a custom name is provided, use that
        if (!string.IsNullOrEmpty(customDestinationName))
        {
            destinationDisplayName = customDestinationName;
            return;
        }
        
        // Try to load scene definition directly using AssetDatabase
        var sceneDef = UnityEditor.AssetDatabase.LoadAssetAtPath<SceneDefinition>($"Assets/Resources/Scenes/{destinationScene}.asset");
        if (sceneDef != null && !string.IsNullOrEmpty(sceneDef.displayName))
        {
            destinationDisplayName = sceneDef.displayName;
        }
        else
        {
            // Use formatted scene name as fallback
            destinationDisplayName = FormatSceneTypeName(destinationScene.ToString());
        }
    }
    #endif
    
    private void OnDestroy()
    {
        // Clean up the event subscriptions
        OnTimeComplete -= OnLevelLoadComplete;
        OnActionPress -= OnPlayerBeginInteraction;
        OnActionRelease -= OnPlayerCancelInteraction;
    }
}